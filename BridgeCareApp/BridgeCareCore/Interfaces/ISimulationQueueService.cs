using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationQueueService
    {
        PagingPageModel<QueuedSimulationDTO> GetSimulationQueuePage(PagingRequestModel<QueuedSimulationDTO> request, string role);
    }
}
