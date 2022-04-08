using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CloneSimulationDTO : BaseDTO
    {
        public Guid scenarioId { get; set; }
        public Guid networkId { get; set; }
        public string scenarioName { get; set; }

    }
}
