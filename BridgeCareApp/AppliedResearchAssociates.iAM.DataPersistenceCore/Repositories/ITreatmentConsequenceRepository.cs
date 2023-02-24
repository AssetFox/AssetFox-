using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
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

        List<TreatmentConsequenceDTO> GetScenarioTreatmentConsequencesByTreatmentId(Guid treatmentId);

        List<TreatmentConsequenceDTO> GetTreatmentConsequencesByTreatmentId(Guid treatmentId);

        List<CommittedProjectConsequenceDTO> GetCommittedProjectConsequencesByProjectId(Guid projectId);
        List<TreatmentConsequenceDTO> GetTreatmentConsequencesByLibraryIdAndTreatmentName(Guid treatmentLibraryId, string treatmentName);
    }
}
