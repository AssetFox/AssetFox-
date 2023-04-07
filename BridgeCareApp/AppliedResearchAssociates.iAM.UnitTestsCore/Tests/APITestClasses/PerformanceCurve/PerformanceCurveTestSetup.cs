using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveTestSetup
    {
        private static PerformanceCurveDTO TestPerformanceCurveDto(Guid libraryId, Guid curveId, string attributeName, string equation)
        {
            var equationDto = equation == null ? null : EquationTestSetup.Dto(equation);
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
                Equation = equationDto,
            };
            return curve;
        }

        /// <summary>If an equation is passed in, it needs to have a nonempty id. BUT the id will
        /// be changed when it is inserted.</summary>
        public static PerformanceCurveDTO TestLibraryPerformanceCurveInDb(IUnitOfWork unitOfWork, Guid libraryId, Guid curveId, string attributeName, string equation = null)
        {
            var curve = TestPerformanceCurveDto(libraryId, curveId, attributeName, "2");
            var curves = new List<PerformanceCurveDTO> { curve };
            unitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(curves, libraryId);
            var curvesFromDb = unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(libraryId);
            var curveToReturn = curvesFromDb.Single(c => c.Id == curveId);
            return curveToReturn;
        }
    }
}
