using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public static class ExcelRangeFunctions
    {
        /// <summary>
        /// Returns the left neighbor of the start of the range.
        /// For example, Left(B3:C4) = A3.
        /// </summary>
        public static Func<ExcelRange, string> Left =>
            range =>
                {
                    var start = range.Start;
                    var r = ExcelCellAddressFunctions.Left(start).Address;
                    return r;
                };
        /// <summary>Always returns the text, regardless of the range</summary>
        public static Func<ExcelRange, string> Constant(string text)
            => range => text;
        /// <summary>Always returns the empty string</summary> 
        public static Func<ExcelRange, string> Empty
            => Constant("");
        public static Func<ExcelRange, string> Concat(params Func<ExcelRange, string>[] functions)
            => range =>
                {
                    var builder = new StringBuilder();
                    foreach (var func in functions)
                    {
                        builder.Append(func(range));
                    }
                    var r = builder.ToString();
                    return r;
                };
        public static Func<ExcelRange, string> Plus(params Func<ExcelRange, string>[] summands)
        {
            if (summands.Any())
            {
                return range =>
                {
                    var builder = new StringBuilder();
                    builder.Append(summands[0](range));
                    for (int i=1; i<summands.Length; i++)
                    {
                        builder.Append("+");
                        builder.Append(summands[i](range));
                    }
                    var r = builder.ToString();
                    return r;
                };
            }
            return Empty;
        }
    }
}
