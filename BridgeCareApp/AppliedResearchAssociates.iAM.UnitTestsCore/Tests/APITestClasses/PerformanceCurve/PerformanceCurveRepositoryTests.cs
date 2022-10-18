using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
{
    public class PerformanceCurveRepositoryTests
    {

        [Fact]
        public void Delete_PerformanceCurveLibraryExists_Deletes()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var performanceCurveLibraryId = Guid.NewGuid();
            var performanceCurveId = Guid.NewGuid();
            var testLibrary = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, performanceCurveLibraryId);
            var curveDto = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, performanceCurveLibraryId, performanceCurveId, TestAttributeNames.ActionType);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork);
            var dtos = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibraries();

            var performanceCurveLibraryDTO = dtos.Single(dto => dto.Id == performanceCurveLibraryId);
            curveDto.CriterionLibrary = criterionLibrary;

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(performanceCurveLibraryId);


            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == performanceCurveLibraryId));
            Assert.False(TestHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
            Assert.False(
                TestHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                    _.PerformanceCurveId == performanceCurveId));
        }

    }
}
