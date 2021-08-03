using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

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

        public static void CreateTreatmentCost(this ScenarioTreatmentCostEntity entity, SelectableTreatment selectableTreatment)
        {
            var treatmentCost = selectableTreatment.AddCost();
            treatmentCost.Id = entity.Id;
            treatmentCost.Equation.Expression = entity.ScenarioTreatmentCostEquationJoin?.Equation.Expression ?? string.Empty;
            treatmentCost.Criterion.Expression =
                entity.CriterionLibraryScenarioTreatmentCostJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
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

        public static TreatmentCostDTO ToDto(this ScenarioTreatmentCostEntity entity) =>
            new TreatmentCostDTO
            {
                Id = entity.Id,
                Equation = entity.ScenarioTreatmentCostEquationJoin != null
                    ? entity.ScenarioTreatmentCostEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                CriterionLibrary = entity.CriterionLibraryScenarioTreatmentCostJoin != null
                    ? entity.CriterionLibraryScenarioTreatmentCostJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };
    }
}
