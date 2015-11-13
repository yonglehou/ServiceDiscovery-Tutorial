using Orders_Core.Ports.Commands;
using Orders_Core.Ports.Events;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.Logging;
using paramore.brighter.commandprocessor.policy.Attributes;

namespace Orders_Core.Ports.Handlers
{
    public class OrderAddedEventHandler : RequestHandler<OrderAddedEvent> 
    {
        private readonly IAmACommandProcessor _commandProcessor;

        public OrderAddedEventHandler(ILog logger, IAmACommandProcessor commandProcessor)
            : base(logger)
        {
            _commandProcessor = commandProcessor;
        }

        [RequestLogging(step: 1, timing: HandlerTiming.Before)]
        [UsePolicy(CommandProcessor.RETRYPOLICY, 2)]
        public override OrderAddedEvent Handle(OrderAddedEvent @event)
        {
            _commandProcessor.Post(new OrderUpdateCommand(orderName: @event.CustomerName, dueDate: @event.OrderDueDate.Value, recipient:"paramore.brighter@yahoo.co.uk", copyTo: null));
            return base.Handle(@event);
        }
    }
}
