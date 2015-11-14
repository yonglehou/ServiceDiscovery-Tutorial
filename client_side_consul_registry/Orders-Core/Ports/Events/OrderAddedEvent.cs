using System;
using paramore.brighter.commandprocessor;

namespace Orders_Core.Ports.Events
{
    public class OrderAddedEvent : Event
    {
        public string CustomerName { get; set; }
        public string OrderDescription { get; set; }
        public DateTime? OrderDueDate { get; set; }

        public OrderAddedEvent(string customerName, string orderDescription, DateTime? orderDueDate)
            : base(Guid.NewGuid())
        {
            CustomerName = customerName;
            OrderDescription = orderDescription;
            OrderDueDate = orderDueDate;
        }
    }
}
