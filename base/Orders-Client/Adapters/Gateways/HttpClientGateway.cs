using System;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Orders_Client.Adapters.Configuration;

namespace Orders_Client.Adapters.Gateways
{
    class HttpClientGateway
    {
        public HttpGatewayConfiguration GatewayConfiguration { get; private set; }
        private ThreadLocal<HttpClient> _client;
        private double _timeout;

        public HttpClient Client()
        {
            GatewayConfiguration = new HttpGatewayConfiguration();
            _timeout = Convert.ToDouble(GatewayConfiguration.Timeout);
            _client = new ThreadLocal<HttpClient>(() => CreateClient(_timeout));
            return _client.Value;
        }


        private HttpClient CreateClient(double timeout)
        {
            var requestHandler = new WebRequestHandler
            {
                AllowPipelining = true,
                AllowAutoRedirect = true,
                CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate)
            };
            var client = HttpClientFactory.Create(requestHandler);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            return client;
        }

    }
}
