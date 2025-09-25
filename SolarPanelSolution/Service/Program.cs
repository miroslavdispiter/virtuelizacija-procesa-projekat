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
            SolarService service = new SolarService();
            ServiceEventListener listener = new ServiceEventListener();

            service.OnTransferStarted += listener.OnTransferStarted;
            service.OnSampleReceived += listener.OnSampleReceived;
            service.OnTransferCompleted += listener.OnTransferCompleted;
            service.OnWarningRaised += listener.OnWarningRaised;

            using (ServiceHost host = new ServiceHost(service))
            {
                host.Open();
                Console.WriteLine("Servis pokrenut na: net.tcp://localhost:4000/SolarService");
                Console.WriteLine("Pritisni bilo koji taster za zatvaranje servisa...");
                Console.ReadKey();
                host.Close();
            }

            Console.WriteLine("Servis zatvoren.");
            Console.ReadKey();
        }
    }
}
