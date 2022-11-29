using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Models
{
    public class CommittedProjectFillTreatmentReturnValuesModel
    {
        public List<CommittedProjectConsequenceDTO> ValidTreatmentConsequences { get; set; }
        public double TreatmentCost { get; set; }
        public TreatmentCategory TreatmentCategory { get; set; }
    }
}
