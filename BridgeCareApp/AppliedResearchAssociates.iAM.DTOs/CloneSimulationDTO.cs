using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CloneSimulationDTO : BaseDTO
    {
        public Guid ScenarioId { get; set; }
        public Guid NetworkId { get; set; }
        public Guid DestinationNetworkId { get; set; }
        public string ScenarioName { get; set; }

    }
}
