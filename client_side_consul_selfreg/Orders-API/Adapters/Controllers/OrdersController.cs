using System;
using System.Web.Http;
using Orders_Core;
using Orders_Core.Adapters.DataAccess;
using Orders_Core.Ports.Commands;
using Orders_Core.Ports.Resources;
using Orders_Core.Ports.ViewModelRetrievers;
using paramore.brighter.commandprocessor;

namespace Orders_API.Adapters.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly IOrdersDAO _ordersDao;
        private readonly IAmACommandProcessor _commandProcessor;

        public OrdersController(OrdersDAO ordersDao, IAmACommandProcessor commandProcessor)
        {
            _ordersDao = ordersDao;
            _commandProcessor = commandProcessor;
        }

        [HttpGet]
        public OrderListModel Get()
        {
            var orderRetriever = new OrderListModelRetriever(Globals.HostName, _ordersDao);
            return orderRetriever.RetrieveOrders();

        }

        [HttpPost]
        public OrderListModel Post(AddOrderModel addOrderModel)
        {
            _commandProcessor.Send(new AddOrderCommand(addOrderModel.CustomerName, addOrderModel.Description, DateTime.Parse(addOrderModel.DueDate)));

            return Get();
        }

    }
}
