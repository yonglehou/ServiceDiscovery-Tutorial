using System;
using System.Net;
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

        public HttpClient Client(double timeout)
        {
            _client = new ThreadLocal<HttpClient>(() => CreateClient(timeout));
            return _client.Value;
        }


        private HttpClient CreateClient(double timeout)
        {
            var requestHandler = new WebRequestHandler
            {
                AllowPipelining = true,
                AllowAutoRedirect = true,
                Proxy = new WebProxy("http://localhost:8888", false),
                CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate)
            };
            var client = HttpClientFactory.Create(requestHandler);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
            return client;
        }

    }
}
