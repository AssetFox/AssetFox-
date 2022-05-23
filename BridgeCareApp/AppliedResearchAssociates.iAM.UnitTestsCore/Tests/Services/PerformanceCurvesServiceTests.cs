using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Moq;
using OfficeOpenXml;
using Xunit;
using System.Threading;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class PerformanceCurvesServiceTests
    {
        private static readonly Guid performanceCurveLibraryId = Guid.NewGuid();                
        private PerformanceCurvesService performanceCurvesService;
        private readonly TestHelper _testHelper;
        private readonly Mock<IExpressionValidationService> mockExpressionValidationService;        

        public PerformanceCurvesServiceTests()
        {
            Thread.Sleep(3000);
            _testHelper = TestHelper.Instance;
            var dbContext = _testHelper.DbContext;
            Thread.Sleep(2000);
            if (!dbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.SetupDefaultHttpContext();
            }            
            mockExpressionValidationService = new Mock<IExpressionValidationService>();
            if (!dbContext.PerformanceCurveLibrary.Any())
            {
                dbContext.Add(new PerformanceCurveLibraryEntity { Id = performanceCurveLibraryId, Name = "TestPerformanceCurveLibrary" });
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Act
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportPerformanceCurve.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.WarningMessage);
            Assert.Equal(result.PerformanceCurveLibraryDTO.PerformanceCurves.Count, 1);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary);
            Assert.NotNull(result.PerformanceCurveLibraryDTO.PerformanceCurves[0].Equation);
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileInvalidAttributeTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Act
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportPerformanceCurveInvalidAttribute.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.WarningMessage, "Error occured in import of performance curve(s): No attribute found having name AGE_.");
        }

        [Fact]
        public void ImportScenarioPerformanceCurvesFileTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = (Guid)_testHelper.DbContext.Simulation.FirstOrDefault()?.Id;
            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.WarningMessage);
            Assert.Equal(result.PerformanceCurves.Count, 1);
            Assert.NotNull(result.PerformanceCurves[0].CriterionLibrary);
            Assert.NotNull(result.PerformanceCurves[0].Equation);
        }

        [Fact]
        public void ImportScenarioPerformanceCurvesFileInvalidCriterionTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = false });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = (Guid)_testHelper.DbContext.Simulation.FirstOrDefault()?.Id;
            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WarningMessage.Contains("The following performace curves are imported without criteria due to invalid values"));
            Assert.True(result.PerformanceCurves.Count > 0);
        }

        [Fact]
        public void ImportScenarioPerformanceCurvesFileInvalidEquationTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            mockExpressionValidationService.Setup(m => m.ValidateEquation(It.IsAny<EquationValidationParameters>())).Returns(new ValidationResult { IsValid = false });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = (Guid)_testHelper.DbContext.Simulation.FirstOrDefault()?.Id;
            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.WarningMessage.Contains("The following performace curves are imported without equation due to invalid values"));
            Assert.True(result.PerformanceCurves.Count > 0);
        }
    }
}
