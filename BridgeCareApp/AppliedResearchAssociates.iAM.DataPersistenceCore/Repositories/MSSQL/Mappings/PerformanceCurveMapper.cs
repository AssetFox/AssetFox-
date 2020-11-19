using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using SimulationAnalysisDomains = AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class PerformanceCurveMapper
    {
        public static PerformanceCurveEntity ToEntity(this PerformanceCurve domain, Guid performanceCurveLibraryId, Guid attributeId) =>
            new PerformanceCurveEntity
            {
                Id = Guid.NewGuid(),
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Shift = domain.Shift
            };

        public static PerformanceCurve ToDomain(this PerformanceCurveEntity entity)
        {
            var domain = new PerformanceCurve(new Explorer())
            {
                Attribute = (NumberAttribute)Convert
                    .ChangeType(entity.Attribute.ToDomain().ToSimulationAnalysisAttribute(), typeof(NumberAttribute)),
                Name = entity.Name,
                Shift = entity.Shift
            };
            domain.Equation.Expression = entity.PerformanceCurveEquationJoin?.Equation.Expression ?? string.Empty;
            domain.Criterion.Expression =
                entity.CriterionLibraryPerformanceCurveJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            return domain;
        }
    }
}
