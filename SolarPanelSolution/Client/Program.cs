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
            Console.WriteLine("Klijent započeo rad...\n");

            using (ChannelFactory<ISolarService> factory = new ChannelFactory<ISolarService>("SolarServiceEndpoint"))
            {
                ISolarService proxy = factory.CreateChannel();
                try
                {
                    var meta = new PvMeta("session1", 5, "1.0", 20);
                    proxy.StartSession(meta);

                    string csvPath = "Data/FPV_Altamonte_FL_data.csv";
                    var samples = CsvSampleReader.ReadSamples(csvPath, meta.RowLimitN);

                    int sentCount = 0;
                    foreach (var sample in samples)
                    {
                        sentCount++;
                        Console.WriteLine($"Šaljem red {sentCount}/{meta.RowLimitN}...");
                        proxy.PushSample(sample);
                    }

                    proxy.EndSession();
                    ((IClientChannel)proxy).Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška u prenosu: " + ex.Message);
                    ((IClientChannel)proxy)?.Abort();
                }
            }

            Console.WriteLine("\nKlijent je završio rad.");
        }
    }
}
