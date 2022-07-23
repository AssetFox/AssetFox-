using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models.Validation;

namespace BridgeCareCore.Models
{
    public class GetValidTreatmentConsequenceParameters
    {
        public List<TreatmentConsequenceDTO> Consequences { get; set; }
        public ValidationParameter ValidationParameters { get; set; }
    }
}
