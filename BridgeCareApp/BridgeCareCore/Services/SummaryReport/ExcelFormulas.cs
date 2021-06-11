using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport
{
    public static class ExcelFormulas
    {
        public static string Sum(params string[] addresses)
        {
            var builder = new StringBuilder();
            builder.Append("Sum(");
            var first = true;
            foreach (var address in addresses)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(address);
            }
            builder.Append(")");
            var r = builder.ToString();
            return r;
        }
        public static string Sum(params ExcelRange[] ranges)
        {
            var addresses = ranges.Select(r => r.Address).ToArray();
            return Sum(addresses);
        }

        public static string Sum(IEnumerable<ExcelRange> ranges)
        {
            var addresses = ranges.Select(r => r.Address).ToArray();
            return Sum(addresses);
        }
    }
}
