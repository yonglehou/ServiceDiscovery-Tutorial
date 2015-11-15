using Consul;

namespace Orders_Sidekick.Adapters.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            var registrationService = new OrdersSideKickRegistration();
            var client = new Client();
            registrationService.Register(client);
            registrationService.DisplayRegisteredServices(client);
        }
    }
}
