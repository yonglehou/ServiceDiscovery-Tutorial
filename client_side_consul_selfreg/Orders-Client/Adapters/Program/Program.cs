using System;

namespace Orders_Client.Adapters.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new OrdersClientController().Run());
        }
    }
}
