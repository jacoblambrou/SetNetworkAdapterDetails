using NetworkAdapter.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetworkAdapter.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            bool activeOnly = false;

            Console.WriteLine("Only display active interfaces? Y/N");
            var input = Console.ReadLine();

            if (input == "y" || input == "Y")
                activeOnly = true;

            if (activeOnly)
            {
                foreach (NetworkInterface nic in nics)
                {
                    var adapter = new Library.NetworkAdapter(nic);
                    if (adapter.Status == "Up")
                        Console.WriteLine(adapter.DisplayAdapterInfo());
                }
            }
            else
            {
                foreach (NetworkInterface nic in nics)
                {
                    var adapter = new Library.NetworkAdapter(nic);
                    Console.WriteLine(adapter.DisplayAdapterInfo());
                }
            }
        }


    }
}
