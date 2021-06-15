using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent
{
    public enum BPNName
    {
        BPN1,
        BPN2,
        BPN3,
        BPN4,
        BPNL,
        BPNOther,
        BPNTotal
    }
    public static class BPNNamesExtensions
    {
        public static string ToSpreadsheetString(this BPNName name) => name switch
        {
            BPNName.BPN1 => "BPN1",
            BPNName.BPN2 => "BPN2",
            BPNName.BPN3 => "BPN3",
            BPNName.BPN4 => "BPN4",
            BPNName.BPNL => "BPNL",
            BPNName.BPNOther => "BPN Other (BPN D, BPN H,BPN N, BPN T)",
            BPNName.BPNTotal => "Total BPN Cost",
            _ => name.ToString(),
        };
    }
}
