using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void CreatePerformanceCurve(this PerformanceCurveEntity entity, Simulation simulation)
        {
            var performanceCurve = simulation.AddPerformanceCurve();
            performanceCurve.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            performanceCurve.Name = entity.Name;
            performanceCurve.Shift = entity.Shift;
            performanceCurve.Equation.Expression = entity.PerformanceCurveEquationJoin?.Equation.Expression ?? string.Empty;
            performanceCurve.Criterion.Expression =
                entity.CriterionLibraryPerformanceCurveJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
        }
    }
}
