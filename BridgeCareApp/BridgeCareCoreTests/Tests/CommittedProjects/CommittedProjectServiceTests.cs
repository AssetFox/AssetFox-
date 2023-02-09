using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using System.Data;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCoreTests.Tests
{
    public class CommittedProjectServiceTests : IClassFixture<ExcelAccess>
    {
        private IUnitOfWork _testUOW;
        private Mock<IAMContext> _mockedContext;
        private Mock<ISimulationRepository> _mockedSimulationRepo;
        private Mock<ICommittedProjectRepository> _mockCommittedProjectRepo;
        private Guid _badScenario = Guid.Parse("0c66674c-8fcb-462b-8765-69d6815e0958");
        private ExcelPackage _excelData;

        public CommittedProjectServiceTests(ExcelAccess excelData)
        {
            _excelData = excelData.ExcelData;

            var mockedTestUOW = new Mock<IUnitOfWork>();
            _mockedContext = new Mock<IAMContext>();

            var mockAssetDataRepository = new Mock<IAssetData>();
            mockAssetDataRepository.Setup(_ => _.KeyProperties).Returns(TestDataForCommittedProjects.KeyProperties);
            mockedTestUOW.Setup(_ => _.AssetDataRepository).Returns(mockAssetDataRepository.Object);

            _mockCommittedProjectRepo = new Mock<ICommittedProjectRepository>();
            _mockCommittedProjectRepo.Setup(_ => _.GetCommittedProjectsForExport(It.IsAny<Guid>()))
                .Returns<Guid>(_ => TestDataForCommittedProjects.ValidCommittedProjects
                    .Where(q => q.SimulationId == _)
                    .Select(p => (BaseCommittedProjectDTO)p)
                    .ToList());
            mockedTestUOW.Setup(_ => _.CommittedProjectRepo).Returns(_mockCommittedProjectRepo.Object);

            _mockedSimulationRepo = new Mock<ISimulationRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestEntitiesForCommittedProjects.Simulations.AsQueryable());
            _mockedSimulationRepo.Setup(_ => _.GetSimulationName(It.Is<Guid>(_ => _ != _badScenario))).Returns("Test");
            _mockedSimulationRepo.Setup(_ => _.GetSimulationName(It.Is<Guid>(_ => _ == _badScenario))).Returns<string>(null);
            mockedTestUOW.Setup(_ => _.SimulationRepo).Returns(_mockedSimulationRepo.Object);

            var mockAttributeRepository = new Mock<IAttributeRepository>();
            mockAttributeRepository.Setup(_ => _.GetAttributes()).Returns(TestDataForCommittedProjects.Attributes);
            mockedTestUOW.Setup(_ => _.AttributeRepo).Returns(mockAttributeRepository.Object);

            var mockMaintainableAssetRepository = new Mock<IMaintainableAssetRepository>();
            mockMaintainableAssetRepository.Setup(_ => _.GetAllInNetworkWithLocations(It.IsAny<Guid>()))
                .Returns(TestDataForCommittedProjects.MaintainableAssets);
            mockedTestUOW.Setup(_ => _.MaintainableAssetRepo).Returns(mockMaintainableAssetRepository.Object);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository.Setup(_ => _.GetScenarioBudgets(It.IsAny<Guid>())).Returns(TestDataForCommittedProjects.ScenarioBudgets);
            mockedTestUOW.Setup(_ => _.BudgetRepo).Returns(mockBudgetRepository.Object);

            //_testUOW = new UnitOfDataPersistenceWork(new Mock<IConfiguration>().Object, _mockedContext.Object);
            mockedTestUOW.Setup(_ => _.Context).Returns(_mockedContext.Object);
            _testUOW = mockedTestUOW.Object;
        }

        [Fact]
        public void ExportValidWithGoodData()
        {
            // Arrange
            var service = new CommittedProjectService(_testUOW);

            // Act
            var result = service.ExportCommittedProjectsFile(TestDataForCommittedProjects.SimulationId);

            // Asset
            Assert.False(string.IsNullOrEmpty(result.FileName));
            Assert.True(result.FileData.Length > 0);
            var excel = new ExcelPackage(new System.IO.MemoryStream(Convert.FromBase64String(result.FileData)));
            Assert.True(excel.Workbook.Worksheets.Count > 0);
            var cells = excel.Workbook.Worksheets[0].Cells.Value;
            Assert.NotNull(cells);
            var cellArray = (Array)cells;
            var cellArrayLength = cellArray.GetLength(0);
            var committedProjectsForThisSimulation = TestDataForCommittedProjects.ValidCommittedProjects.Where(cp => cp.SimulationId == TestDataForCommittedProjects.SimulationId).ToList();
            Assert.Equal(committedProjectsForThisSimulation.Count + 1, cellArrayLength);
        }

        [Fact]
        public void ExportHandlesInvalidScenarioId()
        {
            // Arrange
            _mockCommittedProjectRepo.Setup(_ => _.GetCommittedProjectsForExport(It.IsAny<Guid>())).Throws<RowNotInTableException>();
            var service = new CommittedProjectService(_testUOW);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ExportCommittedProjectsFile(_badScenario));
        }

        [Fact]
        public void ExportProvidesATemplateWhenNoCommittedProjectsExist()
        {
            // Arrange
            var service = new CommittedProjectService(_testUOW);

            // Act
            var result = service.ExportCommittedProjectsFile(Guid.Parse("dae1c62c-adba-4510-bfe5-61260c49ec99"));

            // Assert
            Assert.False(string.IsNullOrEmpty(result.FileName));
            Assert.True(result.FileData.Length > 0);
            var excel = new ExcelPackage(new System.IO.MemoryStream(Convert.FromBase64String(result.FileData)));
            Assert.True(excel.Workbook.Worksheets.Count > 0);
            var cells = excel.Workbook.Worksheets[0].Cells.Value;
            Assert.NotNull(cells);
            Assert.Equal(1, ((Array)cells).GetLength(0));
        }

        [Fact]
        public void ImportHandlesBadSimulationId()
        {
            // Arrange
            var service = new CommittedProjectService(_testUOW);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ImportCommittedProjectFiles(_badScenario, new ExcelPackage(), "Bad File", false));
        }

        [Fact]
        public void ImportCreatesValidRecordsWithoutNoTreatment()
        {
            // Arrange
            List<SectionCommittedProjectDTO> testInput = new List<SectionCommittedProjectDTO>();
            _mockCommittedProjectRepo.Setup(_ => _.UpsertCommittedProjects(It.IsAny<List<SectionCommittedProjectDTO>>()))
                .Callback<List<SectionCommittedProjectDTO>>(_ => {
                    testInput = _;
                });
            var service = new CommittedProjectService(_testUOW);
            const string networkKeyAttribute = "BRKEY_";
            // Act - The result is delivered through the callback
            service.ImportCommittedProjectFiles(TestDataForCommittedProjects.SimulationId, _excelData, "GoodFile", false);

            // Assert
            Assert.True(testInput.Count == 2, "Number of comitted projects is wrong");
            Assert.Equal("f286b7cf-445d-4291-9167-0f225b170cae", testInput[0].LocationKeys.Single(_ => _.Key == "ID").Value);
            Assert.True(testInput[0] is SectionCommittedProjectDTO, "Provided value is not a Section type");
            Assert.True(testInput[0].VerifyLocation(networkKeyAttribute), "Could not verify location");
            Assert.Equal(8, testInput[0].Consequences.Count);
            Assert.Equal(2023, testInput[1].Year);
        }

        [Fact]
        public void ImportCreatesValidRecordsWithNoTreatment()
        {
            // Arrange
            List<SectionCommittedProjectDTO> testInput = new List<SectionCommittedProjectDTO>();
            _mockCommittedProjectRepo.Setup(_ => _.UpsertCommittedProjects(It.IsAny<List<SectionCommittedProjectDTO>>()))
                .Callback<List<SectionCommittedProjectDTO>>(_ => {
                    testInput = _;
                });
            var service = new CommittedProjectService(_testUOW);
            const string networkKeyAttribute = "BRKEY_";
            // Act - The result is delivered through the callback
            service.ImportCommittedProjectFiles(TestDataForCommittedProjects.SimulationId, _excelData, "GoodFileWithNoTreatment", true);

            // Assert
            Assert.True(testInput.Count == 3, "Number of comitted projects is wrong");
            Assert.Equal("cf28e62e-0a02-4195-8d28-5cdb9646dd58", testInput[1].LocationKeys.Single(_ => _.Key == "ID").Value);
            Assert.True(testInput[1] is SectionCommittedProjectDTO, "Provided value is not a Section type");
            Assert.True(testInput[1].VerifyLocation(networkKeyAttribute), "Could not verify location");
            Assert.Equal(8, testInput[1].Consequences.Count);
            Assert.Equal(2023, testInput[1].Year);
            Assert.Equal(TreatmentCategory.CapacityAdding, testInput[1].Category);
            Assert.True(testInput.Any(_ => _.Treatment == "No Treatment"), "No Treatment was not created");
            var noTreatment = testInput.First(_ => _.Treatment == "No Treatment");
            Assert.Equal(0, noTreatment.Cost);
            Assert.Equal(2022, noTreatment.Year);
        }
    }

    // Only read the excel file once to prevent conflicts
    public class ExcelAccess : IDisposable
    {
        public ExcelPackage ExcelData { get; set; }

        public ExcelAccess()
        {
            var fileLocation = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files", "TestCommittedProjects_Good.xlsx");
            using (var stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read))
            {
                ExcelData = new ExcelPackage(stream);
            }
        }

        public void Dispose()
        {
            ExcelData.Dispose();
        }
    }
}
