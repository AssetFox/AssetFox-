using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentLoadResult
    {
        public TreatmentDTO Treatment { get; set; }
        public List<string> ValidationMessages { get; set; }
    }
}
