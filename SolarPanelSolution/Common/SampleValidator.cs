using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class SampleValidator
    {
        private static int lastRowIndex = -1;

        public static ValidationResult Validate(PvSample s)
        {
            if (s == null)
                return ValidationResult.Fail("Sample is null");

            double? SafeVal(double val) =>
                (Math.Abs(val - 32767.0) < 0.0001) ? (double?)null : val;

            double? acPwrt = SafeVal(s.AcPwrt);
            double? dcVolt = SafeVal(s.DcVolt);
            double? temper = SafeVal(s.Temper);
            double? v12 = SafeVal(s.Vlt1to2);
            double? v23 = SafeVal(s.Vlt2to3);
            double? v31 = SafeVal(s.Vlt3to1);
            double? acCur1 = SafeVal(s.AcCur1);
            double? acVlt1 = SafeVal(s.AcVlt1);

            // Pravila validacije
            if (acPwrt.HasValue && acPwrt.Value < 0)
                return ValidationResult.Fail("AcPwrt < 0");

            if ((dcVolt.HasValue && dcVolt.Value <= 0) ||
                (v12.HasValue && v12.Value <= 0) ||
                (v23.HasValue && v23.Value <= 0) ||
                (v31.HasValue && v31.Value <= 0) ||
                (acCur1.HasValue && acCur1.Value <= 0) ||
                (acVlt1.HasValue && acVlt1.Value <= 0))
            {
                return ValidationResult.Fail("Napon/struja ima nevalidnu vrednost (<=0)");
            }

            if (s.RowIndex <= lastRowIndex)
                return ValidationResult.Fail($"RowIndex nije monoton: {s.RowIndex}");

            lastRowIndex = s.RowIndex;

            return ValidationResult.Ok();
        }
    }
}
