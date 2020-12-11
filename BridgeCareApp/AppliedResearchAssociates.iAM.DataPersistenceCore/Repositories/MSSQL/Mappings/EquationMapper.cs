using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class EquationMapper
    {
        public static EquationEntity ToEntity(this Equation domain) =>
            new EquationEntity
            {
                Id = domain.Id,
                Expression = domain.Expression
            };
    }
}
