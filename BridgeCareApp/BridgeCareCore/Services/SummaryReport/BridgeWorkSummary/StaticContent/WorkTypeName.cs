using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public enum WorkTypeName
    {
        Preservation,
        EmergencyRepair,
        Rehab,
        Replacement,
        Other,
    }

    public static class WorkTypeNamesExtensions
    {
        public static string ToSpreadsheetString(this WorkTypeName name) => name switch
        {
            WorkTypeName.Preservation => "Preservation",
            WorkTypeName.EmergencyRepair => "Emergency Repair",
            WorkTypeName.Rehab => "Rehab",
            WorkTypeName.Replacement => "Replacement",
            _ => name.ToString(),
        };
    }
}
