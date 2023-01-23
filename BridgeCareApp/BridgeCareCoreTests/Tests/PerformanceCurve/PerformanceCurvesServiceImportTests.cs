using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
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

        private PerformanceCurvesService CreatePerformanceCurvesService(Mock<IUnitOfWork> unitOfWork, Mock<IExpressionValidationService> expressionValidationService = null)
        {
            var hubService = HubServiceMocks.DefaultMock();
            expressionValidationService ??= ExpressionValidationServiceMocks.EverythingIsValid();
            var service = new PerformanceCurvesService(unitOfWork.Object, hubService.Object, expressionValidationService.Object);
            return service;
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFromFile_CallsUpsertOrDeleteOnRepository()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("PerformanceCurve library");
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
            var performanceCurvesService = CreatePerformanceCurvesService(unitOfWork);

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

        private Mock<IExpressionValidationService> SetupMockOld(Guid performanceCurveLibraryId)
        {
            var dbContext = TestHelper.DbContext;
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var mockExpressionValidationService = ExpressionValidationServiceMocks.New();
            dbContext.Add(new PerformanceCurveLibraryEntity { Id = performanceCurveLibraryId, Name = "TestPerformanceCurveLibrary" });
            dbContext.SaveChanges();
            return mockExpressionValidationService;
        }

        [Fact]
        public void ImportScenarioPerformanceCurvesFileTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var unitOfWork = UnitOfWorkMocks.New();
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New(unitOfWork); 
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            var performanceCurvesService = CreatePerformanceCurvesService(unitOfWork);
            var args = new List<List<PerformanceCurveDTO>>();
            performanceCurveRepo.Setup(r => r.UpsertOrDeleteScenarioPerformanceCurves(Capture.In(args), It.IsAny<Guid>()));
            performanceCurveRepo.Setup(r => r.GetScenarioPerformanceCurves(It.IsAny<Guid>()))
                .Returns(() => args.Last());
                //.Returns<List<PerformanceCurveDTO>>(x => args.Last());

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            var simulationId = Guid.NewGuid();

            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.Null(result.WarningMessage);
            var performanceCurve = result.PerformanceCurves.Single();
            Assert.Equal("ScenarioPerformance_Eq1", performanceCurve.Name);
            Assert.Equal("AGE", performanceCurve.Attribute);
            Assert.Equal("[AGE]", performanceCurve.Equation.Expression);
            performanceCurveRepo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.UpsertOrDeleteScenarioPerformanceCurves));
            performanceCurveRepo.SingleInvocationWithName(nameof(IPerformanceCurveRepository.GetScenarioPerformanceCurves));
        }

        // The import goes ahead and completes, even if there are invalid
        // equations and/or criteria. It simply issues warnings in that case.
        // This needs to change. Instead, it should reject if anything is invalid.
        [Fact]
        public void ImportScenarioPerformanceCurvesFile_ImportScenarioCurvesThrows_ExceptionMessageIsReturned()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var mockExpressionValidationService = ExpressionValidationServiceMocks.New();
            mockExpressionValidationService.SetupValidateAnyCriterionWithoutResults(false);
            mockExpressionValidationService.SetupValidateAnyEquation(true);
            var performanceCurveRepo = PerformanceCurveRepositoryMocks.New();
            performanceCurveRepo.SetupUpsertOrDeleteScenarioPerformanceCurvesThrows("exception message");
            var mockUnitOfWork = UnitOfWorkMocks.New();
            mockUnitOfWork.Setup(m => m.PerformanceCurveRepo).Returns(performanceCurveRepo.Object);
            var performanceCurvesService = CreatePerformanceCurvesService(mockUnitOfWork, mockExpressionValidationService);

            // Act            
            var filePathToImport = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestImportScenarioPerformanceCurve.xlsx");
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));

            var result = performanceCurvesService.ImportScenarioPerformanceCurvesFile(simulationId, excelPackage, new UserCriteriaDTO());

            // Assert
            Assert.NotNull(result);
            Assert.Contains("exception message", result.WarningMessage);
        }

        [Fact(Skip = WjFixMe)]
        public void ImportScenarioPerformanceCurvesFileInvalidEquationTest()
        {
            // Setup
            var libraryId = Guid.NewGuid();
            var mockExpressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.Default();
            var performanceCurvesService = new PerformanceCurvesService(TestHelper.UnitOfWork, hubService, mockExpressionValidationService.Object);

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
