using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.BridgeWorkSummary
{
    public static class WorkTypeMap
    {
        public static Dictionary<string, WorkTypeName> Map =
            new Dictionary<string, WorkTypeName>
            {
                { "County Maintenance - Deck Work", WorkTypeName.Preservation },
                { "County Maintenance - Superstructure Work", WorkTypeName.Preservation },
                { "County Maintenance - Substructure Work", WorkTypeName.Preservation },
                { "Bituminous Overlay", WorkTypeName.Preservation },
                { "Structural Overlay/Joints/Coatings", WorkTypeName.Preservation },
                { "Epoxy/Joint Glands/Coatings", WorkTypeName.Preservation },
                { "Painting (Joint/Spot/Zone)", WorkTypeName.Preservation },
                { "Painting (Full)", WorkTypeName.Preservation },
                { "Deck Replacement", WorkTypeName.Rehab },
                { "Substructure Rehab", WorkTypeName.Rehab },
                { "Superstructure Rep/Rehab", WorkTypeName.Rehab },
                { "Bridge Replacement", WorkTypeName.Replacement},
            };
    }
}
