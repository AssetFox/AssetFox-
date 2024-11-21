
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent
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
        //BPNL = 4, // Constants from Bridge version; TODO: Consider whether to remove or keep for future use
        //BPND = 5,
        //BPNH = 6,
        //BPNN = 7,
        //BPNT = 8
        Statewide = 100
    }

    public static class BPNNamesExtensions
    {
        static readonly List<string> bpnAllNames = new List<string> { "1", "2", "3", "4", "L", "D", "H", "N", "T" };

        public static string ToMatchInDictionary(this BPNName value) => value != BPNName.Statewide ? bpnAllNames[(int) value] : "";
    };


};

