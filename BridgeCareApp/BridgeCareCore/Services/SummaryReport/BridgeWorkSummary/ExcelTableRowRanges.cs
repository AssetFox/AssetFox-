using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public class ExcelTableRowRanges
    {
        public Range HeaderRange { get; set; }
        public Range ContentRange { get; set; }
        public Range FooterRange { get; set; }
    }
}
