using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentCostRepository
    {
        void UpsertOrDeleteTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> treatmentCostPerTreatmentId,
            Guid libraryId);

        void UpsertOrDeleteScenarioTreatmentCosts(Dictionary<Guid, List<TreatmentCostDTO>> scenarioTreatmentCostPerTreatmentId,
            Guid SimulationId);

        List<TreatmentCostDTO> GetTreatmentCostByScenarioTreatmentId(Guid treatmentId); // unused except for tests 1/9/24

        List<TreatmentCostDTO> GetTreatmentCostByTreatmentId(Guid treatmentId);  // unused except for tests 1/9/24
        List<TreatmentCostDTO> GetTreatmentCostsWithEquationJoinsByLibraryIdAndTreatmentName(Guid treatmentLibraryId, string treatmentName);
    }
}
