using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CashFlowRule;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class CashFlowPagingServiceTests
    {
        private CashFlowPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new CashFlowPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }
        [Fact]
        public void GetSyncedScenarioDataset_RowToDeleteNotInDatabase_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { Guid.NewGuid() },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToUpdate_ReturnsUpdatedRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            dto2.Name = "Updated name";
            var dto3 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            dto3.Name = "Updated name";
            var dtos = new List<CashFlowRuleDTO> { dto1 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                UpdateRows = new List<CashFlowRuleDTO> { dto2 },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto3, resultDto);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RowToDelete_DeletesRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var dto = CashFlowRuleDtos.Rule(ruleId);
            var dtos = new List<CashFlowRuleDTO> { dto };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { ruleId },
            };

            var result = pagingService.GetSyncedScenarioDataSet(
                scenarioId, syncModel);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataset_RepoReturnsRule_ReturnedInResult()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, criterionLibraryId, distributionRuleId);
            var dto2 = CashFlowRuleDtos.Rule(ruleId, criterionLibraryId, distributionRuleId);
            var dtos = new List<CashFlowRuleDTO> { dto1 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();

            var result = pagingService.GetSyncedScenarioDataSet(scenarioId, syncModel);

            var resultDto = result.Single();
            ObjectAssertions.Equivalent(dto2, resultDto);
        }

        [Fact]
        public void GetScenarioPage_TooManyRulesForPage_Truncates()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto1Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2 = CashFlowRuleDtos.Rule();
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 1,
            };

            var result = pagingService.GetScenarioPage(scenarioId, pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto1Clone, item);
        }

        [Fact]
        public void GetScenarioPage_SortByName_Throws()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            dto2.Name = "Aaaaaa";
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 1,
                sortColumn = "Name",
            };

            Assert.Throws<NotImplementedException>(() => pagingService.GetScenarioPage(scenarioId, pagingRequest));
        }

        [Fact]
        public void GetScenarioPage_Search_Throws()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            dto2.Name = "Aaaaaa";
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 1,
                search = "Name",
            };

            Assert.Throws<NotImplementedException>(() => pagingService.GetScenarioPage(scenarioId, pagingRequest));
        }

        [Fact]
        public void GetScenarioPage_RequestSecondPage_Expected()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var scenarioId = Guid.NewGuid();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule();
            var dto2 = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dto2Clone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId);
            var dtos = new List<CashFlowRuleDTO> { dto1, dto2 };
            repo.Setup(r => r.GetScenarioCashFlowRules(scenarioId)).Returns(dtos);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>();
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                SyncModel = syncModel,
                RowsPerPage = 1,
                Page = 2,
            };

            var result = pagingService.GetScenarioPage(scenarioId, pagingRequest);

            Assert.Equal(2, result.TotalItems);
            var item = result.Items.Single();
            ObjectAssertions.Equivalent(dto2Clone, item);
        }

        [Fact]
        public void GetSyncedLibraryDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedLibraryDataset_EmptyLibraryWithRowForDeletion_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { Guid.NewGuid() },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedLibraryDataset_RowInDbIsRowForDeletion_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rowId = Guid.NewGuid();
            var dto = CashFlowRuleDtos.Rule(rowId);
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(dto);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { rowId },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedLibraryDataset_RowInDbIsUpdatedRow_Updates()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rowId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var dto1 = CashFlowRuleDtos.Rule(rowId, distributionRuleId, criterionLibraryId);
            var updatedDto = CashFlowRuleDtos.Rule(rowId, distributionRuleId, criterionLibraryId);
            updatedDto.Name = "Updated Name";
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(dto1);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                UpdateRows = new List<CashFlowRuleDTO> { updatedDto },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedDto = result.Single();
            Assert.Equal("Updated Name", returnedDto.Name);
        }

        [Fact]
        public void GetSyncedLibraryDataset_RowToAdd_Adds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsEmptyList();
            var ruleId = Guid.NewGuid();
            var criterionLibraryId = Guid.NewGuid();
            var distributionRuleId = Guid.NewGuid();
            var rowToAdd = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId); ;
            var rowToAddClone = CashFlowRuleDtos.Rule(ruleId, distributionRuleId, criterionLibraryId); ;
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                AddedRows = new List<CashFlowRuleDTO> { rowToAdd },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var addedRow = result.Single();
            ObjectAssertions.Equivalent(rowToAddClone, addedRow);
        }

        [Fact]
        public void GetSyncedScenarioDataset_EmptyNewLibrary_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithAddedRow_HasRowWithFreshIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule = CashFlowRuleDtos.Rule(Guid.Empty, Guid.Empty, Guid.Empty);
            rule.CriterionLibrary = null;
            var ruleName = rule.Name;
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                AddedRows = new List<CashFlowRuleDTO> { rule },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedRule = result.Single();
            Assert.Equal(ruleName, returnedRule.Name);
            Assert.NotEqual(Guid.Empty, returnedRule.Id);
            Assert.Null(returnedRule.CriterionLibrary);
            Assert.NotEqual(Guid.Empty, returnedRule.CashFlowDistributionRules.Single().Id);
        }

        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithAddedRowWithCriterionLibrary_HasRowWithFreshIds()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule = CashFlowRuleDtos.Rule(Guid.Empty, Guid.Empty, Guid.Empty); ;
            var ruleName = rule.Name;
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                AddedRows = new List<CashFlowRuleDTO> { rule },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            var returnedRule = result.Single();
            Assert.Equal(ruleName, returnedRule.Name);
            Assert.NotEqual(Guid.Empty, returnedRule.Id);
            Assert.NotEqual(Guid.Empty, returnedRule.CriterionLibrary.Id);
            Assert.NotEqual(Guid.Empty, returnedRule.CashFlowDistributionRules.Single().Id);
        }


        [Fact]
        public void GetSyncedLibraryDataset_NewLibraryWithDeletedRow_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
                RowsForDeletion = new List<Guid> { Guid.NewGuid() },
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = true,
                Library = library,
                SyncModel = syncModel,
            };

            var result = pagingService.GetSyncedLibraryDataset(upsertRequest);

            Assert.Empty(result);
        }

        [Fact]
        public void GetLibraryPage_NumberOfRowsGoesBeyondPageSize_TruncatesReturnedList()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule1 = CashFlowRuleDtos.Rule();
            var rule2 = CashFlowRuleDtos.Rule();
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(rule1, rule2);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedRule = result.Items.Single();
            ObjectAssertions.Equivalent(rule1, returnedRule);
        }

        [Fact]
        public void GetLibraryPage_Sort_Throws()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule1 = CashFlowRuleDtos.Rule();
            var rule2 = CashFlowRuleDtos.Rule();
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(rule1, rule2);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
                sortColumn = "Name",
            };

            Assert.Throws<NotImplementedException>(() => pagingService.GetLibraryPage(libraryId, pagingRequest));
        }

        [Fact]
        public void GetLibraryPage_Search_Throws()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule1 = CashFlowRuleDtos.Rule();
            var rule2 = CashFlowRuleDtos.Rule();
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(rule1, rule2);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                Page = 1,
                RowsPerPage = 1,
                SyncModel = syncModel,
                search = "Name",
            };

            Assert.Throws<NotImplementedException>(() => pagingService.GetLibraryPage(libraryId, pagingRequest));
        }

        [Fact]
        public void GetLibraryPage2_NumberOfRowsGoesBeyondPageSize_SkipsPage1()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var repo = CashFlowRuleRepositoryMocks.DefaultMock(unitOfWork);
            var pagingService = CreatePagingService(unitOfWork);
            var libraryId = Guid.NewGuid();
            var library = CashFlowRuleLibraryDtos.Empty(libraryId);
            var rule1 = CashFlowRuleDtos.Rule();
            var rule2 = CashFlowRuleDtos.Rule();
            repo.Setup(r => r.GetCashFlowRulesByLibraryId(libraryId)).ReturnsList(rule1, rule2);
            var syncModel = new PagingSyncModel<CashFlowRuleDTO>
            {
            };
            var upsertRequest = new LibraryUpsertPagingRequestModel<CashFlowRuleLibraryDTO, CashFlowRuleDTO>
            {
                IsNewLibrary = false,
                Library = library,
                SyncModel = syncModel,
            };
            var pagingRequest = new PagingRequestModel<CashFlowRuleDTO>
            {
                Page = 2,
                RowsPerPage = 1,
                SyncModel = syncModel,
            };

            var result = pagingService.GetLibraryPage(libraryId, pagingRequest);
            Assert.Equal(2, result.TotalItems);
            var returnedRule = result.Items.Single();
            ObjectAssertions.Equivalent(rule2, returnedRule);
        }
    }
}
