using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationOutputJsonDTO: BaseDTO
    {
        public string Output { get; set; }

        public SimulationOutputEnum OutputType { get; set; }
    }    
}
