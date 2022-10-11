using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.PerformanceCurve;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class PerformanceCurvesServiceImportTests
    {
        public const string WjFixMe = "Wj fix me";
        public const string Filename = "TestImportPerformanceCurve.xlsx";
        private PerformanceCurvesService performanceCurvesService;

        [Fact]
        public void ImportLibraryPerformanceCurvesFromFile_CallsUpsertOrDeleteOnRepository()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("PerformanceCurve library");
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            var unitOfWork = UnitOfWorkMocks.New();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            var expectedPerformanceCurve = new PerformanceCurveDTO
            {
                Attribute = "AGE",
                Name = "Performance_Eq1",
                Shift = false,
                CriterionLibrary = new CriterionLibraryDTO
                {
                    MergedCriteriaExpression = "[AGE]='5'",
                    Id = Guid.NewGuid(),
                },
                Equation = new EquationDTO
                {
                    Expression = "[AGE]",
                    Id = Guid.NewGuid(),
                },
                Id = Guid.NewGuid(),
            };
            var outputCurves = new List<PerformanceCurveDTO> { expectedPerformanceCurve };
            var outputDto = new PerformanceCurveLibraryDTO
            {
                Id = libraryId,
                Name = libraryName,
                PerformanceCurves = new List<PerformanceCurveDTO> { expectedPerformanceCurve },
            };
            performanceCurveRepo.SetupGetPerformanceCurveLibrary(outputDto);
            performanceCurveRepo.SetupGetPerformanceCurvesForLibrary(libraryId, outputCurves);
            unitOfWork.Setup(u => u.PerformanceCurveRepo).Returns(performanceCurveRepo.Object);
            performanceCurvesService = new PerformanceCurvesService(unitOfWork.Object, hubService, mockExpressionValidationService.Object);

            // Act
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", Filename);
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));

            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(libraryId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.WarningMessage);
            Assert.Single(result.PerformanceCurveLibraryDTO.PerformanceCurves);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].Equation);
            var upsertedCurves = performanceCurveRepo.GetUpsertedOrDeletedPerformanceCurves(libraryId);
            var upsertedCurve = upsertedCurves.Single();
            ObjectAssertions.EquivalentExcluding(expectedPerformanceCurve, upsertedCurve, c => c.Id, c => c.CriterionLibrary.Id, c => c.Equation.Id);
        }

        [Fact(Skip = WjFixMe)]
        public void ImportScenarioPerformanceCurvesFileTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);

            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.WarningMessage);
            var performanceCurve = result.PerformanceCurves.Single();
            Assert.NotNull(performanceCurve.CriterionLibrary);
            Assert.NotNull(performanceCurve.Equation);
        }

        [Fact(Skip = WjFixMe)]
        public void ImportScenarioPerformanceCurvesFileInvalidCriterionTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));

            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Contains(PerformanceCurvesService.ImportedWithoutCriterioDueToInvalidValues, result.WarningMessage);
            Assert.NotEmpty(result.PerformanceCurves);
        }

        [Fact(Skip = WjFixMe)]
        public void ImportScenarioPerformanceCurvesFileInvalidEquationTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId);
            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Contains("The following performace curves are imported without equation due to invalid values", result.WarningMessage);
            Assert.True(result.PerformanceCurves.Count > 0);
        }
    }
}
