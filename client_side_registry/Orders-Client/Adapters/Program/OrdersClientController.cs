using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Orders_Client.Adapters.Configuration;
using Orders_Client.Adapters.Gateways;
using Orders_Client.Adapters.Parsers;
using Orders_Core.Ports.Resources;

namespace Orders_Client.Adapters.Program
{
    class OrdersClientController
    {
        private readonly Uri _baseAddres;

        public OrdersClientController()
        {
            var gatewayConfiguration = HttpGatewayConfiguration.GetConfiguration();
            _baseAddres = gatewayConfiguration.OrderServiceConfiguration.Uri;   
        }

        public string Run()
        {
            var client = new HttpClientGateway().Client();
            try
            {
                client.BaseAddress = _baseAddres;
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
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    Console.Write(e.Message);
                    if (e.InnerException != null)
                        Console.WriteLine(" : " + e.InnerException);
                    else
                        Console.WriteLine();

                }
            }
            catch (Exception he)
            {
                Console.WriteLine("Exception talking to server: {0}", he);
            }
            finally
            {
                client.Dispose();
            }
            return null;
        }

        public HttpRequestMessage CreateRequest(string uri, StringContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _baseAddres + uri) { Content = content };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Xml);
            return request;
        }
    }
}
