using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IEquationRepository
    {
        void CreateEquations(Dictionary<Guid, EquationEntity> equationEntitiesPerEntityId, string joinEntity);
        void CreateEquations(List<EquationEntity> equationEntities);
    }
}
