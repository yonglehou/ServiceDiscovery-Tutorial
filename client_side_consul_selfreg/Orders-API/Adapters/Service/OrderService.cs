using System;
using Consul;
using Microsoft.Owin.Hosting;
using Orders_API.Adapters.Configuration;
using Orders_Core;
using Topshelf;

namespace Orders_API.Adapters.Service
{
    class OrderService : ServiceControl
    {
        private IDisposable _app;
        public bool Start(HostControl hostControl)
        {
            var configuration = OrderServerConfiguration.GetConfiguration();
            var address = configuration.Server.Address;
            var port = configuration.Server.Port;
            var uri = new Uri(string.Format("{0}:{1}/", address, port));
            Globals.HostName = uri.Host + ":" + uri.Port;
            _app = WebApp.Start<StartUp>(uri.AbsoluteUri);
            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            _app.Dispose();
            return true;
        }

        public void Shutdown(HostControl hostcontrol)
        {
            var client = new Client();
            var configuration = OrderServerConfiguration.GetConfiguration();
            client.Agent.ServiceDeregister(configuration.Server.Id);

            return;
        }
    }
}
