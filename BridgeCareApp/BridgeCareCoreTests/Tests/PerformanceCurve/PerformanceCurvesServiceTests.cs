using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class PerformanceCurvesServiceTests
    {
        private PerformanceCurvesService performanceCurvesService;

        private Mock<IExpressionValidationService> SetupMock(Guid performanceCurveLibraryId)
        {
            var dbContext = TestHelper.DbContext;
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var mockExpressionValidationService = new Mock<IExpressionValidationService>();
                dbContext.Add(new PerformanceCurveLibraryEntity { Id = performanceCurveLibraryId, Name = "TestPerformanceCurveLibrary" });
                dbContext.SaveChanges();
            return mockExpressionValidationService;
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = SetupMock(libraryId);
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            var hubService = HubServiceMocks.Default();
            performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

            // Act
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(libraryId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.WarningMessage);
            Assert.Single(result.PerformanceCurveLibraryDTO.PerformanceCurves);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].Equation);
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileInvalidAttributeTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = SetupMock(libraryId);
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            var hubService = HubServiceMocks.Default();
            performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

            // Act
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportPerformanceCurveInvalidAttribute.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(libraryId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error occured in import of performance curve(s): No attribute found having name AGE_.", result.WarningMessage);
        }

        [Fact]
        public void ImportScenarioPerformanceCurvesFileTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = SetupMock(libraryId);
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
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

        [Fact]
        public void ImportScenarioPerformanceCurvesFileInvalidCriterionTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = SetupMock(libraryId);
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = false });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
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

        [Fact]
        public void ImportScenarioPerformanceCurvesFileInvalidEquationTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = SetupMock(libraryId);
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = false });
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
