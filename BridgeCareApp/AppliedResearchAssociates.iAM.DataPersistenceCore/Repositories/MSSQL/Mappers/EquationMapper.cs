﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class EquationMapper
    {
        public static EquationEntity ToEntity(this Equation domain) =>
            new EquationEntity { Id = domain.Id, Expression = domain.Expression };

        public static EquationEntity ToEntity(this EquationDTO dto) =>
            new EquationEntity { Id = dto.Id, Expression = dto.Expression };

        public static EquationDTO ToDto(this EquationEntity entity) =>
            new EquationDTO { Id = entity.Id, Expression = entity.Expression };
    }
}
