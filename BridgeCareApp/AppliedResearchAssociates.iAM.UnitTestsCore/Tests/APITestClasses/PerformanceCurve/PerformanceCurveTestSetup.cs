using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

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

        public static PerformanceCurveEntity TestPerformanceCurve(Guid libraryId, Guid curveId)
        {
            var returnValue = new PerformanceCurveEntity
            {
                Id = curveId,
                PerformanceCurveLibraryId = libraryId,
                Name = "Test Name",
                Shift = false
            };
            return returnValue;
        }

        public static PerformanceCurveEntity TestPerformanceCurveInDb(IUnitOfWork unitOfWork, Guid libraryId, Guid curveId)
        {
            var curve = TestPerformanceCurve(libraryId, curveId);
            curve.AttributeId = unitOfWork.Context.Attribute.First().Id;
            unitOfWork.Context.PerformanceCurve.Add(curve);
            unitOfWork.Context.SaveChanges();
            return curve;
        }
    }
}
