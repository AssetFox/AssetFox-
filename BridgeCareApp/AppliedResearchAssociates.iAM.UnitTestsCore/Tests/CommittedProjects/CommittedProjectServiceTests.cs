using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using BridgeCareCore.Services;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectServiceTests
    {
        private IUnitOfWork _testUOW;
        private Mock<IAMContext> _mockedContext;

        public CommittedProjectServiceTests()
        {
            var mockedTestUOW = new Mock<IUnitOfWork>();
            _mockedContext = new Mock<IAMContext>();

            var mockAssetDataRepository = new Mock<IAssetData>();
            mockAssetDataRepository.Setup(_ => _.KeyProperties).Returns(TestDataForCommittedProjects.KeyProperties);
            mockedTestUOW.Setup(_ => _.AssetDataRepository).Returns(mockAssetDataRepository.Object);

            var mockCommittedProjectRepository = new Mock<ICommittedProjectRepository>();
            mockCommittedProjectRepository.Setup(_ => _.GetCommittedProjectsForExport(It.IsAny<Guid>()))
                .Returns(TestDataForCommittedProjects.ValidCommittedProjects);
            mockedTestUOW.Setup(_ => _.CommittedProjectRepo).Returns(mockCommittedProjectRepository.Object);

            var mockSimulationRepository = new Mock<ISimulationRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestDataForCommittedProjects.Simulations.AsQueryable());
            mockSimulationRepository.Setup(_ => _.GetSimulationName(It.IsAny<Guid>())).Returns("Test");
            mockedTestUOW.Setup(_ => _.SimulationRepo).Returns(mockSimulationRepository.Object);

            var mockAttributeRepository = new Mock<IAttributeRepository>();
            mockAttributeRepository.Setup(_ => _.GetAttributes()).Returns(TestDataForCommittedProjects.Attributes);
            mockedTestUOW.Setup(_ => _.AttributeRepo).Returns(mockAttributeRepository.Object);

            var mockMaintainableAssetRepository = new Mock<IMaintainableAssetRepository>();
            mockMaintainableAssetRepository.Setup(_ => _.GetAllInNetworkWithAssignedDataAndLocations(It.IsAny<Guid>()))
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
        public void CreatesAValidExport()
        {
            // Arrange
            var service = new CommittedProjectService(_testUOW);

            // Act
            var result = service.ExportCommittedProjectsFile(Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));

            // Asset
            Assert.False(string.IsNullOrEmpty(result.FileName));
            Assert.True(result.FileData.Length > 0);
        }
    }
}
