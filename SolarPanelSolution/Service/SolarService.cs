using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private double? lastDcVolt = null;

        public delegate void TransferStartedHandler(object sender, EventArgs e);
        public delegate void SampleReceivedHandler(object sender, SampleEventArgs e);
        public delegate void TransferCompletedHandler(object sender, EventArgs e);
        public delegate void WarningRaisedHandler(object sender, WarningEventArgs e);

        public event TransferStartedHandler OnTransferStarted;
        public event SampleReceivedHandler OnSampleReceived;
        public event TransferCompletedHandler OnTransferCompleted;
        public event WarningRaisedHandler OnWarningRaised;

        private double OverTempThreshold =>
            double.Parse(ConfigurationManager.AppSettings["OverTempThreshold"] ?? "70");

        private double PowerSpikeThreshold =>
            double.Parse(ConfigurationManager.AppSettings["PowerSpikeThreshold"] ?? "500");

        private double DcSagThreshold =>
            double.Parse(ConfigurationManager.AppSettings["DcSagThreshold"] ?? "50");

        private double EfficiencyThreshold =>
            double.Parse(ConfigurationManager.AppSettings["EfficiencyThreshold"] ?? "0.5");

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

            Console.WriteLine($"\nSession started: {meta} | Folder: {dateDir}");
            OnTransferStarted?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public bool PushSample(PvSample sample)
        {
            receivedCount++;

            double percent = (currentMeta != null && currentMeta.RowLimitN > 0) ? (100.0 * receivedCount / currentMeta.RowLimitN) : 0;

            Console.WriteLine($"Primljen red {receivedCount} / {currentMeta.RowLimitN} ({percent:F1}%)");

            var validation = SampleValidator.Validate(sample);

            if (!validation.IsValid)
            {
                rejectWriter?.WriteRejected(sample, validation.ErrorMessage);
                OnWarningRaised?.Invoke(this, new WarningEventArgs(validation.ErrorMessage));
                return false;
            }

            sessionWriter?.WriteSample(sample);
            OnSampleReceived?.Invoke(this, new SampleEventArgs(sample));


            if (sample.Temper > OverTempThreshold)
                OnWarningRaised?.Invoke(this,
                    new WarningEventArgs($"OverTempWarning: {sample.Temper}°C > {OverTempThreshold}°C"));

            if (sample.AcPwrt > PowerSpikeThreshold)
                OnWarningRaised?.Invoke(this,
                    new WarningEventArgs($"PowerSpikeWarning: AC Power {sample.AcPwrt}W > {PowerSpikeThreshold}W"));

            // --- TASK 9: Analitika 1 (DC napon i prekidi) ---
            if (Math.Abs(sample.DcVolt - 32767.0) < 0.0001)
            {
                OnWarningRaised?.Invoke(this,
                    new WarningEventArgs("DcSentinelWarning: DCVolt = 32767 (nema validnih podataka)"));
            }

            if (sample.DcVolt == 0 && sample.AcPwrt > 0)
            {
                OnWarningRaised?.Invoke(this, new WarningEventArgs("DcFaultWarning: DCVolt == 0 dok AC Power > 0"));
            }

            if (lastDcVolt.HasValue)
            {
                double delta = Math.Abs(sample.DcVolt - lastDcVolt.Value);
                if (delta > DcSagThreshold)
                {
                    OnWarningRaised?.Invoke(this, new WarningEventArgs($"DCSagWarning: Nagla promena DC napona! DCVolt={delta:F2}V > {DcSagThreshold}V"));
                }
            }

            lastDcVolt = sample.DcVolt;


            // --- TASK 10: Analitika 2 (Efikasnost) ---
            double expectedPower = sample.AcVlt1 * sample.AcCur1;
            if (expectedPower > 0)
            {
                double efficiencyRatio = sample.AcPwrt / expectedPower;

                if (efficiencyRatio < EfficiencyThreshold)
                {
                    OnWarningRaised?.Invoke(this, new WarningEventArgs($"LowEfficiencyWarning: Efikasnost {efficiencyRatio:P1} < prag {EfficiencyThreshold:P0}"));
                }
            }

            return true;
        }

        public bool EndSession()
        {
            Console.WriteLine($"\nPrenos zavrsen. Ukupno primljeno {receivedCount} redova.\n");

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
