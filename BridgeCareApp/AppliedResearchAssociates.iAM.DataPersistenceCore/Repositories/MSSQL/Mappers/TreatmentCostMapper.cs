using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentCostMapper
    {
        public static TreatmentCostEntity ToEntity(this TreatmentCost domain, Guid treatmentId) =>
            new TreatmentCostEntity
            {
                Id = domain.Id,
                TreatmentId = treatmentId
            };

        public static TreatmentCostEntity ToEntity(this TreatmentCostDTO dto, Guid treatmentId) =>
            new TreatmentCostEntity { Id = dto.Id, TreatmentId = treatmentId };

        public static void CreateTreatmentCost(this TreatmentCostEntity entity, SelectableTreatment selectableTreatment)
        {
            var treatmentCost = selectableTreatment.AddCost();
            treatmentCost.Id = entity.Id;
            treatmentCost.Equation.Expression = entity.TreatmentCostEquationJoin?.Equation.Expression ?? string.Empty;
            treatmentCost.Criterion.Expression =
                entity.CriterionLibraryTreatmentCostJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
        }

        public static TreatmentCostDTO ToDto(this TreatmentCostEntity entity) =>
            new TreatmentCostDTO
            {
                Id = entity.Id,
                Equation = entity.TreatmentCostEquationJoin != null
                    ? entity.TreatmentCostEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                CriterionLibrary = entity.CriterionLibraryTreatmentCostJoin != null
                    ? entity.CriterionLibraryTreatmentCostJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
