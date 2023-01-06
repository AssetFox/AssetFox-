using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Interfaces
{
    public interface ICalculatedAttributePagingService
    {
        PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetLibraryCalculatedAttributePage(Guid libraryId, CalculatedAttributePagingRequestModel request);
        PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetScenarioCalculatedAttributePage(Guid simulationId, CalculatedAttributePagingRequestModel request);
        List<CalculatedAttributeDTO> GetSyncedScenarioDataset(Guid simulationId, CalculatedAttributePagingSyncModel request);
        List<CalculatedAttributeDTO> GetSyncedLibraryDataset(Guid libraryId, CalculatedAttributePagingSyncModel request);
    }
}
