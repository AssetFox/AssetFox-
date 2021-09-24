using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentCostMapper
    {
        public static ScenarioTreatmentCostEntity ToScenarioEntity(this TreatmentCost domain, Guid treatmentId) =>
            new ScenarioTreatmentCostEntity
            {
                Id = domain.Id,
                ScenarioSelectableTreatmentId = treatmentId
            };

        public static TreatmentCostEntity ToLibraryEntity(this TreatmentCostDTO dto, Guid treatmentId) =>
            new TreatmentCostEntity { Id = dto.Id, TreatmentId = treatmentId };

        public static ScenarioTreatmentCostEntity ToScenarioEntity(this TreatmentCostDTO dto, Guid treatmentId) =>
            new ScenarioTreatmentCostEntity { Id = dto.Id, ScenarioSelectableTreatmentId = treatmentId };

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
