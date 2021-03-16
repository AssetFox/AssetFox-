using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentConsequenceRepository
    {
        void CreateTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId, string simulationName);

        void UpsertOrDeleteTreatmentConsequences(
            Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId, Guid libraryId, Guid? userId = null);
    }
}
