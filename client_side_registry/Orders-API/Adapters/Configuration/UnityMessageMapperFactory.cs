using System;
using Microsoft.Practices.Unity;
using paramore.brighter.commandprocessor;

namespace Orders_API.Adapters.Configuration
{
    internal class UnityMessageMapperFactory : IAmAMessageMapperFactory
    {
        private readonly UnityContainer _container;

        public UnityMessageMapperFactory (UnityContainer container)
        {
            _container = container;
        }

        public IAmAMessageMapper Create(Type messageMapperType)
        {
            return (IAmAMessageMapper)_container.Resolve(messageMapperType);
        }
    }
}
