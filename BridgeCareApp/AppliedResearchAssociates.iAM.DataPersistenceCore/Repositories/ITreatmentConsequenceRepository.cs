using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentConsequenceRepository
    {
        void CreateScenarioConditionalTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId);

        void UpsertOrDeleteTreatmentConsequences(
            Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId, Guid libraryId);

        void UpsertOrDeleteScenarioTreatmentConsequences(
            Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId, Guid simulationId);
    }
}
