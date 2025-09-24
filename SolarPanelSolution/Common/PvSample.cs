using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class PvSample
    {
        public PvSample() { }

        public PvSample(int day, int hour, double acPwrt, double dcVolt, double temper, 
            double vlt1to2, double vlt2to3, double vlt3to1, 
            double acCur1, double acVlt1, int rowIndex)
        {
            Day = day;
            Hour = hour;
            AcPwrt = acPwrt;
            DcVolt = dcVolt;
            Temper = temper;
            Vlt1to2 = vlt1to2;
            Vlt2to3 = vlt2to3;
            Vlt3to1 = vlt3to1;
            AcCur1 = acCur1;
            AcVlt1 = acVlt1;
            RowIndex = rowIndex;
        }

        [DataMember]
        public int Day { get; set; }

        [DataMember]
        public int Hour { get; set; }

        [DataMember]
        public double AcPwrt { get; set; }

        [DataMember]
        public double DcVolt { get; set; }

        [DataMember]
        public double Temper { get; set; }

        [DataMember]
        public double Vlt1to2 { get; set; }

        [DataMember]
        public double Vlt2to3 { get; set; }

        [DataMember]
        public double Vlt3to1 { get; set; }

        [DataMember]
        public double AcCur1 { get; set; }

        [DataMember]
        public double AcVlt1 { get; set; }

        [DataMember]
        public int RowIndex { get; set; }

        public override string ToString()
        {
            return $"Day: {Day}, Hour: {Hour}, AC Power: {AcPwrt}, DcVolt: {DcVolt}, Temp: {Temper}, " +
                   $"V12: {Vlt1to2}, V23: {Vlt2to3}, V31: {Vlt3to1}, Iac1: {AcCur1}, Vac1: {AcVlt1}, RowIndex: {RowIndex}";
        }
    }
}
