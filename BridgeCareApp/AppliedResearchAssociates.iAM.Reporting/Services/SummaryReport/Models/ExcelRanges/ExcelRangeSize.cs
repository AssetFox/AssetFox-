using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public class ExcelRangeSize
    {
        public ExcelRangeSize(int width = 1, int height = 1)
        {
            Width = width;
            Height = height;
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public override string ToString() => $"{nameof(ExcelRangeSize)} {Width}x{Height}";
    }
    public static class ExcelRangeSizes
    {
        public static ExcelRangeSize Default = new ExcelRangeSize(1, 1);
    }
}
