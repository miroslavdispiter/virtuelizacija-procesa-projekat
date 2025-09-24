using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SampleEventArgs : EventArgs
    {
        public PvSample Sample { get; }

        public SampleEventArgs(PvSample sample)
        {
            Sample = sample;
        }
    }

    public class WarningEventArgs : EventArgs
    {
        public string Warning { get; }

        public WarningEventArgs(string warning)
        {
            Warning = warning;
        }
    }
}
