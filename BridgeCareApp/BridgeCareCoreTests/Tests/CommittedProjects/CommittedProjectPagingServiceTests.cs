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
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

namespace BridgeCareCoreTests.Tests
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
            var service = new CommittedProjectPagingService(_testUOW);

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 1,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.Equal(3, page.TotalItems);
            Assert.Equal(page.Items.Count ,request.RowsPerPage);
            Assert.True(TestDataForCommittedProjects.ValidCommittedProjects.FirstOrDefault(_ => _.Id == page.Items[0].Id) != null);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoSortColumnTreatmentIsDescendingFalse()
        {
            var service = new CommittedProjectPagingService(_testUOW);


            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "",
                sortColumn = "treatment"
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);
            var sorted = TestDataForCommittedProjects.ValidCommittedProjects.OrderBy(_ => _.Treatment).ToList();
            Assert.Equal(3, page.TotalItems);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            Assert.True(page.Items[0].Id == sorted[0].Id);
            Assert.True(page.Items[1].Id == sorted[1].Id);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoSearchItemsCountOne()
        {
            var service = new CommittedProjectPagingService(_testUOW);


            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>(),
                search = "Simple",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.Equal(3, page.TotalItems);
            Assert.Equal(2, page.Items.Count);
            Assert.True(page.Items.All(_ => _.Treatment == "Simple"));
        }

        [Fact]
        public void GetCommittedProjectPageSizeThreeAddRow()
        {
            var service = new CommittedProjectPagingService(_testUOW);

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
                RowsPerPage = 4,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    AddedRows = new List<SectionCommittedProjectDTO> { addrow}
                },
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.Equal(4, page.TotalItems);
            Assert.Equal(page.Items.Count, request.RowsPerPage);
            Assert.True(page.Items.SingleOrDefault(_ => _.Id == addrow.Id) != null);
        }

        [Fact]
        public void GetCommittedProjectPageSizetwoUpdateRow()
        {
            var service = new CommittedProjectPagingService(_testUOW);

            var updateRow = TestDataForCommittedProjects.ValidCommittedProjects[0];
            var newTreament = "updated treatment";
            updateRow.Treatment = newTreament;

            var request = new PagingRequestModel<SectionCommittedProjectDTO>()
            {
                Page = 1,
                RowsPerPage = 2,
                isDescending = false,
                SyncModel = new PagingSyncModel<SectionCommittedProjectDTO>()
                {
                    UpdateRows = new List<SectionCommittedProjectDTO> { updateRow }
                },
                search = "",
                sortColumn = ""
            };

            var page = service.GetCommittedProjectPage(TestDataForCommittedProjects.ValidCommittedProjects, request);

            Assert.Equal(3, page.TotalItems);
            Assert.True(page.Items.Count == request.RowsPerPage);
            Assert.True(page.Items.FirstOrDefault(_ => updateRow.Id == _.Id).Treatment == newTreament);
        }

        [Fact]
        public void GetSyncedDataNoChanges()
        {
            var service = new CommittedProjectPagingService(_testUOW);


            var dataSet = service.GetSyncedDataset(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id, new PagingSyncModel<SectionCommittedProjectDTO>());
            var dataIds = dataSet.Select(_ => _.Id).ToList();

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.True(TestDataForCommittedProjects.ValidCommittedProjects.Where(_ => dataIds.Contains(_.Id)).Count() ==
                TestDataForCommittedProjects.ValidCommittedProjects.Count);
        }

        [Fact]
        public void GetSyncedDataUpdateRow()
        {
            var service = new CommittedProjectPagingService(_testUOW);

            var updateRow = TestDataForCommittedProjects.ValidCommittedProjects[0];

            updateRow.Treatment = "updated treatment";

            var sync = new PagingSyncModel<SectionCommittedProjectDTO>()
            {
                UpdateRows = new List<SectionCommittedProjectDTO> { updateRow }
            };
            var dataSet = service.GetSyncedDataset(TestDataForCommittedProjects.Simulations.Single(_ => _.Name == "Test").Id,sync);
            var dataIds = dataSet.Select(_ => _.Id).ToList();

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.True(TestDataForCommittedProjects.ValidCommittedProjects.Where(_ => dataIds.Contains(_.Id)).Count() ==
                TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.True(dataSet.FirstOrDefault(_ => _.Id == updateRow.Id).Treatment == updateRow.Treatment);
        }

        [Fact]
        public void GetSyncedDataAddRow()
        {
            var service = new CommittedProjectPagingService(_testUOW);

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
            var dataIds = dataSet.Select(_ => _.Id).ToList();

            Assert.True(dataSet.Count == TestDataForCommittedProjects.ValidCommittedProjects.Count + 1);
            Assert.True(TestDataForCommittedProjects.ValidCommittedProjects.Where(_ => dataIds.Contains(_.Id)).Count() ==
                TestDataForCommittedProjects.ValidCommittedProjects.Count);
            Assert.True(dataSet.FirstOrDefault(_ => _.Id == addrow.Id) != null);
        }
    }
}
