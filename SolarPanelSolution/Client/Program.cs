using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Klijent započeo rad...");

            using (ChannelFactory<ISolarService> factory = new ChannelFactory<ISolarService>("SolarServiceEndpoint"))
            {
                ISolarService proxy = factory.CreateChannel();
                try
                {
                    var meta = new PvMeta("session1", 5, "1.0", 100);
                    proxy.StartSession(meta);

                    proxy.EndSession();
                    ((IClientChannel)proxy).Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška u komunikaciji: " + ex.Message);
                    ((IClientChannel)proxy)?.Abort();
                }
            }

            Console.WriteLine("Klijent je završio rad.");
        }
    }
}
