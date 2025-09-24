using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Solar Service Host";

            SolarService service = new SolarService();

            using (ServiceHost host = new ServiceHost(service))
            {
                host.Open();
                Console.WriteLine("Servis pokrenut na: net.tcp://localhost:4000/SolarService");
                Console.WriteLine("Pritisni bilo koji taster za zatvaranje servisa...");
                Console.ReadKey();
            }

            Console.WriteLine("Servis zatvoren.");
        }
    }
}
