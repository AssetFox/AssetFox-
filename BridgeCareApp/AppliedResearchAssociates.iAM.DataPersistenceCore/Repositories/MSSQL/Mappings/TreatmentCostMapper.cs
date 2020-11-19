using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class TreatmentCostMapper
    {
        public static TreatmentCostEntity ToEntity(this TreatmentCost domain, Guid treatmentId) =>
            new TreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                TreatmentId = treatmentId
            };

        public static TreatmentCost ToSimulationAnalysisDomain(this TreatmentCostEntity entity, SelectableTreatment selectableTreatment)
        {
            var treatmentCost = selectableTreatment.AddCost();
            treatmentCost.Equation.Expression = entity.TreatmentCostEquationJoin?.Equation.Expression ?? string.Empty;
            treatmentCost.Criterion.Expression =
                entity.CriterionLibraryTreatmentCostJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
            return treatmentCost;
        }
    }
}
