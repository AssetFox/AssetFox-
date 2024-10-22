using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SimulationOutputJsonDTO: BaseDTO
    {
        public Guid SimulationId { get; set; }

        public Guid? SimulationOutputId { get; set; }

        public string Output { get; set; }

        public SimulationOutputEnum OutputType { get; set; }
    }    
}
