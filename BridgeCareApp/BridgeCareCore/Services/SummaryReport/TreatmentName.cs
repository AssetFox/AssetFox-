using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SummaryReport
{
    public enum MPMSTreatmentName
    {
        Other,
        Preservation,
        EmergencyRepair,
        Rehabilitation,
        Repair,
        Removal,
        Replacement
    }
    public enum CulvertTreatmentName
    {
        Other,
        CulvertRehabOther,
        CulvertReplacementBoxFrameArch,
        CulvertReplacementOther,
        CulvertReplacementPipe
    }
    public enum BAMSTreatmentName
    {
        Other,
        CountyMaintenanceDeckWork,
        CountyMaintenanceSuperstructureWork,
        CountyMaintenanceSubstructureWork,
        BituminousOverlay,
        StructuralOverlayJointsCoatings,
        EpoxyJointGlandsCoatings,
        PaintingJointSpotZone,
        PaintingFull,
        DeckReplacement,
        SubstructureRehab,
        SuperstructureRepRehab,
        BridgeReplacement
    }
    public static class MPMSTreatmentMap
    {
        public static Dictionary<string, MPMSTreatmentName> Map =
            new Dictionary<string, MPMSTreatmentName>
            {
                { "Preservation", MPMSTreatmentName.Preservation },
                { "Emergency Repair", MPMSTreatmentName.EmergencyRepair },
                { "Rehabilitation", MPMSTreatmentName.Rehabilitation },
                { "Repair", MPMSTreatmentName.Repair },
                { "Removal", MPMSTreatmentName.Removal },
                { "Replacement", MPMSTreatmentName.Replacement }
            };
    }
    public static class CulvertTreatmentMap
    {
        public static Dictionary<string, CulvertTreatmentName> Map =
            new Dictionary<string, CulvertTreatmentName>
            {
                { "Culvert Rehab (Other)", CulvertTreatmentName.CulvertRehabOther },
                { "Culvert Replacement (Box/Frame/Arch)", CulvertTreatmentName.CulvertReplacementBoxFrameArch },
                { "Culvert Replacement (Other)", CulvertTreatmentName.CulvertReplacementOther },
                { "Culvert Replacement (Pipe)", CulvertTreatmentName.CulvertReplacementPipe }
            };
    }
    public static class BamsTreatmentMap
    {
        public static Dictionary<string, BAMSTreatmentName> Map =
            new Dictionary<string, BAMSTreatmentName>
            {
                { "County Maintenance - Deck Work", BAMSTreatmentName.CountyMaintenanceDeckWork },
                { "County Maintenance - Superstructure Work", BAMSTreatmentName.CountyMaintenanceSuperstructureWork },
                { "County Maintenance - Substructure Work", BAMSTreatmentName.CountyMaintenanceSubstructureWork },
                { "Bituminous Overlay", BAMSTreatmentName.BituminousOverlay },
                { "Structural Overlay/Joints/Coatings", BAMSTreatmentName.StructuralOverlayJointsCoatings },
                { "Epoxy/Joint Glands/Coatings", BAMSTreatmentName.EpoxyJointGlandsCoatings },
                { "Painting (Joint/Spot/Zone)", BAMSTreatmentName.PaintingJointSpotZone },
                { "Painting (Full)", BAMSTreatmentName.PaintingFull },
                { "Deck Replacement", BAMSTreatmentName.DeckReplacement },
                { "Substructure Rehab", BAMSTreatmentName.SubstructureRehab },
                { "Superstructure Rep/Rehab", BAMSTreatmentName.SuperstructureRepRehab },
                { "Bridge Replacement", BAMSTreatmentName.BridgeReplacement }
            };
    }
}
