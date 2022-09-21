using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Moq;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using BridgeCareCore.Interfaces;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using BridgeCareCore.Logging;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectPagingServiceTests
    {
        private IUnitOfWork _testUOW;
        private Mock<IAMContext> _mockedContext;
        private Mock<ISimulationRepository> _mockedSimulationRepo;
        private Mock<ICommittedProjectRepository> _mockCommittedProjectRepo;

        public CommittedProjectPagingServiceTests()
        {
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
            _mockCommittedProjectRepo.Setup(_ => _.GetSectionCommittedProjectDTOs(It.IsAny<Guid>())).Returns(TestDataForCommittedProjects.ValidCommittedProjects);
            mockedTestUOW.Setup(_ => _.CommittedProjectRepo).Returns(_mockCommittedProjectRepo.Object);

            _mockedSimulationRepo = new Mock<ISimulationRepository>();
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestDataForCommittedProjects.Simulations.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioBudget, TestDataForCommittedProjects.ScenarioBudgetEntities.AsQueryable());
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
        public void GetCommittedProjectPageSizeOneSuccess()
        {
            var service = new CommittedProjectService(_testUOW);

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 1,
                isDescending = false,
                PagingSync = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.True(page.TotalItems == 2);
            Assert.Equal(page.Items.Count ,request.RowsPerPage);
            Assert.Equal(page.Items[0].Id, TestDataForCommittedProjects.ValidCommittedProjects[1].Id);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoSortColumnTreatmentIsDescendingFalse()
        {
            var service = new CommittedProjectService(_testUOW);


            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                PagingSync = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = "treatment"
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.True(page.TotalItems == 2);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            Assert.True(page.Items[0].Treatment == "Simple");
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoSearchItemsCountOne()
        {
            var service = new CommittedProjectService(_testUOW);


            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                PagingSync = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "Simple",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.True(page.TotalItems == 2);
            Assert.True(page.Items.Count == 1);
            Assert.True(page.Items[0].Treatment == "Simple");
        }

        [Fact]
        public void GetCommittedProjectPageSizeThreeAddRow()
        {
            var service = new CommittedProjectService(_testUOW);

            var addrow = new SectionCommittedProjectDTO()
            {
                Id = Guid.NewGuid(),
                Year = 2022,
                Treatment = "Something",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Cost = 10000,
                SimulationId = TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "f286b7cf-445d-4291-9167-0f225b170cae" },
                    { "BRKEY_", "1" },
                    { "BMSID", "12345678" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "+3"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            };

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 3,
                isDescending = false,
                PagingSync = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    AddedRows = new List<SectionCommittedProjectDTO> { addrow}
                },
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.True(page.TotalItems == 3);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            Assert.Equal(page.Items[2].Id, addrow.Id);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoUpdateRow()
        {
            var service = new CommittedProjectService(_testUOW);

            var updateRow = TestDataForCommittedProjects.ValidCommittedProjects[0];

            updateRow.Treatment = "updated treatment";

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                PagingSync = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    UpdateRows = new List<SectionCommittedProjectDTO> { updateRow }
                },
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.True(page.TotalItems == 2);
            Assert.True(page.Items.Count == request.RowsPerPage);
            Assert.True(page.Items[1].Treatment == updateRow.Treatment);
        }

        [Fact]
        public void GetSyncedDataNoChanges()
        {
            var service = new CommittedProjectService(_testUOW);


            var dataSet = service.GetSyncedDataset(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id, new PagingSyncModel<SectionCommittedProjectDTO>());

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.Equal(dataSet[0].Id, TestDataForCommittedProjects.ValidCommittedProjects[0].Id);
            Assert.Equal(dataSet[1].Id, TestDataForCommittedProjects.ValidCommittedProjects[1].Id);
        }

        [Fact]
        public void GetSyncedDataUpdateRow()
        {
            var service = new CommittedProjectService(_testUOW);

            var updateRow = TestDataForCommittedProjects.ValidCommittedProjects[0];

            updateRow.Treatment = "updated treatment";

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                UpdateRows = new List<SectionCommittedProjectDTO> { updateRow }
            };
            var dataSet = service.GetSyncedDataset(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id,sync);

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.Equal(dataSet[0].Id, TestDataForCommittedProjects.ValidCommittedProjects[0].Id);
            Assert.Equal(dataSet[1].Id, TestDataForCommittedProjects.ValidCommittedProjects[1].Id);
            Assert.True(dataSet[0].Treatment == updateRow.Treatment);
        }

        [Fact]
        public void GetSyncedDataAddRow()
        {
            var service = new CommittedProjectService(_testUOW);

            var addrow = new SectionCommittedProjectDTO()
            {
                Id = Guid.NewGuid(),
                Year = 2022,
                Treatment = "Something",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Cost = 10000,
                SimulationId = TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "f286b7cf-445d-4291-9167-0f225b170cae" },
                    { "BRKEY_", "1" },
                    { "BMSID", "12345678" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "+3"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            };

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                AddedRows = new List<SectionCommittedProjectDTO> { addrow }
            };
            var dataSet = service.GetSyncedDataset(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id, sync);

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count + 1);
            Assert.Equal(dataSet[0].Id, TestDataForCommittedProjects.ValidCommittedProjects[0].Id);
            Assert.Equal(dataSet[1].Id, TestDataForCommittedProjects.ValidCommittedProjects[1].Id);
            Assert.Equal(dataSet[2].Id, addrow.Id);
        }
    }
}
