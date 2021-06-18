using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport
{
    public static class ExcelAddressFunctions
    {
        public static string ChangeAbsolute(string address, bool absoluteColumn, bool absoluteRow)
        {
            if (absoluteRow || absoluteColumn)
            {
                int x = 666;
            }
            var r = new string(address.Where(x => x != '$').ToArray());
            if (absoluteRow)
            {
                var firstNumberIndex = 0;
                while (char.IsLetter(r[firstNumberIndex]))
                {
                    firstNumberIndex++;
                }
                r = $"{r.Substring(0, firstNumberIndex)}${r.Substring(firstNumberIndex)}";

            }
            if (absoluteColumn)
            {
                r = $"${r}";
            }
            return r;
        }
    }
}
