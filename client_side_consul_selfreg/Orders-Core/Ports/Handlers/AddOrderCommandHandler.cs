﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using Orders_Core.Adapters.DataAccess;
using Orders_Core.Model;
using Orders_Core.Ports.Commands;
using Orders_Core.Ports.Events;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.Logging;
using paramore.brighter.commandprocessor.policy.Attributes;

namespace Orders_Core.Ports.Handlers
{
    public class AddOrderCommandHandler : RequestHandler<AddOrderCommand>
    {
        private readonly IOrdersDAO _ordersDao;
        private readonly IAmACommandProcessor _commandProcessor;

        public AddOrderCommandHandler(IOrdersDAO ordersDao, ILog logger, IAmACommandProcessor commandProcessor) : base(logger)
        {
            _ordersDao = ordersDao;
            _commandProcessor = commandProcessor;
        }

                [Validation(step: 2, timing: HandlerTiming.Before)]

        [RequestLogging(step: 1, timing: HandlerTiming.Before)]
        [UsePolicy(CommandProcessor.RETRYPOLICY, step: 3)]
        public override AddOrderCommand Handle(AddOrderCommand addOrderCommand)
        {
            using (var scope = _ordersDao.BeginTransaction())
            {
                base.logger.DebugFormat(string.Format("Writing new order for customer: {0}", addOrderCommand.CustomerName));
                var inserted = _ordersDao.Add(
                    new Order(
                        customerName: addOrderCommand.CustomerName,
                        orderDescription: addOrderCommand.OrderDescription,
                        dueDate: addOrderCommand.OrderDueDate
                        )
                    );

                scope.Commit();

                addOrderCommand.OrderId = inserted.Id;
            }

            _commandProcessor.Publish(
                new OrderAddedEvent(
                    addOrderCommand.CustomerName, 
                    addOrderCommand.OrderDescription, 
                    addOrderCommand.OrderDueDate));

            return base.Handle(addOrderCommand);
        }
    }
}