using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Consul;
using Orders_Client.Adapters.Gateways;
using Orders_Client.Adapters.Parsers;
using Orders_Core.Ports.Resources;
using Polly;

namespace Orders_Client.Adapters.Program
{
    class OrdersClientController
    {
        private readonly IList<Server> _serverList = new List<Server>() ;
        private Server _currentServer = null;
        private static int _currentServerIndex = 0;
        private readonly Policy _retryPolicy;

        public OrdersClientController()
        {
            var client = new Client();
            var services = client.Agent.Services().Response;
            foreach (var service in services)
            {
                bool isOrderService = service.Value.Tags.Any(tag => tag == "Orders");
                if (isOrderService)
                {
                    var server = new Server()
                    {
                        Timeout = 5000,
                        Uri = new Uri(string.Format("{0}:{1}/", service.Value.Address, service.Value.Port))
                    };
                    _serverList.Add(server);
                }
            }

            _retryPolicy = Policy
                .Handle<AggregateException>()
                .Or<Exception>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(150),
                    TimeSpan.FromMilliseconds(250), 
                }, (exception, timespan) =>
                {
                    LogError(exception);
                    TrySelectNextServer();
                });

            TrySelectNextServer();
        }


        public string Run()
        {
            var result = _retryPolicy.Execute(() =>
            {
                using (var client = new HttpClientGateway().Client(_currentServer.Uri, _currentServer.Timeout))
                {
                    return PostOrder(client);
                }
            });
            return result;
        }

        private string PostOrder(HttpClient client)
        {

            var order = new AddOrderModel()
            {
                CustomerName = "Winnie the Pooh",
                Description = "Pot of Honey",
                DueDate = DateTime.UtcNow.ToString("o")
            };

            string orderBody;
            XmlRequestBuilder.TryBuild(order, out orderBody);
            var requestMessage = CreateRequest("orders", new StringContent(orderBody));
            var response = client.SendAsync(requestMessage).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public HttpRequestMessage CreateRequest(string uri, StringContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _currentServer.Uri + uri) { Content = content };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Xml);
            return request;
        }

        private void TrySelectNextServer()
        {
            _currentServer = _serverList[_currentServerIndex];
            _currentServerIndex++;
            if (_currentServerIndex > _serverList.Count - 1)
                _currentServerIndex = 0;
        }

        private void LogError(Exception exception)
        {
            if (exception is AggregateException)
            {
                var e = exception as AggregateException;
                foreach (var ae in e.Flatten().InnerExceptions)
                {
                    Console.Write(e.Message);
                    if (e.InnerException != null)
                        Console.WriteLine(" : " + e.InnerException);
                    else
                        Console.WriteLine();
                }
            }
            else
            {
                var e = exception as Exception;
                Console.WriteLine("Exception talking to server: {0}", e);
            }
        }

        private class Server
        {
            public Uri Uri { get; set; }
            public double Timeout { get; set; }
        }
    }
}
