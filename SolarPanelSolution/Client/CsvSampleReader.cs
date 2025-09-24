using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class CsvSampleReader
    {
        public static List<PvSample> ReadSamples(string filePath, int limitN)
        {
            var samples = new List<PvSample>();

            using (var reader = new StreamReader(filePath))
            using (var rejected = new StreamWriter("Data/rejected_client.csv", append: false))
            {
                string headerLine = reader.ReadLine(); // Preskoči zaglavlje
                int rowCount = 0;

                while (!reader.EndOfStream && rowCount < limitN)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');

                    try
                    {
                        if (parts.Length < 15)
                        {
                            rejected.WriteLine($"Invalid column count: {line}");
                            continue;
                        }

                        int day = int.Parse(parts[1], CultureInfo.InvariantCulture);

                        string hourStr = parts[2];
                        if (!TimeSpan.TryParse(hourStr, CultureInfo.InvariantCulture, out TimeSpan ts))
                            throw new FormatException("Invalid hour format: " + hourStr);

                        int hour = ts.Hours;

                        double ParseDouble(string s)
                        {
                            if (string.IsNullOrWhiteSpace(s))
                                return double.NaN;

                            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                                throw new FormatException("Invalid double: " + s);

                            return Math.Abs(val - 32767.0) < 0.0001 ? double.NaN : val;
                        }

                        double acPwrt = ParseDouble(parts[3]);
                        double dcVolt = ParseDouble(parts[4]);
                        double temper = ParseDouble(parts[6]);
                        double v12 = ParseDouble(parts[7]);
                        double v23 = ParseDouble(parts[8]);
                        double v31 = ParseDouble(parts[9]);
                        double acCur1 = ParseDouble(parts[10]);
                        double acVlt1 = ParseDouble(parts[13]);

                        int rowIndex = rowCount + 1;

                        var sample = new PvSample(day, hour, acPwrt, dcVolt, temper,
                                                  v12, v23, v31, acCur1, acVlt1, rowIndex);

                        samples.Add(sample);
                        rowCount++;
                    }
                    catch (Exception ex)
                    {
                        rejected.WriteLine($"Rejected line: {line} | Reason: {ex.Message}");
                    }
                }
            }

            return samples;
        }
    }
}
