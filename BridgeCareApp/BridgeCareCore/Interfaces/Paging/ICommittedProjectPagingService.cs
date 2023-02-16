using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectPagingService
    {
        PagingPageModel<SectionCommittedProjectDTO> GetCommittedProjectPage(List<SectionCommittedProjectDTO> committedProjects, PagingRequestModel<SectionCommittedProjectDTO> request);

        List<SectionCommittedProjectDTO> GetSyncedDataset(Guid simulationId, PagingSyncModel<SectionCommittedProjectDTO> request);
    }
}
