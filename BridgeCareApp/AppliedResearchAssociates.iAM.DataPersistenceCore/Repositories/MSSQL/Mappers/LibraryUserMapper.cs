﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class LibraryUserMapper
    {
        public static BudgetLibraryUserEntity ToBudgetLibraryUserEntity(this LibraryUserDTO dto, Guid budgetLibraryId) =>
            new BudgetLibraryUserEntity
            {
                BudgetLibraryId = budgetLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };

        public static LibraryUserDTO ToDto(this BudgetLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
    }
}