using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveTestSetup
    {
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
            var attributeId = unitOfWork.Context.Attribute.First().Id;
            curve.AttributeId = attributeId;
            unitOfWork.Context.PerformanceCurve.Add(curve);
            unitOfWork.Context.SaveChanges();
            return curve;
        }
    }
}
