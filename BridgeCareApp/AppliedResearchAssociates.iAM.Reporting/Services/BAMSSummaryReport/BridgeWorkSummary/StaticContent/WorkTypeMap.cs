
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeWorkSummary
{
    public static class WorkTypeMap
    {
        public static Dictionary<string, TreatmentCategory> Map =
            new Dictionary<string, TreatmentCategory>
            {
                // MPMS
                {"Preservation", TreatmentCategory.Preservation },
                { "Capacity Adding", TreatmentCategory.CapacityAdding },
                {"Rehabilitation", TreatmentCategory.Rehabilitation },
                { "Replacement", TreatmentCategory.Replacement },
                { "Maintenance", TreatmentCategory.Maintenance },
                { "Other", TreatmentCategory.Other },
                { "Work Outside Scope", TreatmentCategory.WorkOutsideScope }
            };
    }
}
