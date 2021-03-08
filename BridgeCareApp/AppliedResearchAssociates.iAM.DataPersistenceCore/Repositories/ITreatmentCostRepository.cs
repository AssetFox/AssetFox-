using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentCostRepository
    {
        void CreateTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId,
            string simulationName);

        void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId,
            Guid libraryId);
    }
}
