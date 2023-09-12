using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentPerformanceFactorLoadResult
    {
        public List<TreatmentPerformanceFactorDTO> PerformanceFactors { get; set; }
        public List<string> ValidationMessages { get; set; }
    }
}

