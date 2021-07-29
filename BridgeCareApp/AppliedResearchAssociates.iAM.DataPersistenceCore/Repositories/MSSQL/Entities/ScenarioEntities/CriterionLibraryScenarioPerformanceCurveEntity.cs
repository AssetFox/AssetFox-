using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryScenarioPerformanceCurveEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid ScenarioPerformanceCurveId { get; set; }

        public virtual ScenarioPerformanceCurveEntity ScenarioPerformanceCurve { get; set; }
    }
}
