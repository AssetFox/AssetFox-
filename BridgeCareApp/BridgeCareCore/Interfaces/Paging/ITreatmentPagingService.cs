using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentPagingService
    {
        TreatmentLibraryDTO GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest);

        List<TreatmentDTO> GetSyncedScenarioDataset(Guid simulationId, PagingSyncModel<TreatmentDTO> request);
    }
}

