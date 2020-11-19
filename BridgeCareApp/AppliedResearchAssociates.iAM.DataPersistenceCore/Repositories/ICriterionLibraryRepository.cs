using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICriterionLibraryRepository
    {
        void JoinEntitiesWithCriteria(Dictionary<string, List<Guid>> entityIdsPerExpression, string joinEntity,
            string simulationName);

        void JoinSelectableTreatmentEntitiesWithCriteria(Dictionary<Guid, List<string>> expressionsPerSelectableTreatmentEntityId, string simulationName);
    }
}
