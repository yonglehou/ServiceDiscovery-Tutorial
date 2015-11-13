using System;
using System.Configuration;

namespace Orders_Client.Adapters.Configuration
{
    class HttpGatewayConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("uri", DefaultValue = "http://localhost:3476/orders", IsRequired = true)]
        public Uri Uri
        {
            get { return (Uri)this["uri"]; }
            set { this["uri"] = value; }
        }


        [ConfigurationProperty("Timeout", DefaultValue = "500", IsRequired = true)]
        public string Timeout
        {
            get { return this["Timeout"] as string; }
            set { this["Timeout"] = value; }
        }


        public static HttpGatewayConfiguration GetConfiguration()
        {
            var configuration =
                ConfigurationManager.GetSection("httpGatewayConfiguration") as HttpGatewayConfiguration;

            if (configuration != null)
                return configuration;

            return new HttpGatewayConfiguration();
        }

    }
}
