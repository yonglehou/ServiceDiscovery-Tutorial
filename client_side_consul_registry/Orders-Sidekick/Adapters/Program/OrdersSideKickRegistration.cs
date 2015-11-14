
using System;
using Consul;
using Orders_Sidekick.Adapters.Configuration;

namespace Orders_Sidekick.Adapters.Program
{
    class OrdersSideKickRegistration
    {

        public void Register()
        {
            var client = new Client();
            
            var gatewayConfiguration = HttpGatewayConfiguration.GetConfiguration();
            foreach (OrderAPIServerElement serverDefinition in gatewayConfiguration.OrderServiceConfiguration.Servers)
            {
                var registration = new AgentServiceRegistration()
                {
                    Name = "orders-api",
                    Address = serverDefinition.Uri.ToString(),
                };

                client.Agent.ServiceRegister(registration);

                var services = client.Agent.Services();
            }
        }
    }
}
