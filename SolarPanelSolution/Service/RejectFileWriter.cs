using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RejectFileWriter : IDisposable
    {
        private StreamWriter writer;
        private bool disposed = false;
        private readonly string filePath;

        public string FilePath => filePath;

        public RejectFileWriter(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None));
            writer.WriteLine("REJECTED_ROW,REASON");
        }

        public void WriteRejected(PvSample sample, string reason)
        {
            int rowIndex = sample?.RowIndex ?? -1;
            writer.WriteLine($"{rowIndex},{reason}");
        }

        public void Flush()
        {
            writer?.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    writer?.Flush();
                    writer?.Dispose();
                    Console.WriteLine("RejectFileWriter disposed!");
                }
                disposed = true;
            }
        }

        ~RejectFileWriter()
        {
            Dispose(false);
        }
    }
}
