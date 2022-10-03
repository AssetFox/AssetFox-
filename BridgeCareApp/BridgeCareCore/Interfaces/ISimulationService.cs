using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationService
    {
        PagingPageModel<SimulationDTO> GetUserScenarioPage(PagingRequestModel<SimulationDTO> request);
        public PagingPageModel<SimulationDTO> GetSharedScenarioPage(PagingRequestModel<SimulationDTO> request, string role);
        PagingPageModel<QueuedSimulationDTO> RetrieveSimulationQueue(PagingRequestModel<QueuedSimulationDTO> request, string role);
    }
}
