using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class ScenarioPerformanceCurveEquationEntity : BaseEquationJoinEntity
    {
        public Guid ScenarioPerformanceCurveId { get; set; }

        public virtual ScenarioPerformanceCurveEntity ScenarioPerformanceCurve { get; set; }
    }
}
