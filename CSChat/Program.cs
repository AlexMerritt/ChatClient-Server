using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSChat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select the application");
            Console.WriteLine("1) Clent");
            Console.WriteLine("2) Server");

            var key = Console.ReadKey();
            Console.Clear();
            if (key.Key == ConsoleKey.D1)
            {
                Client c = new Client();
            }
            else if (key.Key == ConsoleKey.D2)
            {
                ChatServer s = new ChatServer(0);
            }

            Console.Read();
        }
    }
}
