using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Reporting
{
    public class ReportGeneratorTests
    {
        private ReportLookupLibrary _testReportLibrary;
        private UnitOfDataPersistenceWork _testRepo;
        private DictionaryBasedReportGenerator _generator;
        private TestHelper _testHelper => TestHelper.Instance;

        public ReportGeneratorTests()
        {
            var mockedContext = new Mock<IAMContext>();
            var testReportIndexList = TestDataForReportIndex.SimpleRepo().AsQueryable();
            var mockedReportIndexSet = MockedContextBuilder.AddDataSet(mockedContext, _ => _.ReportIndex, testReportIndexList);
            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, mockedContext.Object);
            _testRepo = mockedRepo;

            _testReportLibrary = new ReportLookupLibrary(TestDataForReportIndex.SimpleReportLibrary());

            _generator = new DictionaryBasedReportGenerator(_testRepo, _testReportLibrary, _testHelper.MockHubService.Object);
        }

        [Fact]
        public async Task GeneratorCanGenerateReportInLibrary()
        {
            // Arrange
            string goodReport = "Test Report File";

            // Act
            IReport report = await _generator.Generate(goodReport);

            // Assert
            Assert.Equal(ReportType.File, report.Type);
            Assert.Equal(goodReport, report.ReportTypeName);
            Assert.Equal(0, report.Errors.Count());
        }

        [Fact]
        public async Task GeneratorReturnsFailureReportWhenReportNotInLibrary()
        {
            // Arrange
            string badReport = "Some missing report";

            // Act
            IReport report = await _generator.Generate(badReport);

            // Assert
            Assert.Equal(ReportType.HTML, report.Type);
            Assert.Equal("Failure Report", report.ReportTypeName);
            Assert.True(report.Errors.Count() > 0);
        }

        [Fact]
        public void GeneratorReturnsAllScenarioReports()
        {
            // Arrange
            Guid scenarioId = new Guid("be82f095-c108-4ab7-af7e-cb7ecd18ede2");

            // Act
            var reportList = _generator.GetAllReportsForScenario(scenarioId);

            // Assert
            Assert.Equal(2, reportList.Count());
        }

        [Fact]
        public void GeneratorHandlesAScenarioWithoutReports()
        {
            // Arrange
            Guid scenarioId = new Guid("be82f095-aaaa-aaaa-aaaa-cb7ecd18ede2"); // Should not exist in demo repo

            // Act
            var reportList = _generator.GetAllReportsForScenario(scenarioId);

            // Assert
            Assert.Empty(reportList);
        }

        [Fact]
        public async Task GeneratorSuccessfullyReturnsASpecificReport()
        {
            // Arrange
            Guid reportId = new Guid("b32ecb1e-297f-4caa-9608-f28ab61cbd91");

            // Act
            var report = await _generator.GetExisting(reportId);

            // Assert
            Assert.Equal("0951aaad-eddd-462d-ab8d-99ed3829019f", report.SimulationID.ToString());
        }

        [Fact]
        public async Task GeneratorHandlesNotFindingASpecificReport()
        {
            // Arrange
            Guid reportId = new Guid("be82f095-aaaa-aaaa-aaaa-cb7ecd18ede2"); // Should not exist in demo repo

            // Act
            var report = await _generator.GetExisting(reportId);
        }
    }

    
}
