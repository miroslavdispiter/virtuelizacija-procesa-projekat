using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceEventListener
    {
        public void OnTransferStarted(object sender, EventArgs e)
        {
            Console.WriteLine("EVENT: Transfer started!");
        }

        public void OnSampleReceived(object sender, SampleEventArgs e)
        {
            Console.WriteLine($"EVENT: Sample received | RowIndex={e.Sample.RowIndex}, AC Power={e.Sample.AcPwrt}");
        }

        public void OnTransferCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("EVENT: Transfer completed successfully!");
        }

        public void OnWarningRaised(object sender, WarningEventArgs e)
        {
            Console.WriteLine($"WARNING: {e.Warning}");
        }
    }
}
