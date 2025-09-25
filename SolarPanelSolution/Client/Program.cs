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
            Console.WriteLine("Klijent zapoceo sa radom!\n");

            using (ChannelFactory<ISolarService> factory = new ChannelFactory<ISolarService>("SolarServiceEndpoint"))
            {
                ISolarService proxy = factory.CreateChannel();

                PvMeta meta = new PvMeta("session1", 5, "1.0", 250);
                proxy.StartSession(meta);

                string csvPath = "Data/FPV_Altamonte_FL_data.csv";
                var samples = CsvSampleReader.ReadSamples(csvPath, meta.RowLimitN);

                int sentCount = 0;
                Console.WriteLine("\n");

                foreach (var sample in samples)
                {
                    sentCount++;

                    // TASK 4: Simulacija prekida
                    /*if (sentCount == 10)
                    {
                        Console.WriteLine("Simulacija prekida! Gasim klijenta...");
                        break; 
                    }*/
                    Console.WriteLine($"Saljem red {sentCount} / {meta.RowLimitN}...");
                    proxy.PushSample(sample);
                }
                Console.WriteLine("\nSvi redovi su uspesno poslati serveru.\n");

                // DEO ZA TASK 4
                /*for (int i = 1; i <= 3; i++)
                {
                    PvSample sample = new PvSample(day: i, hour: i, acPwrt: 100 + i, dcVolt: 600 + i, 
                                                    temper: 25 + i, vlt1to2: 220, vlt2to3: 220, 
                                                    vlt3to1: 220, acCur1: 10, acVlt1: 230, rowIndex: i);
                    proxy.PushSample(sample);
                }*/

                proxy.EndSession();
                ((IClientChannel)proxy).Dispose();

            }
            Console.WriteLine("Klijent je zavrsio sa radom.");
            Console.ReadKey();
        }
    }
}
