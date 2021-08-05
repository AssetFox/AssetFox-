using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities
{
    public class ScenarioPerformanceCurveEntity : BasePerformanceCurveEntity
    {
        public Guid SimulationId { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual CriterionLibraryScenarioPerformanceCurveEntity CriterionLibraryScenarioPerformanceCurveJoin { get; set; }

        public virtual ScenarioPerformanceCurveEquationEntity ScenarioPerformanceCurveEquationJoin { get; set; }
    }
}
