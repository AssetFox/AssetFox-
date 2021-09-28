using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
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
                { "Other", TreatmentCategory.Other }
            };
    }
}
