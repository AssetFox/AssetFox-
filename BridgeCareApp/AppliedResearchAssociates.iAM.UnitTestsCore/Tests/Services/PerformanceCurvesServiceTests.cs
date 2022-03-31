using System;
using System.IO;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Services;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Services
{
    public class PerformanceCurvesServiceTests
    {
        private static readonly Guid performanceCurveLibraryId = Guid.Parse("D7DB0659-2BF4-4019-A0D4-7CA683DF07E5");
        private PerformanceCurvesService performanceCurvesService;
        private readonly TestHelper _testHelper;
        private readonly Mock<IExpressionValidationService> mockExpressionValidationService;

        public PerformanceCurvesServiceTests()
        {
            _testHelper = TestHelper.Instance;

            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.SetupDefaultHttpContext();
            }
            mockExpressionValidationService = new Mock<IExpressionValidationService>();
            if (!_testHelper.DbContext.PerformanceCurveLibrary.Any())
            {
                _testHelper.DbContext.Add(new PerformanceCurveLibraryEntity { Id = performanceCurveLibraryId, Name = "TestPerformanceCurveLibrary" });
                _testHelper.DbContext.SaveChanges();
            }
        }

        [Fact]
        public async void ImportLibraryPerformanceCurvesFileTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                // Act
                var filePathToImport = "C:\\Users\\aborgaonkar\\Documents\\iAM\\ExportImportPerformanceLibrarySettings\\ImportExportTemplate_PerformanceCurveTest.xlsx";
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));

                var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, new UserCriteriaDTO());

                // Assert
                Assert.NotNull(result);
                Assert.Null(result.WarningMessage);
                Assert.Equal(result.PerformanceCurves.Count, 1);
                Assert.NotNull(result.PerformanceCurves[0].CriterionLibrary);
                Assert.NotNull(result.PerformanceCurves[0].Equation);
            };
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileInvalidAttributeTest()
        {
            // Setup
            mockExpressionValidationService.Setup(m => m.ValidateCriterionWithoutResults(It.IsAny<string>(), It.IsAny<UserCriteriaDTO>())).Returns(new CriterionValidationResult { IsValid = true });
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                // Act
                var filePathToImport = "C:\\Users\\aborgaonkar\\Documents\\iAM\\ExportImportPerformanceLibrarySettings\\ImportExportTemplate_PerformanceCurveTestInvalidAttribute.xlsx";
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));

                var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, new UserCriteriaDTO());

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.WarningMessage, "Error occured in import of performance curve(s): No attribute found having name AGE_.");
                Assert.Equal(result.PerformanceCurves.Count, 0);
            };
        }
    }
}
