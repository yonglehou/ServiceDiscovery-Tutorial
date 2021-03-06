﻿using System;
using System.Collections.Generic;
using System.Configuration;

namespace Orders_Sidekick.Adapters.Configuration
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
            return ((OrderAPIServerElement)element).Id;
        }
    }
}
