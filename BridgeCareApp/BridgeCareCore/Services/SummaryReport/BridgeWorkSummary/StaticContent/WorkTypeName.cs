using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public static class WorkTypeNamesExtensions
    {
        public static string ToSpreadsheetString(this TreatmentCategory name) => name switch
        {
            TreatmentCategory.Preservation => "Preservation",
            TreatmentCategory.CapacityAdding => "Capacity Adding",
            TreatmentCategory.Rehabilitation => "Rehabilitation",
            TreatmentCategory.Replacement => "Replacement",
            TreatmentCategory.Maintenance => "Maintenance",
            TreatmentCategory.Other => "Other",
            _ => name.ToString(),
        };
    }
}
