using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
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

        public static TreatmentLibraryUserEntity ToTreatmentLibraryUserEntity(this LibraryUserDTO dto, Guid treatmentLibraryId) =>
            new TreatmentLibraryUserEntity
            {
                TreatmentLibraryId = treatmentLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
            };
        public static PerformanceCurveLibraryUserEntity ToPerformanceCurveLibraryUserEntity(this LibraryUserDTO dto, Guid performanceCurveLibraryId) =>
            new PerformanceCurveLibraryUserEntity
            {
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                UserId = dto.UserId,
                AccessLevel = (int)dto.AccessLevel
            };
        public static LibraryUserDTO ToDto(this BudgetLibraryUserEntity entity) =>
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
    }
}
