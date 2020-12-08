using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectConsequenceRepository
    {
        void CreateCommittedProjectConsequences(
            List<((Guid committedProjectId, Guid attributeId) committedProjectIdAttributeIdTuple, TreatmentConsequence
                committedProjectConsequence)> committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple);
    }
}
