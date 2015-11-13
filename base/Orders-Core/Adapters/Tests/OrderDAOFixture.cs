#region Licence
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

using System;
using Machine.Specifications;
using Orders_Core.Adapters.DataAccess;
using Orders_Core.Model;

namespace Orders_Core.Adapters.Tests
{
    [Subject(typeof(OrdersDAO))]
    public class When_updating_an_order
    {
        private static OrdersDAO s_dao;
        private static Order s_newOrder;
        private static Order s_addedOrder;
        private static Order s_foundOrder;
        private const string NEW_CUSTOMER_NAME = "Bob Smith";
        private const string NEW_ORDER_DESCRIPTION = "New Task Description";
        private static readonly DateTime? s_NEW_DUE_DATE = DateTime.Now.AddDays(1);
        private static readonly DateTime? s_NEW_COMPLETION_DATE = DateTime.Now.AddDays(2);

        private Establish _context = () =>
        {
            s_dao = new OrdersDAO();
            s_dao.Clear();
            s_newOrder = new Order(customerName: "Test Name", orderDescription: "Test Description", dueDate: DateTime.Now);
            s_addedOrder = s_dao.Add(s_newOrder);
            s_addedOrder.CustomerName = NEW_CUSTOMER_NAME;
            s_addedOrder.OrderDescription = NEW_ORDER_DESCRIPTION;
            s_addedOrder.DueDate = s_NEW_DUE_DATE;
            s_addedOrder.CompletionDate = s_NEW_COMPLETION_DATE;
        };

        private Because _of = () => s_dao.Update(s_addedOrder);

        private It _should_add_the_order_into_the_list = () => GetOrder().ShouldNotBeNull();
        private It _should_set_the_order_name = () => GetOrder().CustomerName.ShouldEqual(NEW_CUSTOMER_NAME);
        private It _should_set_the_order_description = () => GetOrder().OrderDescription.ShouldEqual(NEW_ORDER_DESCRIPTION);
        private It _should_set_the_order_duedate = () => GetOrder().DueDate.Value.ToShortDateString().ShouldEqual(s_NEW_DUE_DATE.Value.ToShortDateString());
        private It _should_set_the_order_completion_date = () => GetOrder().CompletionDate.Value.ToShortDateString().ShouldEqual(s_NEW_COMPLETION_DATE.Value.ToShortDateString());

        private static Order GetOrder()
        {
            return s_foundOrder ?? (s_foundOrder = s_dao.FindById(s_addedOrder.Id));
        }
    }
}