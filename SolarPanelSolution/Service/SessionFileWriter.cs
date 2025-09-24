using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SessionFileWriter : IDisposable
    {
        private StreamWriter writer;
        private bool disposed = false;
        private readonly string filePath;

        public string FilePath => filePath;

        public SessionFileWriter(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None));
        }

        public void WriteHeader(PvMeta meta)
        {
            writer.WriteLine($"FILE: {meta.FileName},ROWS: {meta.TotalRows},SCHEMA: {meta.SchemaVersion},ROWLIMIT: {meta.RowLimitN}");
            writer.WriteLine("DAY,HOUR,ACPWRT,DCVOLT,TEMPER,VLT1to2,VLT2to3,VLT3to1,ACCUR1,ACVLT1,ROWINDEX");
        }

        public void WriteSample(PvSample sample)
        {
            writer.WriteLine($"{sample.Day},{sample.Hour},{sample.AcPwrt},{sample.DcVolt},{sample.Temper},{sample.Vlt1to2},{sample.Vlt2to3},{sample.Vlt3to1},{sample.AcCur1},{sample.AcVlt1},{sample.RowIndex}");
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
                    Console.WriteLine("SessionFileWriter disposed!");
                }
                disposed = true;
            }
        }

        ~SessionFileWriter()
        {
            Dispose(false);
        }
    }
}
