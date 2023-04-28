using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface IWorkQueueService
    {
        PagingPageModel<QueuedWorkDTO> GetWorkQueuePage(PagingRequestModel<QueuedWorkDTO> request);
        QueuedWorkDTO GetQueuedWorkByWorkId(Guid workId);
    }
}
