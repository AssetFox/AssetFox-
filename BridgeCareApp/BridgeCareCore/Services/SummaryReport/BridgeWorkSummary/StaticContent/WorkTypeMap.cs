using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AppliedResearchAssociates.iAM.Domains.SelectableTreatment;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public static class WorkTypeMap
    {
        public static Dictionary<string, TreatmentCategory> Map =
            new Dictionary<string, TreatmentCategory>
            {
                // Non-culverts
                { "County Maintenance - Deck Work", TreatmentCategory.Preservation },
                { "County Maintenance - Superstructure Work", TreatmentCategory.Preservation },
                { "County Maintenance - Substructure Work", TreatmentCategory.Preservation },
                { "Bituminous Overlay", TreatmentCategory.Preservation },
                { "Structural Overlay/Joints/Coatings", TreatmentCategory.Preservation },
                { "Epoxy/Joint Glands/Coatings", TreatmentCategory.Preservation },
                { "Painting (Joint/Spot/Zone)", TreatmentCategory.Preservation },
                { "Painting (Full)", TreatmentCategory.Preservation },
                { "Deck Replacement", TreatmentCategory.Rehabilitation },
                { "Substructure Rehab", TreatmentCategory.Rehabilitation },
                { "Superstructure Rep/Rehab", TreatmentCategory.Rehabilitation },
                { "Bridge Replacement", TreatmentCategory.Replacement},
                // end of non-culverts

                // culverts
                {"Culvert Rehab (Other)", TreatmentCategory.Rehabilitation },
                {"Culvert Replacement (Box/Frame/Arch)", TreatmentCategory.Replacement },
                { "Culvert Replacement (Other)", TreatmentCategory.Replacement},
                { "Culvert Replacement (Pipe)", TreatmentCategory.Replacement },

                // MPMS
                {"Preservation", TreatmentCategory.Preservation },
                { "Capacity Adding", TreatmentCategory.CapacityAdding },
                {"Rehabilitation", TreatmentCategory.Rehabilitation },
                { "Replacement", TreatmentCategory.Replacement },
                { "Maintenance", TreatmentCategory.Maintenance }
            };
    }
}
