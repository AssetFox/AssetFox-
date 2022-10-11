using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses.PerformanceCurve
{
    public class PerformanceCurveRepositoryUpsertTests
    {
        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveInDb_Description()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            performanceCurveLibraryDto.Description = "Updated Description";

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Equal(performanceCurveLibraryDto.Description, performanceCurveLibraryDtoAfter.Description);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveInDb_UpdatesShiftAndDescription()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveDto = PerformanceCurveTestSetup.TestPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            performanceCurveDto.Shift = true;
            var performanceCurveDtos = new List<PerformanceCurveDTO> { performanceCurveDto };
            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Shift, performanceCurveDtoAfter.Shift);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }
    }
}
