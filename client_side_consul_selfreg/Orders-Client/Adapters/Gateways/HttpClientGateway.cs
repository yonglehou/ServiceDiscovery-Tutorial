using System;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Orders_Client.Adapters.Gateways
{
    class HttpClientGateway
    {
        public HttpClient Client(Uri uri, double timeout)
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
            client.BaseAddress = uri;
            return client;
        }
    }
}
