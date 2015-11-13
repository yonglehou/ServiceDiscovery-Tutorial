using System;
using System.Net.Http;
using Orders_Client.Adapters.Configuration;
using Orders_Client.Adapters.Gateways;

namespace Orders_Client.Adapters.Program
{
    class OrdersClientController
    {
        private readonly Uri _baseAddres;

        public OrdersClientController()
        {
            var gatewayConfiguration = new HttpGatewayConfiguration();
            _baseAddres = gatewayConfiguration.Uri;   
        }

        public string Run()
        {
            var client = new HttpClientGateway().Client();
            try
            {
                client.BaseAddress = _baseAddres;
                var response = client.GetAsync("orders").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;

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
    }
}
