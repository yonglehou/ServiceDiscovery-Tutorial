using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Machine.Specifications;
using Orders_Core.Ports.Resources;

namespace Orders_Core.Adapters.Tests
{
    public class SerializationFixtures
    {
        private static T SerializeToXml<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, value);
                var xml = textWriter.ToString();

                using (var textReader = new StringReader(xml))
                {
                    var obj = (T)serializer.Deserialize(textReader);
                    return obj;
                }
            }
        }


        public class When_serializing_an_add_command_to_xml
        {
            private static AddOrderModel s_order;
            private static AddOrderModel s_orderModel;
            private static string s_now;

            private Establish _context = () =>
            {
                Globals.HostName = "host.com";
                s_now = DateTime.Now.ToString("o");
                s_order= new AddOrderModel ()
                {
                    Description = "Honey Cake",
                    CustomerName = "Pooh Bear",
                    DueDate = s_now
                };
            };

            private Because _of = () => s_orderModel = SerializeToXml(s_order);

            private It _should_set_the_name = () => s_orderModel.Description.ShouldEqual("Honey Cake");
            private It _should_set_the_title = () => s_orderModel.CustomerName.ShouldEqual("Pooh Bear");
            private It _should_set_the_profile_name = () => s_orderModel.DueDate.ShouldEqual(s_now);
        }

    }
}
