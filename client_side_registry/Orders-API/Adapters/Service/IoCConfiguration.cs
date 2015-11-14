using System;
using System.IO;
using System.Reflection;
using Microsoft.Practices.Unity;
using Orders_API.Adapters.Configuration;
using Orders_API.Adapters.Controllers;
using Orders_Core.Adapters.DataAccess;
using Orders_Core.Ports;
using Orders_Core.Ports.Commands;
using Orders_Core.Ports.Events;
using Orders_Core.Ports.Handlers;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.Logging;
using paramore.brighter.commandprocessor.messagestore.mssql;
using paramore.brighter.commandprocessor.messaginggateway.rmq;
using Polly;

namespace Orders_API.Adapters.Service
{
    internal static class IoCConfiguration
    {
        public static void Run(UnityContainer container)
        {
            container.RegisterType<OrdersController>();
            container.RegisterInstance(typeof(ILog), LogProvider.For<OrderService>(), new ContainerControlledLifetimeManager());
            container.RegisterType<IOrdersDAO, OrdersDAO>();
            container.RegisterType<AddOrderCommandHandler>();
            container.RegisterType<OrderAddedEventHandler>();

            var handlerFactory = new UnityHandlerFactory(container);

            var subscriberRegistry = new SubscriberRegistry();
            subscriberRegistry.Register<AddOrderCommand, AddOrderCommandHandler>();
            subscriberRegistry.Register<OrderAddedEvent, OrderAddedEventHandler>();

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(new[]
                    {
                        TimeSpan.FromMilliseconds(50),
                        TimeSpan.FromMilliseconds(100),
                        TimeSpan.FromMilliseconds(150)
                    });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(1, TimeSpan.FromMilliseconds(500));

            var policyRegistry = new PolicyRegistry()
            {
                {CommandProcessor.RETRYPOLICY, retryPolicy},
                {CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy}
            };


            var messageMapperFactory = new UnityMessageMapperFactory(container);
            var messageMapperRegistry = new MessageMapperRegistry(messageMapperFactory);
            messageMapperRegistry.Register<OrderUpdateCommand, OrderUpdateCommandMessageMapper>();

            var gateway = new RmqMessageProducer(container.Resolve<ILog>());

            var dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8)), "App_Data\\MessageStore.sdf");
            IAmAMessageStore<Message> sqlMessageStore = new MsSqlMessageStore(new MsSqlMessageStoreConfiguration("DataSource=\"" + dbPath + "\"", "Messages", MsSqlMessageStoreConfiguration.DatabaseType.SqlCe), container.Resolve<ILog>());

            var commandProcessor = CommandProcessorBuilder.With()
                    .Handlers(new HandlerConfiguration(subscriberRegistry, handlerFactory))
                    .Policies(policyRegistry)
                    .Logger(container.Resolve<ILog>())
                    .TaskQueues(new MessagingConfiguration(sqlMessageStore, gateway, messageMapperRegistry))
                    .RequestContextFactory(new InMemoryRequestContextFactory())
                    .Build();

            container.RegisterInstance(typeof(IAmACommandProcessor), commandProcessor);
        }
    }
}
