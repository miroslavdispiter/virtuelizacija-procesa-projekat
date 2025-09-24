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

        public delegate void TransferStartedHandler(object sender, EventArgs e);
        public delegate void SampleReceivedHandler(object sender, SampleEventArgs e);
        public delegate void TransferCompletedHandler(object sender, EventArgs e);
        public delegate void WarningRaisedHandler(object sender, WarningEventArgs e);

        public event TransferStartedHandler OnTransferStarted;
        public event SampleReceivedHandler OnSampleReceived;
        public event TransferCompletedHandler OnTransferCompleted;
        public event WarningRaisedHandler OnWarningRaised;

        public bool StartSession(PvMeta meta)
        {
            if (meta == null) return false;

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

            Console.WriteLine($"\nSession started: {meta} | Folder: {dateDir}");

            OnTransferStarted?.Invoke(this, EventArgs.Empty);
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
                OnWarningRaised?.Invoke(this, new WarningEventArgs(validation.ErrorMessage));
                return false;
            }

            sessionWriter?.WriteSample(sample);

            OnSampleReceived?.Invoke(this, new SampleEventArgs(sample));
            return true;
        }

        public bool EndSession()
        {
            Console.WriteLine($"\nPrenos završen. Ukupno primljeno {receivedCount} redova.\n");

            sessionWriter?.Flush();
            sessionWriter?.Dispose();
            sessionWriter = null;

            rejectWriter?.Flush();
            rejectWriter?.Dispose();
            rejectWriter = null;

            currentMeta = null;

            OnTransferCompleted?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
