﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class LibraryUserMapper
    {
        public static RemainingLifeLimitLibraryUserEntity ToRemainingLifeLimitLibraryUserEntity(this LibraryUserDTO dto, Guid remainingLifeLimitLibraryId) =>
            new RemainingLifeLimitLibraryUserEntity
            {
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };
        public static DeficientConditionGoalLibraryUserEntity ToDeficientConditionGoalLibraryUserEntity(this LibraryUserDTO dto, Guid deficientConditionGoalLibraryId) =>
            new DeficientConditionGoalLibraryUserEntity
            {
                DeficientConditionGoalLibraryId = deficientConditionGoalLibraryId,
        public static BudgetLibraryUserEntity ToBudgetLibraryUserEntity(this LibraryUserDTO dto, Guid budgetLibraryId) =>
            new BudgetLibraryUserEntity
            {
                BudgetLibraryId = budgetLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };

        public static LibraryUserDTO ToDto(this RemainingLifeLimitLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this DeficientConditionGoalLibraryUserEntity entity) =>
        public static LibraryUserDTO ToDto(this BudgetLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
    }
}
