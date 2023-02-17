using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Services;
using BridgeCareCoreTests.Helpers;
using Xunit;
using Moq;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;

namespace BridgeCareCoreTests.Tests.Treatment
{
    public class TreatmentPagingServiceTests
    {
        public TreatmentPagingService CreatePagingService(Mock<IUnitOfWork> unitOfWork)
        {
            var service = new TreatmentPagingService(unitOfWork.Object);
            return service;
        }

        [Fact]
        public void GetScenarioDataset_EverythingIsEmpty_Empty()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            selectableTreatmentRepo.Setup(s => s.GetScenarioSelectableTreatments(simulationId)).ReturnsEmptyList();
            var syncModel = new PagingSyncModel<TreatmentDTO>();

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            Assert.Empty(result);
        }


        [Fact]
        public void GetScenarioDataset_RepoReturnsARow_ReturnsRow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetScenarioSelectableTreatments(simulationId)).ReturnsList(dto);
            var syncModel = new PagingSyncModel<TreatmentDTO>();

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            ObjectAssertions.Equivalent(dtoClone, returnedDto);
        }

        [Fact]
        public void GetScenarioDataset_LibraryIdInRequest_NullPropertiesInDto_DoesNotThrow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetSelectableTreatments(libraryId)).ReturnsList(dto);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            budgetRepo.Setup(b => b.GetScenarioBudgets(simulationId)).ReturnsList(budget);
            var syncModel = new PagingSyncModel<TreatmentDTO>
            {
                LibraryId = libraryId,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            ObjectAssertions.EquivalentExcluding(dtoClone, returnedDto, x => x.Id, x => x.BudgetIds);
            var returnedBudgetId = returnedDto.BudgetIds.Single();
            Assert.Equal(budgetId, returnedBudgetId);
            Assert.NotEqual(treatmentId, returnedDto.Id);
        }

        [Fact]
        public void GetScenarioDataset_LibraryIdInRequest_DtoHasNullChildren_DoesNotThrow()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var costId = Guid.NewGuid();
            var consequenceId = Guid.NewGuid();
            var cost = TreatmentCostDtos.Dto(costId);
            var consequence = TreatmentConsequenceDtos.Dto(consequenceId);
            dto.Costs = new List<TreatmentCostDTO> { cost };
            dto.Consequences = new List<TreatmentConsequenceDTO> { consequence };
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetSelectableTreatments(libraryId)).ReturnsList(dto);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            budgetRepo.Setup(b => b.GetScenarioBudgets(simulationId)).ReturnsList(budget);
            var syncModel = new PagingSyncModel<TreatmentDTO>
            {
                LibraryId = libraryId,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            var returnedBudgetId = returnedDto.BudgetIds.Single();
            Assert.Equal(budgetId, returnedBudgetId);
            Assert.NotEqual(treatmentId, returnedDto.Id);
        }



        [Fact]
        public void GetScenarioDataset_LibraryIdInRequest_IdsAreRegenerated()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var selectableTreatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var service = CreatePagingService(unitOfWork);
            var simulationId = Guid.NewGuid();
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = TreatmentDtos.Dto(treatmentId);
            var criterionLibraryId = Guid.NewGuid();
            var criterionLibrary = CriterionLibraryDtos.Dto(criterionLibraryId);
            dto.CriterionLibrary = criterionLibrary;
            var costId = Guid.NewGuid();
            var costEquationId = Guid.NewGuid();
            var costCriterionLibraryId = Guid.NewGuid();
            var consequenceId = Guid.NewGuid();
            var consequenceEquationId = Guid.NewGuid();
            var consequenceCriterionLibraryId = Guid.NewGuid();
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(costId, costEquationId, costCriterionLibraryId);
            var consequence = TreatmentConsequenceDtos.WithEquationAndCriterionLibrary(consequenceId, consequenceEquationId, consequenceCriterionLibraryId);
            cost.Equation = EquationDtos.AgePlus1();
            
            dto.Costs = new List<TreatmentCostDTO> { cost };
            dto.Consequences = new List<TreatmentConsequenceDTO> { consequence };
            var dtoClone = TreatmentDtos.Dto(treatmentId);
            selectableTreatmentRepo.Setup(s => s.GetSelectableTreatments(libraryId)).ReturnsList(dto);
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            budgetRepo.Setup(b => b.GetScenarioBudgets(simulationId)).ReturnsList(budget);
            var syncModel = new PagingSyncModel<TreatmentDTO>
            {
                LibraryId = libraryId,
            };

            var result = service.GetSyncedScenarioDataSet(simulationId, syncModel);

            var returnedDto = result.Single();
            Assert.NotEqual(criterionLibraryId, returnedDto.CriterionLibrary.Id);
            var returnedBudgetId = returnedDto.BudgetIds.Single();
            Assert.Equal(budgetId, returnedBudgetId);
            Assert.NotEqual(treatmentId, returnedDto.Id);
            var returnedCost = returnedDto.Costs.Single();
            Assert.NotEqual(costId, returnedCost.Id);
            Assert.NotEqual(costEquationId, returnedCost.Equation.Id);
            Assert.NotEqual(costCriterionLibraryId, returnedCost.CriterionLibrary.Id);
            var returnedConsequence = returnedDto.Consequences.Single();
            Assert.NotEqual(returnedConsequence.Id, consequenceId);
            Assert.NotEqual(returnedConsequence.Equation.Id, consequenceEquationId);
            Assert.NotEqual(returnedConsequence.CriterionLibrary.Id, consequenceCriterionLibraryId);
        }
    }
}
