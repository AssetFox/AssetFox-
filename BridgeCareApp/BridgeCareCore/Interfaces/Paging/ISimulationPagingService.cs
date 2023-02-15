using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationPagingService
    {
        PagingPageModel<SimulationDTO> GetUserScenarioPage(PagingRequestModel<SimulationDTO> request);

        public PagingPageModel<SimulationDTO> GetSharedScenarioPage(PagingRequestModel<SimulationDTO> request, bool hasAdminAccess, bool hasSimulationAccess);
    }
}
