using System;
using System.Collections.Generic;
using System.Configuration;

namespace Orders_Client.Adapters.Configuration
{
    class HttpGatewayConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("orderService")]
        public OrderServiceConfiguration OrderServiceConfiguration
        {
            get { return this["orderService"] as OrderServiceConfiguration; }
            set { this["orderService"] = value; }
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

    class OrderServiceConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("servers", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(OrderAPIServerElement))]
        public OrderAPIServersElement Servers
        {
            get
            {
                return (OrderAPIServersElement)this["servers"];
            }
        }
    }

    public class OrderAPIServerElement : ConfigurationElement
    {
        [ConfigurationProperty("uri", DefaultValue = "http://127.0.0.1:3476/", IsRequired = true)]
        public Uri Uri
        {
            get { return (Uri)this["uri"]; }
            set { this["uri"] = value; }
        }


        [ConfigurationProperty("Timeout", DefaultValue = "5000", IsRequired = true)]
        public string Timeout
        {
            get { return this["Timeout"] as string; }
            set { this["Timeout"] = value; }
        }
    }

    public class OrderAPIServersElement : ConfigurationElementCollection
    {
        private readonly List<OrderAPIServerElement > _elements = new List<OrderAPIServerElement>();

        protected override ConfigurationElement CreateNewElement()
        {
            var newElement = new OrderAPIServerElement();
            _elements.Add(newElement);
            return newElement;
        }

        public ConfigurationElement this[int i]
        {
            get
            {
                return (ConfigurationElement)BaseGet(i);
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OrderAPIServerElement)element).Uri;
        }
    }
}
