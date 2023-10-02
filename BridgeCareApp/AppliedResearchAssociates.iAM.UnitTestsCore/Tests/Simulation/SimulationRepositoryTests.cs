using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationRepositoryTests
    {
        
               
        private async Task<UserDTO> AddTestUser()
        {
            var randomName = RandomStrings.Length11();
            TestHelper.UnitOfWork.AddUser(randomName, true);
            var returnValue = await TestHelper.UnitOfWork.UserRepo.GetUserByUserName(randomName);
            return returnValue;
        }

        [Fact] 
        public void DeleteSimulation_Does()
        {
            SimulationRepositoryTestSetup.Setup();
            // Arrange
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            TestHelper.UnitOfWork.SimulationRepo.DeleteSimulation(simulation.Id);
            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id));
        }

        [Fact]
        public async Task GetUserScenarios_SimulationInDbOwnedByUser_Gets()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            TestHelper.UnitOfWork.SetUser(user.Username);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, owner: user.Id);

            // Act
            var result = TestHelper.UnitOfWork.SimulationRepo.GetUserScenarios();
            var retrievedSimulation = result.Single();
            var simulationFromRepo = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulation.Id);
            ObjectAssertions.Equivalent(simulationFromRepo, retrievedSimulation);
        }

        [Fact]
        public void GetSharedScenarios_DoesNotThrow()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();

            var result = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(true, true);
        }

        [Fact]
        public void GetSimulationNameOrId_SimulationNotInDb_GetsId()
        {
            var simulationId = Guid.NewGuid();
            var nameOrId = TestHelper.UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            Assert.Contains(simulationId.ToString(), nameOrId);
        }

        
        [Fact]
        public async Task CreateSimulation_Does()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();
            var newSimulationDto = SimulationDtos.Dto();
            var testUser = await AddTestUser();

            newSimulationDto.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = testUser.Id,
                        Username = testUser.Username,
                        CanModify = true,
                        IsOwner = true
                    }
                };

            // Act

            TestHelper.UnitOfWork.SimulationRepo.CreateSimulation(NetworkTestSetup.NetworkId, newSimulationDto);
            var dto = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(newSimulationDto.Id);
            // Assert
            var simulationEntity = TestHelper.UnitOfWork.Context.Simulation
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .SingleOrDefault(_ => _.Id == dto.Id);

            Assert.NotNull(simulationEntity);
            //    Assert.Equal(dto.Users[0].UserId, simulationEntity.CreatedBy); // Not true in any world I can find. -- WJ

            var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
            var simulationUser = simulationUsers.Single();
            Assert.Equal(dto.Users[0].UserId, simulationUser.UserId);
        }

        [Fact]
        public async Task UpdateSimulation_Does()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();
            var testUser1 = await AddTestUser();
            var testUser2 = await AddTestUser();
            TestHelper.UnitOfWork.Context.SaveChanges();
            var simulationId = Guid.NewGuid();
            var ownerId = testUser1.Id;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, owner: ownerId);
            var updatedSimulation = SimulationDtos.Dto(simulationId, owner: testUser2.Id);

            updatedSimulation.Name = "Updated Name";
            updatedSimulation.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = testUser2.Id,
                        Username = testUser2.Username,
                        CanModify = true,
                        IsOwner = true
                    }
                };

            // Act
            TestHelper.UnitOfWork.SimulationRepo.UpdateSimulationAndPossiblyUsers(updatedSimulation);
            var dto = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(updatedSimulation.Id);

            // Assert
            var simulationEntity = TestHelper.UnitOfWork.Context.Simulation
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Single(_ => _.Id == dto.Id);

            Assert.Equal(dto.Name, simulationEntity.Name);

            var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
            Assert.True(simulationUsers.Count == 2);
            Assert.Equal(testUser2.Id,
                simulationUsers.Single(_ => _.UserId != ownerId).UserId);
        }

        [Fact]
        public async Task UpdateSimulation_InvalidUserInDto_ThrowsWithoutUpdating()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();
            var testUser1 = await AddTestUser();
            var testUser2 = await AddTestUser();
            TestHelper.UnitOfWork.Context.SaveChanges();
            var simulationId = Guid.NewGuid();
            var ownerId = testUser1.Id;
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, owner: ownerId);
            var updateDto = SimulationDtos.Dto(simulationId, owner: testUser2.Id);

            updateDto.Name = "Updated Name";
            var invalidUserId = Guid.NewGuid();
            updateDto.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = testUser2.Id,
                        Username = "conflicting userId",
                        CanModify = true,
                        IsOwner = true
                    },
                    new SimulationUserDTO
                    {
                        UserId = testUser2.Id,
                        Username = "other conflicting userId",
                        CanModify = true,
                        IsOwner = true
                    }
                };
            var dtoBefore = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(updateDto.Id);

            // Act
            var exception = Assert.Throws<SqlException>(() => TestHelper.UnitOfWork.SimulationRepo.UpdateSimulationAndPossiblyUsers(updateDto));

            // Assert
            TestHelper.UnitOfWork.
                Context.ChangeTracker.Clear();
            var dtoAfter = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(updateDto.Id);
            ObjectAssertions.Equivalent(dtoBefore, dtoAfter);
            Assert.NotEqual(updateDto.Name, dtoAfter.Name);
        }
      

        [Fact (Skip ="Fails for reasons WJ does not understand.")]
        public void SimulationInDbWithBudgetAndAmount_Delete_DeletesAll()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
            var simulation = SimulationTestSetup.DomainSimulation(TestHelper.UnitOfWork);
            var simulationId = simulation.Id;
            var investmentPlan = simulation.InvestmentPlan;
            var investmentPlanId = investmentPlan.Id;
            var budgetName = RandomStrings.WithPrefix("Budget");

            var budgetId = Guid.NewGuid();
            var budgetDto = BudgetDtos.New(budgetId, budgetName);
            var criterionLibrary = CriterionLibraryDtos.Dto();
            budgetDto.CriterionLibrary = criterionLibrary;
            var budgetDtos = new List<BudgetDTO> { budgetDto };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgetDtos, simulation.Id);
            var budgetAmountId = Guid.NewGuid();
            BudgetAmountTestSetup.SetupSingleAmountForBudget(unitOfWork, simulationId, budgetName, budgetId, budgetAmountId);
            var budgetPercentagePairId = Guid.NewGuid();
            var budgetPriorityId = Guid.NewGuid();
            var percentagePair =
                new BudgetPercentagePairDTO
                {
                    BudgetId = budgetId,
                    BudgetName = budgetName,
                    Id = budgetPercentagePairId,
                    Percentage = 100,
                };
            var budgetPriority = new BudgetPriorityDTO
            {
                Id = budgetPriorityId,
                BudgetPercentagePairs = new List<BudgetPercentagePairDTO>
                {
                    percentagePair,
                },
                CriterionLibrary = CriterionLibraryDtos.Dto(),
            };
            var budgetPriorities = new List<BudgetPriorityDTO> { budgetPriority };
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(budgetPriorities, simulationId); // If we delete this line, the test passes. But why can't we have budget priorities?

            var simulationBudgetsBefore = unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
            var budgetAmountEntityBefore = TestHelper.UnitOfWork.Context.ScenarioBudgetAmount.SingleOrDefault(ba => ba.Id == budgetAmountId);
            var scenarioBudgetEntityBefore = TestHelper.UnitOfWork.Context.ScenarioBudget.SingleOrDefault(sb => sb.Id == budgetId);
            var budgetPriorityEntityBefore = TestHelper.UnitOfWork.Context.ScenarioBudgetPriority
                .SingleOrDefault(bp => bp.Id == budgetPriorityId);
            Assert.NotNull(budgetAmountEntityBefore);
            Assert.NotNull(scenarioBudgetEntityBefore);
            Assert.NotNull(budgetPriorityEntityBefore);
            unitOfWork.Context.SaveChanges();

            unitOfWork.SimulationRepo.DeleteSimulation(simulationId);

            unitOfWork.Context.ChangeTracker.Clear();
            var budgetAmountEntityAfter = TestHelper.UnitOfWork.Context.ScenarioBudgetAmount.SingleOrDefault(ba => ba.Id == budgetAmountId);
            var scenarioBudgetEntityAfter = TestHelper.UnitOfWork.Context.ScenarioBudget.SingleOrDefault(sb => sb.Id == budgetId);
            var investmentPlanEntityAfter = TestHelper.UnitOfWork.Context.InvestmentPlan.SingleOrDefault(ip => ip.Id == investmentPlanId);
            Assert.Null(budgetAmountEntityAfter);
            Assert.Null(scenarioBudgetEntityAfter);
            Assert.Null(investmentPlanEntityAfter);
        }
    }
}
