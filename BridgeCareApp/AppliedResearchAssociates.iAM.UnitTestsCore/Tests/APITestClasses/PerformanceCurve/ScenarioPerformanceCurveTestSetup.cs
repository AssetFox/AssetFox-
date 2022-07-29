using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioPerformanceCurveTestSetup
    {
        public static ScenarioPerformanceCurveEntity SetupForScenarioCurveGet(IUnitOfWork unitOfWork, Guid simulationId, Guid performanceCurveId)
        {
            var performanceCurve = PerformanceCurveTestSetup.ScenarioEntity(simulationId, performanceCurveId);
            performanceCurve.AttributeId = unitOfWork.Context.Attribute.First().Id;
            unitOfWork.Context.ScenarioPerformanceCurve.Add(performanceCurve);
            unitOfWork.Context.SaveChanges();
            return performanceCurve;
        }
    }
}
