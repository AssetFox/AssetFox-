using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentPerformanceFactorDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public float PerformanceFactor { get; set; }
    }
}
