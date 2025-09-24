using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class PvMeta
    {
        public PvMeta() { }

        public PvMeta(string fileName, int totalRows, string schemaVersion, int rowLimitN)
        {
            FileName = fileName;
            TotalRows = totalRows;
            SchemaVersion = schemaVersion;
            RowLimitN = rowLimitN;
        }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public int TotalRows { get; set; }

        [DataMember]
        public string SchemaVersion { get; set; }

        [DataMember]
        public int RowLimitN { get; set; }

        public override string ToString()
        {
            return $"File: {FileName}, Rows: {TotalRows}, Schema: {SchemaVersion}, Limit: {RowLimitN}";
        }
    }
}
