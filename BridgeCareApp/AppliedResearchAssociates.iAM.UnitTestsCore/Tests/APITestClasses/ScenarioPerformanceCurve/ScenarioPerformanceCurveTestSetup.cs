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


        public static ScenarioPerformanceCurveEntity EntityInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid curveId)
        {
            var attributeId = unitOfWork.Context.Attribute.First().Id;
            var performanceCurve = ScenarioEntity(simulationId, attributeId, curveId);
            performanceCurve.AttributeId = attributeId;
            unitOfWork.Context.ScenarioPerformanceCurve.Add(performanceCurve);
            unitOfWork.Context.SaveChanges();
            return performanceCurve;
        }
    }
}
