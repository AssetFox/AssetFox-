using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveTestSetup
    {
        public static ScenarioPerformanceCurveEntity ScenarioEntity(Guid simulationId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            return new ScenarioPerformanceCurveEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                Name = "Test Name",
                Shift = false,
            };
        }
    }
}
