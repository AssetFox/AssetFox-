using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Interfaces
{
    public interface ICalculatedAttributePagingService
    {
        PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetScenarioPage(Guid libraryId, CalculatedAttributePagingRequestModel request);
        PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO> GetLibraryPage(Guid simulationId, CalculatedAttributePagingRequestModel request);
        List<CalculatedAttributeDTO> GetSyncedScenarioDataSet(Guid simulationId, CalculatedAttributePagingSyncModel request);
        List<CalculatedAttributeDTO> GetSyncedLibraryDataset(Guid libraryId, CalculatedAttributePagingSyncModel request);
    }
}
