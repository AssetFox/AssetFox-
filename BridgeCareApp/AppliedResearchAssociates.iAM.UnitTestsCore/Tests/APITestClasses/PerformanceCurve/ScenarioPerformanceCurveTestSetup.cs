using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioPerformanceCurveTestSetup
    {
        // WjJake -- This is a for-example of setups that return an entity. All calls to it either
        // add the entity to the db via the Context (I think you said that was a no-no?) or
        // convert the entity to a dto before using it. Thus it is likely possible to replace the
        // call with a call that returns a dto. If doing this, I'd want to do it as broadly as possible,
        // i.e. across different types of entities.
        public static ScenarioPerformanceCurveEntity ScenarioEntity(Guid simulationId, Guid attributeId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = RandomStrings.WithPrefix("Curve name");
            return new ScenarioPerformanceCurveEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                Name = resolveName,
                Shift = false,
                AttributeId = attributeId,
            };
        }


        public static ScenarioPerformanceCurveEntity SetupForScenarioCurveGet(IUnitOfWork unitOfWork, Guid simulationId, Guid performanceCurveId)
        {
            var attributeId = unitOfWork.Context.Attribute.First().Id;
            var performanceCurve = ScenarioEntity(simulationId, attributeId, performanceCurveId);
            unitOfWork.Context.ScenarioPerformanceCurve.Add(performanceCurve);
            unitOfWork.Context.SaveChanges();
            return performanceCurve;
        }
    }
}
