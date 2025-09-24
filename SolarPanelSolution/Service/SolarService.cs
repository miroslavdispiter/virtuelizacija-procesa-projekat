using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SolarService : ISolarService
    {
        private PvMeta currentMeta;
        private SessionFileWriter sessionWriter;
        private RejectFileWriter rejectWriter;
        private int receivedCount;

        public bool StartSession(PvMeta meta)
        {
            if (meta == null)
                return false;

            currentMeta = meta;
            receivedCount = 0;

            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string plantDir = Path.Combine(baseDir, meta.FileName);
            string dateDir = Path.Combine(plantDir, DateTime.Now.ToString("yyyy-MM-dd"));

            if (!Directory.Exists(dateDir))
                Directory.CreateDirectory(dateDir);

            string sessionPath = Path.Combine(dateDir, "session.csv");
            string rejectsPath = Path.Combine(dateDir, "rejects.csv");

            sessionWriter = new SessionFileWriter(sessionPath);
            sessionWriter.WriteHeader(meta);

            rejectWriter = new RejectFileWriter(rejectsPath);

            Console.WriteLine($"\nSesija startovana: {meta}\nFajlovi: {sessionPath}, {rejectsPath}\n");
            return true;
        }

        public bool PushSample(PvSample sample)
        {
            receivedCount++;
            Console.WriteLine($"Primljen red {receivedCount} / {currentMeta.RowLimitN}");

            var validation = SampleValidator.Validate(sample);

            if (!validation.IsValid)
            {
                rejectWriter?.WriteRejected(sample, validation.ErrorMessage);
                Console.WriteLine($"Red odbijen (RowIndex={sample.RowIndex}): {validation.ErrorMessage}");
                return false;
            }

            sessionWriter?.WriteSample(sample);
            Console.WriteLine($"Red snimljen (RowIndex={sample.RowIndex})");

            return true;
        }

        public bool EndSession()
        {
            Console.WriteLine($"\nSesija zavrsena. Ukupno primljeno redova: {receivedCount}\n");

            sessionWriter?.Flush();
            sessionWriter?.Dispose();
            sessionWriter = null;

            rejectWriter?.Flush();
            rejectWriter?.Dispose();
            rejectWriter = null;

            currentMeta = null;
            return true;
        }
    }
}
