using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Interfaces
{
    public interface ITreatmentPagingService
    {
        List<TreatmentDTO> GetSyncedLibraryDataset(LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO> upsertRequest);

        List<TreatmentDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<TreatmentDTO> request);
    }
}

