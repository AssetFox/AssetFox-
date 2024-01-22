using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentConsequenceRepository
    {
        void UpsertOrDeleteTreatmentConsequences(
            Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId, Guid libraryId);

        void UpsertOrDeleteScenarioTreatmentConsequences(
            Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId, Guid simulationId);

        // As of 1/8/24, the three getters here are unused. Leaving them because Bryson says they might be used for PAMS.
        List<TreatmentConsequenceDTO> GetScenarioTreatmentConsequencesByTreatmentId(Guid treatmentId); //  unused except for tests 1/9/24

        List<TreatmentConsequenceDTO> GetTreatmentConsequencesByTreatmentId(Guid treatmentId); // unused except for tests 1/9/24

        List<CommittedProjectConsequenceDTO> GetCommittedProjectConsequencesByProjectId(Guid projectId);  // unused 1/9/24

        /// <summary>Returned consequences do NOT include their equations.</summary> 
        List<TreatmentConsequenceDTO> GetTreatmentConsequencesByLibraryIdAndTreatmentName(Guid treatmentLibraryId, string treatmentName);
    }
}
