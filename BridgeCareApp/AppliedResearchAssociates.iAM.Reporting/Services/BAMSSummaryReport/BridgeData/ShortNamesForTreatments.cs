using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData
{
    public static class ShortNamesForTreatments
    {
        internal static Dictionary<string, string> GetShortNamesForTreatments()
        {
            var shortNames = new Dictionary<string, string>
            {
                {"Culvert Rehab (Other)", "Culv_Rehab_Other" },
                {"Culvert Replacement (Box/Frame/Arch)", "Culv_Rep_Box" },
                {"Culvert Replacement (Other)", "Culv_Rep_Other" },
                {"Culvert Replacement (Pipe)", "Culv_Rep_Pipe" },
                {"County Maintenance - Deck Work", "CM_Deck" },
                {"County Maintenance - Superstructure Work", "CM_Super" },
                {"County Maintenance - Substructure Work", "CM_Sub" },
                {"Painting (Joint/Spot/Zone)", "Spot_Paint" },
                {"Painting (Full)", "Full_Paint" },
                {"Structural Overlay/Joints/Coatings", "Struct_Overlay" },
                {"Epoxy/Joint Glands/Coatings", "Epx" },
                {"Deck Replacement", "Deck_Replc" },
                {"Substructure Rehab", "Sub_Rehab" },
                {"Superstructure Rep/Rehab", "Sup_Rpl" },
                {"Bridge Replacement", "Brdg_Repl" },
                {"Preservation","MPMS_Pres" },
                {"Emergency Repair", "MPMS_EP" },
                {"Rehabilitation", "MPMS_Rehab" },
                {"Removal", "MPMS_Rem" },
                {"Repair", "MPMS_Repair" },
                {"Replacement", "MPMS_Repl" },
                {"Bituminous Overlay", "Bit_Over" },
                {"Wishful Preservation/ Light Rehab", "Wish_Pres_Rehab" },
                {"Wishful Replacement", "Wish_Repl" }
            };
            return shortNames;
        }
    }
}
