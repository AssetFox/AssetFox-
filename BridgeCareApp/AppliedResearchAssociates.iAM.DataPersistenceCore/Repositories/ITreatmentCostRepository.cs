using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentCostRepository
    {
        void CreateScenarioTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId,
            string simulationName);

        void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId,
            Guid libraryId);

        void UpsertOrDeleteScenarioTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> scenarioTreatmentCostPerTreatmentId,
            Guid SimulationId);
    }
}
