
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.StaticContent
{
    public static class WorkTypeMap
    {
        public static Dictionary<string, TreatmentCategory> Map =
            new()
            {
                // MPMS
                { "Preservation", TreatmentCategory.Preservation },
                { "Capacity Adding", TreatmentCategory.CapacityAdding },
                { "Rehabilitation", TreatmentCategory.Rehabilitation },
                { "Replacement", TreatmentCategory.Reconstruction },
                { "Reconstruction", TreatmentCategory.Reconstruction },
                { "Maintenance", TreatmentCategory.Maintenance },
                { "Other", TreatmentCategory.Other },
            };
    }
}
