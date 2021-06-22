using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary.StaticContent
{
    public enum BPNCostBudgetName
    {
        BPN1,
        BPN2,
        BPN3,
        BPN4,
        BPNL,
        BPNOther
    }

    public enum BPNName
    {
        BPN1 = 0,
        BPN2 = 1,
        BPN3 = 2,
        BPN4 = 3,
        BPNL = 4,
        BPND = 5,
        BPNH = 6,
        BPNN = 7,
        BPNT = 8
    }

    public static class BPNNamesExtensions
    {
        public static string ToSpreadsheetString(this BPNCostBudgetName name) => name switch
        {
            BPNCostBudgetName.BPN1 => "BPN1",
            BPNCostBudgetName.BPN2 => "BPN2",
            BPNCostBudgetName.BPN3 => "BPN3",
            BPNCostBudgetName.BPN4 => "BPN4",
            BPNCostBudgetName.BPNL => "BPNL",
            BPNCostBudgetName.BPNOther => "BPN Other (BPN D, BPN H,BPN N, BPN T)",
            _ => name.ToString(),
        };

        public static string ToMatchInDictionaryString(this BPNCostBudgetName name) => name switch
        {
            BPNCostBudgetName.BPN1 => "1",
            BPNCostBudgetName.BPN2 => "2",
            BPNCostBudgetName.BPN3 => "3",
            BPNCostBudgetName.BPN4 => "4",
            BPNCostBudgetName.BPNL => "L",
            BPNCostBudgetName.BPNOther => "BPN Other (BPN D, BPN H,BPN N, BPN T)",
            _ => name.ToString(),
        };

        static readonly List<string> bpnOtherNames = new List<string> { "D", "H", "N", "T" };
        public static bool IsBpnOther(this string bpnOtherTest) =>
            bpnOtherNames.Contains(bpnOtherTest);

        static readonly List<string> bpnAllNames = new List<string> { "1", "2", "3", "4", "L", "D", "H", "N", "T" };

        public static string ToReportLabel(this BPNName value, string prefix = "") => $"{prefix}BPN {bpnAllNames[(int) value]}";
        public static string ToMatchInDictionary(this BPNName value) => bpnAllNames[(int) value];
    };


};

