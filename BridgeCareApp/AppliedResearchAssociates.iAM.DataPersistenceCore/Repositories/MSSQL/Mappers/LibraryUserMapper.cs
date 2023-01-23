using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
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
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };
        public static CashFlowRuleLibraryUserEntity ToCashFlowRuleLibraryUserEntity(this LibraryUserDTO dto, Guid cashFlowRuleLibraryId) =>
            new CashFlowRuleLibraryUserEntity
            {
                CashFlowRuleLibraryId= cashFlowRuleLibraryId,
                UserId = dto.UserId,
                AccessLevel= (int)dto.AccessLevel,  
            };

        public static LibraryUserDTO ToDto(this RemainingLifeLimitLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this DeficientConditionGoalLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this CashFlowRuleLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
    }
}
