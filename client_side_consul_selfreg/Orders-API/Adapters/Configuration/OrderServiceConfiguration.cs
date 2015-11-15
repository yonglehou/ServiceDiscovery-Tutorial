using System;
using System.Configuration;

namespace Orders_API.Adapters.Configuration
{
    public class OrderServerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("server")]
        public OrderServer Server
        {
            get { return this["server"] as OrderServer ; }
            set { this["server"] = value; }
        }

        public static OrderServerConfiguration  GetConfiguration()
        {
            var configuration =
                ConfigurationManager.GetSection("orderServer") as OrderServerConfiguration;

            if (configuration != null)
                return configuration;

            return new OrderServerConfiguration ();
        }
    }

    public class OrderServer : ConfigurationElement
    {
        [ConfigurationProperty("id", DefaultValue = "Unknown", IsRequired = true)]
        public string Id
        {
            get { return (string)this["id"]; } 
            set { this["id"] = value; } 
        }

        [ConfigurationProperty("name", DefaultValue = "Unknown", IsRequired = true)]
        public string Name
        { 
            get { return (string)this["name"]; } 
            set { this["name"] = value; } 
        }

        [ConfigurationProperty("uri", DefaultValue = "http://localhost", IsRequired = true)]
        public string Address
        {
            get { return this["uri"] as string; }
            set { this["uri"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue = 3476, IsRequired = true)]
        public int Port
        {
            get { return Convert.ToInt32(this["port"]); } 
            set { this["port"] = value; } 
        }

        [ConfigurationProperty("Timeout", DefaultValue = "5000", IsRequired = true)]
        public string Timeout
        {
            get { return this["Timeout"] as string; }
            set { this["Timeout"] = value; }
        }
    }
}
