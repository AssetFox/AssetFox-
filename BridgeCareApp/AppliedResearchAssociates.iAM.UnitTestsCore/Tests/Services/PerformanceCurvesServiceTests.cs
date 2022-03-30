using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Interfaces;
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
        }

        [Fact]
        public void ImportLibraryPerformanceCurvesFileTest()
        {
            // Setup
            performanceCurvesService = new PerformanceCurvesService(_testHelper.UnitOfWork, _testHelper.MockHubService.Object, mockExpressionValidationService.Object);

            // Add perf. curve library entry

            // Act
            var filePathToImport = "C:\\Users\\aborgaonkar\\Documents\\iAM\\ExportImportPerformanceLibrarySettings\\ImportExportTemplate_PerformanceCurveTest.xlsx";
            ExcelPackage.LicenseContext= LicenseContext.Commercial;
            var excelPackage = new ExcelPackage(File.OpenRead(filePathToImport));
            
            var result = performanceCurvesService.ImportLibraryPerformanceCurvesFile(performanceCurveLibraryId, excelPackage, new DTOs.UserCriteriaDTO());

            // Assert

        }
    }
}
