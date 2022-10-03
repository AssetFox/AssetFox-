using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
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

        private static PerformanceCurveDTO TestPerformanceCurveDto(Guid libraryId, Guid curveId, string attributeName)
        {
            var randomName = RandomStrings.WithPrefix("Curve");
            var curve = new PerformanceCurveDTO
            {
                Id = curveId,
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = libraryId,
                },
                Name = randomName,
                Attribute = attributeName,
            };
            return curve;
        }

        public static PerformanceCurveDTO TestPerformanceCurveInDb2(IUnitOfWork unitOfWork, Guid libraryId, Guid curveId, string attributeName)
        {
            var curve = TestPerformanceCurveDto(libraryId, curveId, attributeName);
            var curves = new List<PerformanceCurveDTO> { curve };
            unitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(curves, libraryId);
            return curve;
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
