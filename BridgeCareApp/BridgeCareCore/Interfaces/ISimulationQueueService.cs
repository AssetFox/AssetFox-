using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface IWorkQueueService
    {
        PagingPageModel<QueuedWorkDTO> GetWorkQueuePage(PagingRequestModel<QueuedWorkDTO> request);
        PagingPageModel<QueuedWorkDTO> GetFastWorkQueuePage(PagingRequestModel<QueuedWorkDTO> request);
        QueuedWorkDTO GetQueuedWorkByWorkId(string workId);
        QueuedWorkDTO GetFastQueuedWorkByWorkId(string workId);
        QueuedWorkDTO GetQueuedWorkByDomainIdAndWorkType(Guid domainId, WorkType workType);
        QueuedWorkDTO GetFastQueuedWorkByDomainIdAndWorkType(Guid domainId, WorkType workType);
    }
}
