using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using System.Security.Principal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class LibraryUserMapper
    {
        public static RemainingLifeLimitLibraryUserEntity ToRemainingLifeLimitLibraryUserEntity(this LibraryUserDTO dto, Guid remainingLifeLimitLibraryId) =>
            new RemainingLifeLimitLibraryUserEntity
            {
                LibraryId = remainingLifeLimitLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };
        public static DeficientConditionGoalLibraryUserEntity ToDeficientConditionGoalLibraryUserEntity(this LibraryUserDTO dto, Guid deficientConditionGoalLibraryId) =>
            new DeficientConditionGoalLibraryUserEntity
            {
                LibraryId = deficientConditionGoalLibraryId,
                UserId = dto.UserId,
                AccessLevel= (int)dto.AccessLevel,
            };
        public static BudgetLibraryUserEntity ToBudgetLibraryUserEntity(this LibraryUserDTO dto, Guid budgetLibraryId) =>
            new BudgetLibraryUserEntity
            {
                BudgetLibraryId = budgetLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };
        public static BudgetPriorityLibraryUserEntity ToBudgetPriorityLibraryUserEntity(this LibraryUserDTO dto, Guid budgetPriorityLibraryId) =>
            new BudgetPriorityLibraryUserEntity
            {
                LibraryId = budgetPriorityLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
            };
        public static CashFlowRuleLibraryUserEntity ToCashFlowRuleLibraryUserEntity(this LibraryUserDTO dto, Guid cashFlowRuleLibraryId) =>
            new CashFlowRuleLibraryUserEntity
            {
                LibraryId= cashFlowRuleLibraryId,
                UserId = dto.UserId,
                AccessLevel= (int)dto.AccessLevel,  
            };
        public static CalculatedAttributeLibraryUserEntity ToCalculatedAttributeLibraryUserEntity(this LibraryUserDTO dto, Guid calculatedAttributeLibraryId) =>
            new CalculatedAttributeLibraryUserEntity
            {
                LibraryId = calculatedAttributeLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel,
            };
        public static TreatmentLibraryUserEntity ToTreatmentLibraryUserEntity(this LibraryUserDTO dto, Guid treatmentLibraryId) =>
            new TreatmentLibraryUserEntity
            {
                LibraryId = treatmentLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
            };
        public static PerformanceCurveLibraryUserEntity ToPerformanceCurveLibraryUserEntity(this LibraryUserDTO dto, Guid performanceCurveLibraryId) =>
            new PerformanceCurveLibraryUserEntity
            {
                LibraryId = performanceCurveLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
            };
        public static TargetConditionGoalLibraryUserEntity ToTargetConditionGoalLibraryUserEntity(this LibraryUserDTO dto, Guid targetConditionGoalLibraryId) =>
            new TargetConditionGoalLibraryUserEntity
            {
                LibraryId = targetConditionGoalLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
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
        public static LibraryUserDTO ToDto(this CalculatedAttributeLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel
            };
        public static LibraryUserDTO ToDto(this BudgetLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this BudgetPriorityLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this TreatmentLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel,
            };
        public static LibraryUserDTO ToDto(this PerformanceCurveLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity?.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel
            };
        public static LibraryUserDTO ToDto(this TargetConditionGoalLibraryUserEntity entity) =>
            new LibraryUserDTO
            {
                UserId = entity.UserId,
                UserName = entity.User?.Username,
                AccessLevel = (LibraryAccessLevel)entity.AccessLevel
            };
    }
}
