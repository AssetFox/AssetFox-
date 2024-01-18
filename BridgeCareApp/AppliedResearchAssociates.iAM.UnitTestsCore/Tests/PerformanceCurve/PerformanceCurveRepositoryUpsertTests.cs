using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class PerformanceCurveRepositoryUpsertTests
    {
        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }


        [Fact]
        public void UpsertOrDeletePerformanceCurves_CurveInDbWithEquation_UpdateRemovesCurve_EquationDeleted()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurves = performanceCurveLibraryDto.PerformanceCurves;
            performanceCurves.RemoveAt(0);
            var equationId = performanceCurve.Equation.Id;
            var equationEntityBefore = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationId);
            Assert.Equal(equationId, equationEntityBefore.Id);
            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurves, libraryId);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
            var equationEntityAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationId);
            Assert.Equal(equationId, equationEntityAfter.Id);
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
        public void UpsertPerformanceCurve_CurveInDb_UpdatesShiftAndDescription()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var library = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveDto = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
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

        [Fact]
        public void UpsertPerformanceCurves_CurveInDbWithEquation_UpdateRemovesEquationFromCurve_EquationRemoved()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var equationId = performanceCurve.Equation.Id;
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtos = performanceCurveLibraryDto.PerformanceCurves;
            var performanceCurveDto = performanceCurveDtos.Single();
            performanceCurveDto.Equation = null;

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Null(performanceCurveDtoAfter.Equation?.Expression);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var equationAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationId);
            Assert.Null(equationAfter);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_CriterionLibraryUnchanged()
        {
            Setup();
            // Arrange
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var criterionLibraryBefore = performanceCurveDto.CriterionLibrary;
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            ObjectAssertions.Equivalent(criterionLibraryBefore, criterionLibraryAfter);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveInDbWithEquation_EquationUnchanged()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDto = performanceCurveLibraryDto.PerformanceCurves[0];
            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(performanceCurveDto.Equation.Id, performanceCurveDtoAfter.Equation.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
        }

        [Fact]
        public void UpsertPerformanceCurveLibrary_CurveInDbWithCriterionLibrary_UpdateRemovesCriterionLibraryFromCurve_CriterionLibraryRemoved()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            var mergedExpression = RandomStrings.WithPrefix("MergedExpression");
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(TestHelper.UnitOfWork, "Performance Curve", mergedExpression);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtos = performanceCurveLibraryDto.PerformanceCurves;
            var performanceCurveDto = performanceCurveDtos.Single();
            performanceCurveDto.CriterionLibrary = null;

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            Assert.Equal(Guid.Empty, performanceCurveDtoAfter.CriterionLibrary.Id);
            Assert.Equal(performanceCurveDto.Attribute, performanceCurveDtoAfter.Attribute);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == libraryId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public void UpsertPerformanceCurves_CurveInDbWithCriterionLibrary_CurveNotInList_RemovesCurveAndCriterionJoin()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType);
            var mergedExpression = RandomStrings.WithPrefix("MergedExpression");
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurves = new List<PerformanceCurveDTO>();

            // Act
            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurves, libraryId);

            // Assert
            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Empty(performanceCurveLibraryDtoAfter.PerformanceCurves);
            var criterionLibraryJoinAfter = TestHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.SingleOrDefault(clpc =>
            clpc.PerformanceCurveId == curveId
            && clpc.CriterionLibraryId == libraryId);
            Assert.Null(criterionLibraryJoinAfter);
        }

        [Fact]
        public void UpsertPerformances_CurveInDbWithEquation_UpdateChangesExpression_ExpressionChangedInDb()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var curveId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurve = PerformanceCurveTestSetup.TestLibraryPerformanceCurveInDb(TestHelper.UnitOfWork, libraryId, curveId, TestAttributeNames.ActionType, "2");
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurves = performanceCurveLibraryDto.PerformanceCurves;
            var performanceCurveDto = performanceCurves.Single();
            performanceCurveDto.Equation.Expression = "123";

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurves, libraryId);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves[0];
            Assert.Equal("123", performanceCurveAfter.Equation.Expression);
            Assert.Equal(performanceCurve.Equation.Id, performanceCurveAfter.Equation.Id);
        }


        [Fact]
        public void UpsertPerformanceCurve_EmptyLibraryInDb_Adds()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            var performanceCurveDtos = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            Assert.Single(performanceCurveLibraryDtoAfter.PerformanceCurves);
        }
        [Fact]
        public void UpsertPerformanceCurveWithEquation_EmptyLibraryInDb_AddsCurveAndEquationToLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.NewGuid(),
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            var performanceCurves = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurves, libraryId); ;

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Equal("3", equationAfter.Expression);
            var equationEntity = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationAfter.Id);
            Assert.NotNull(equationEntity);
        }

        [Fact]
        public void UpsertPerformanceCurveWithEquationWithEmptyId_EmptyLibraryInDb_AddsCurveToLibraryWithoutEquation()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var equation = new EquationDTO
            {
                Expression = "3",
                Id = Guid.Empty,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                Equation = equation,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            var performanceCurveDtos = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var equationAfter = performanceCurveAfter.Equation;
            Assert.Null(equationAfter.Expression);
        }

        [Fact]
        public void UpsertPerformanceCurveWithCriterionLibrary_EmptyLibraryInDb_AddsPerformanceCurveAndCriterionLibrary()
        {
            Setup();
            var libraryId = Guid.NewGuid();
            var libraryDto = PerformanceCurveLibraryTestSetup.TestPerformanceCurveLibraryInDb(TestHelper.UnitOfWork, libraryId);
            var performanceCurveLibraryDto = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var attribute = TestHelper.UnitOfWork.AttributeRepo.GetAttributes().First();
            var criterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                MergedCriteriaExpression = "MergedCriteriaExpression",
                Description = "Description",
                IsSingleUse = true,
            };
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute.Name,
                Id = Guid.NewGuid(),
                Name = "Curve",
                CriterionLibrary = criterionLibrary,
            };
            Assert.Empty(performanceCurveLibraryDto.PerformanceCurves);
            var performanceCurveDtos = new List<PerformanceCurveDTO> { performanceCurveDto };

            TestHelper.UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveDtos, libraryId);

            var performanceCurveLibraryDtoAfter = TestHelper.UnitOfWork.PerformanceCurveRepo.GetPerformanceCurveLibrary(libraryId);
            var performanceCurveDtoAfter = performanceCurveLibraryDtoAfter.PerformanceCurves.Single();
            var criterionLibraryAfter = performanceCurveDtoAfter.CriterionLibrary;
            Assert.Equal("MergedCriteriaExpression", criterionLibraryAfter.MergedCriteriaExpression);
        }
    }
}
