
using System;
using System.Text;
using Consul;
using Orders_Sidekick.Adapters.Configuration;

namespace Orders_Sidekick.Adapters.Program
{
    class OrdersSideKickRegistration
    {

        /// <summary>
        /// Registers the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        public void Register(Client client)
        {
            var gatewayConfiguration = HttpGatewayConfiguration.GetConfiguration();
            foreach (OrderAPIServerElement serverDefinition in gatewayConfiguration.OrderServiceConfiguration.Servers)
            {
                var registration = new AgentServiceRegistration()
                {
                    ID =serverDefinition.Id,
                    Name = serverDefinition.Name,
                    Address = serverDefinition.Address,
                    Port = serverDefinition.Port,
                    Tags = new []{"Orders"}
                };

                //clear any old registration - we don't respond to services running/not running in this version
                client.Agent.ServiceDeregister(registration.ID);
                client.Agent.ServiceRegister(registration);
            }
        }

        public void DisplayRegisteredServices(Client client)
        {
            Console.WriteLine("List of services registred by agent");
            var services = client.Agent.Services().Response;
            foreach (var key in services.Keys)
            {
                var service = services[key];
                var serviceDefinition = new StringBuilder();
                serviceDefinition.AppendLine("Service Definition [");
                serviceDefinition.AppendFormat("ID: {0}", service.ID);
                serviceDefinition.AppendLine();
                serviceDefinition.AppendFormat("Address: {0}", service.Address);
                serviceDefinition.AppendLine();
                serviceDefinition.AppendFormat("Port: {0}", service.Port);
                serviceDefinition.AppendLine();
                foreach (var tag in service.Tags)
                {
                    serviceDefinition.AppendFormat("Tag: {0}", tag);
                }
                serviceDefinition.AppendLine("]");

                Console.WriteLine(serviceDefinition);
            }
            
        }

    }
}
