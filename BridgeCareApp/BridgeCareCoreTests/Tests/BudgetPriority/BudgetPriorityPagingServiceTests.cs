using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Moq;
using Xunit;

namespace BridgeCareCoreTests.Tests.BudgetPriority
{
    public class BudgetPriorityPagingServiceTests
    {
        private BudgetPriorityPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new BudgetPriorityPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetSyncedScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).Returns(new List<BudgetPriorityDTO>());
            budgetRepo.Setup(br => br.GetScenarioSimpleBudgetDetails(simulationId)).Returns(new List<SimpleBudgetDetailDTO>());
            var service = CreatePagingService(unitOfWork);
            var dtos = new List<BudgetPriorityDTO>();
            var request = new PagingSyncModel<BudgetPriorityDTO>();

            // Act
            var result = service
                .GetSyncedScenarioDataSet(simulationId, request);

            Assert.Empty(result);
        }

        [Fact]
        public void GetSyncedScenarioDataset_ReposReturnBudgetAndPriority_CreatesPercentagePair()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var simpleBudgetDto = SimpleBudgetDetailDtos.Dto(budgetId);
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto);
            budgetRepo.Setup(br => br.GetScenarioSimpleBudgetDetails(simulationId)).ReturnsList(simpleBudgetDto);
            var service = CreatePagingService(unitOfWork);
            var dtos = new List<BudgetPriorityDTO>();
            var request = new PagingSyncModel<BudgetPriorityDTO>();

            // Act
            var result = service
                .GetSyncedScenarioDataSet(simulationId, request);

            var resultDto = result.Single();
            var expectedPercentagePair = new BudgetPercentagePairDTO
            {
                BudgetId = budgetId,
                BudgetName = "Budget",
                Percentage = 100,
            };
            var expected = new BudgetPriorityDTO
            {
                Id = scenarioBudgetPriorityId,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO> { expectedPercentagePair },
                CriterionLibrary = null,
                Year = null,
                PriorityLevel = 0,
            };
            ObjectAssertions.EquivalentExcluding(expected, resultDto, x => x.BudgetPercentagePairs[0].Id);
        }

        [Fact]
        public void GetSyncedScenarioDataset_PercentagePairDoesNotCorrespondToABudget_RemovesPercentagePair()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var simpleBudgetDto = SimpleBudgetDetailDtos.Dto(budgetId);
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            var percentagePair = new BudgetPercentagePairDTO
            {
                BudgetName = "Nonexistent budget",
                BudgetId = Guid.NewGuid(),
            };
            budgetPriorityDto.BudgetPercentagePairs.Add(percentagePair);
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto);
            budgetRepo.Setup(br => br.GetScenarioSimpleBudgetDetails(simulationId)).ReturnsList(simpleBudgetDto);
            var service = CreatePagingService(unitOfWork);
            var dtos = new List<BudgetPriorityDTO>();
            var request = new PagingSyncModel<BudgetPriorityDTO>();

            // Act
            var result = service
                .GetSyncedScenarioDataSet(simulationId, request);

            var resultDto = result.Single();
            var expectedPercentagePair = new BudgetPercentagePairDTO
            {
                BudgetId = budgetId,
                BudgetName = "Budget",
                Percentage = 100,
            };
            var expected = new BudgetPriorityDTO
            {
                Id = scenarioBudgetPriorityId,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO> { expectedPercentagePair },
                CriterionLibrary = null,
                Year = null,
                PriorityLevel = 0,
            };
            ObjectAssertions.EquivalentExcluding(expected, resultDto, x => x.BudgetPercentagePairs[0].Id);
        }


        [Fact]
        public void GetScenarioPage_Search_ThinDto_DoesNotThrow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto);
            var service = CreatePagingService(unitOfWork);
            var syncModel = new PagingSyncModel<BudgetPriorityDTO>();
            var request = new PagingRequestModel<BudgetPriorityDTO>
            {
                search = "name",
            };

            var result = service.GetScenarioPage(simulationId, request);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }

        [Fact]
        public void GetScenarioPage_SearchByYear_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            var criterionLibraryId = Guid.NewGuid();
            var criterionLibraryDto = CriterionLibraryDtos.Dto(criterionLibraryId, "mergedExpression");
            var criterionLibraryDtoClone = CriterionLibraryDtos.Dto(criterionLibraryId, "mergedExpression");
            var criterionLibraryDto2 = CriterionLibraryDtos.Dto(null, "not found");
            var budgetPriorityDto2 = BudgetPriorityDtos.New();
            budgetPriorityDto.CriterionLibrary = criterionLibraryDto;
            budgetPriorityDto2.CriterionLibrary = criterionLibraryDto2;
            budgetPriorityDto.Year = 2023;
            budgetPriorityDto2.Year = 2024;
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto, budgetPriorityDto2);
            var service = CreatePagingService(unitOfWork);
            var syncModel = new PagingSyncModel<BudgetPriorityDTO>();
            var request = new PagingRequestModel<BudgetPriorityDTO>
            {
                search = "2023",
            };

            var result = service.GetScenarioPage(simulationId, request);
            var expectedItem = new BudgetPriorityDTO
            {
                Year = 2023,
                CriterionLibrary = criterionLibraryDtoClone,
                Id = budgetPriorityDto.Id,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
            };
            var expected = new PagingPageModel<BudgetPriorityDTO>
            {
                TotalItems = 1,
                Items = new List<BudgetPriorityDTO> { expectedItem },
            };
            ObjectAssertions.Equivalent(expected, result);
        }

        [Fact]
        public void GetScenarioPage_SearchByPriority_Does()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            var criterionLibraryId = Guid.NewGuid();
            var budgetPriorityDto2 = BudgetPriorityDtos.New();
            budgetPriorityDto.PriorityLevel = 80;
            budgetPriorityDto2.PriorityLevel = 90;
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto, budgetPriorityDto2);
            var service = CreatePagingService(unitOfWork);
            var syncModel = new PagingSyncModel<BudgetPriorityDTO>();
            var request = new PagingRequestModel<BudgetPriorityDTO>
            {
                search = "8",
            };

            var result = service.GetScenarioPage(simulationId, request);
            var expectedItem = new BudgetPriorityDTO
            {
                Id = budgetPriorityDto.Id,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
                PriorityLevel = 80,
            };
            var expected = new PagingPageModel<BudgetPriorityDTO>
            {
                TotalItems = 1,
                Items = new List<BudgetPriorityDTO> { expectedItem },
            };
            ObjectAssertions.Equivalent(expected, result);
        }
        [Fact]
        public void GetScenarioPage_Search_SearchesMergedCriteriaExpression()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetPriorityRepo = BudgetPriorityRepositoryMocks.DefaultMock(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var scenarioBudgetPriorityId = Guid.NewGuid();
            var budgetPriorityDto = BudgetPriorityDtos.New(scenarioBudgetPriorityId);
            var criterionLibraryId = Guid.NewGuid();
            var criterionLibraryDto = CriterionLibraryDtos.Dto(criterionLibraryId, "mergedExpression");
            var criterionLibraryDtoClone = CriterionLibraryDtos.Dto(criterionLibraryId, "mergedExpression");
            var criterionLibraryDto2 = CriterionLibraryDtos.Dto(null, "not found");
            var budgetPriorityDto2 = BudgetPriorityDtos.New();
            budgetPriorityDto.CriterionLibrary = criterionLibraryDto;
            budgetPriorityDto2.CriterionLibrary = criterionLibraryDto2;
            budgetPriorityRepo.Setup(b => b.GetScenarioBudgetPriorities(simulationId)).ReturnsList(budgetPriorityDto, budgetPriorityDto2);
            var service = CreatePagingService(unitOfWork);
            var syncModel = new PagingSyncModel<BudgetPriorityDTO>();
            var request = new PagingRequestModel<BudgetPriorityDTO>
            {
                search = "merge",
            };

            var result = service.GetScenarioPage(simulationId, request);
            var expectedItem = new BudgetPriorityDTO
            {
                CriterionLibrary = criterionLibraryDtoClone,
                Id = budgetPriorityDto.Id,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>(),
            };
            var expected = new PagingPageModel<BudgetPriorityDTO>
            {
                TotalItems = 1,
                Items = new List<BudgetPriorityDTO> { expectedItem},                
            };
            ObjectAssertions.Equivalent(expected, result);
        }
    }
}
