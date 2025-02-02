﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BenefitQuantifierMapper
    {
        public static BenefitQuantifierDTO ToDto(this BenefitQuantifierEntity entity) =>
            new BenefitQuantifierDTO
            {
                NetworkId = entity.NetworkId,
                Equation = entity.Equation?.ToDto() ?? new EquationDTO {Id = Guid.NewGuid()}
            };

        public static BenefitQuantifierEntity ToEntity(this BenefitQuantifierDTO dto) =>
            new BenefitQuantifierEntity {NetworkId = dto.NetworkId, EquationId = dto.Equation.Id};
    }
}
