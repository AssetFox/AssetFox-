using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectConsequenceRepository
    {
        void CreateCommittedProjectConsequences(Dictionary<Guid, List<(Guid attributeId, TreatmentConsequence consequence)>> consequencePerAttributeIdPerProjectId);
    }
}
