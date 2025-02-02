using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.TestHelpers.Assertions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
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
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);

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
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, owner: user.Id);

            // Act
            var result = TestHelper.UnitOfWork.SimulationRepo.GetUserScenarios();
            var retrievedSimulation = result.Single();
            var simulationFromRepo = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(simulation.Id);
            ObjectAssertions.Equivalent(simulationFromRepo, retrievedSimulation);
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
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, owner: ownerId);
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
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, owner: ownerId);
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


        [Fact]
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

        // [Fact]
        [Fact(Skip = "Fails. Keeping around until related discussion is complete.")]
        public async Task FailureInASingleTest()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            // Set up a network with maintainable assets
            Guid networkId = Guid.NewGuid();
            var maintainableAssets = new List<MaintainableAsset>();
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, maintainableAssets, networkId, TestAttributeIds.CulvDurationNId);
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            // Issue here is that the code lets us create a simulation owned by a nonexistent user.
            var nonexistentUserId = Guid.NewGuid();
            var simulation = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"), "Test Simulation", nonexistentUserId, networkId);
            // changing the owner Id to user.Id above causes this to pass.

            // Arrange
            var simulation2 = SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);

            // Act
            //  TestHelper.UnitOfWork.SimulationRepo.DeleteSimulation(simulation.Id);
            TestHelper.UnitOfWork.SimulationRepo.DeleteSimulation(simulation2.Id);
        }

        [Fact]
        public void GetSimulationInNetwork_Does()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            CalculatedAttributeTestSetup.CreateDefaultCalculatedAttributeLibrary(TestHelper.UnitOfWork);
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(TestHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeDtoDomainMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);
            var networkName = RandomStrings.WithPrefix("Network");
            var assetList = new List<MaintainableAsset>();
            var network = NetworkTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, assetList);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var simulationDto = new SimulationDTO
            {
                Id = simulationId,
                NetworkId = network.Id,
                Name = simulationName,
            };
            TestHelper.UnitOfWork.SimulationRepo.CreateSimulation(network.Id, simulationDto);
            SimulationAnalysisDetailTestSetup.CreateAnalysisDetail(TestHelper.UnitOfWork, simulationId);
            var explorer = TestHelper.UnitOfWork.AttributeRepo.GetExplorer();
            var analysisNetwork = TestHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(
                network.Id, explorer);
            var simulationsBefore = analysisNetwork.Simulations.ToList();
            Assert.Empty(simulationsBefore);

            TestHelper.UnitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, analysisNetwork);

            var simulationsAfter = analysisNetwork.Simulations.ToList();
            var simulationAfter = simulationsAfter.Single();
            Assert.Equal(simulationId, simulationAfter.Id);
        }

        [Fact]
        public void GetScenariosWithIds_SimulationExists_Gets()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, networkId: networkId);
            var simulationIds = new List<Guid> { simulationId };

            var simulations = TestHelper.UnitOfWork.SimulationRepo.GetScenariosWithIds(simulationIds);

            Assert.Single(simulations);
        }

        [Fact]
        public void DeleteSimulationsByNetworkId_NetworkInDbWithSimulations_Deletes()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationId = Guid.NewGuid();
            var simulationIds = new List<Guid> { simulationId };
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, networkId: networkId);
            var simulationsBefore = TestHelper.UnitOfWork.SimulationRepo.GetScenariosWithIds(simulationIds);
            Assert.NotEmpty(simulationsBefore);

            TestHelper.UnitOfWork.SimulationRepo.DeleteSimulationsByNetworkId(networkId);

            var simulationsAfter = TestHelper.UnitOfWork.SimulationRepo.GetScenariosWithIds(simulationIds);
            Assert.Empty(simulationsAfter);
        }

        [Fact]
        public void GetScenariosWithIds_SimulationDoesNotExist_Empty()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var nonexistentSimulationId = Guid.NewGuid();
            var simulationIds = new List<Guid> { nonexistentSimulationId };

            var simulations = TestHelper.UnitOfWork.SimulationRepo.GetScenariosWithIds(simulationIds);

            Assert.Empty(simulations);
        }

        [Fact]
        public void UpdateLastModifiedDate_SimulationInDb_Updates()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var networkId = NetworkTestSetup.NetworkId;
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, networkId: networkId);
            var simulationEntity = TestHelper.UnitOfWork.Context.Simulation.Single(
                s => s.Id == simulationId );
            simulationEntity.LastModifiedDate = new DateTime(2024, 1, 4);
            TestHelper.UnitOfWork.Context.Update(simulationEntity);
            TestHelper.UnitOfWork.Context.SaveChanges();
            var entityBefore = TestHelper.UnitOfWork.Context.Simulation.Single(
                s => s.Id == simulationId);
            var dateBefore = entityBefore.LastModifiedDate;
            Assert.Equal(new DateTime(2024, 1, 4), dateBefore);
            var dateLowerBound = DateTime.Now;

            TestHelper.UnitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);

            var dateUpperBound = DateTime.Now;
            var entityAfter = TestHelper.UnitOfWork.Context.Simulation.Single(
               s => s.Id == simulationId);
            var dateAfter = entityAfter.LastModifiedDate;
            DateTimeAssertions.Between(dateLowerBound, dateUpperBound, dateAfter, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void GetSimulationName_SimulationInDb_GetsName()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, simulationName);

            var actual = TestHelper.UnitOfWork.SimulationRepo.GetSimulationName(simulationId);

            Assert.Equal(simulationName, actual);
        }

        [Fact]
        public void SetNoTreatmentBeforeCommitted_ThenGet_True_ThenRemove_GetAgain_False()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            SimulationTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId);

            TestHelper.UnitOfWork.SimulationRepo.SetNoTreatmentBeforeCommitted(simulationId);
            var afterSet = TestHelper.UnitOfWork.SimulationRepo.GetNoTreatmentBeforeCommitted(simulationId);
            Assert.True(afterSet);
            TestHelper.UnitOfWork.SimulationRepo.RemoveNoTreatmentBeforeCommitted(simulationId);
            var afterRemove = TestHelper.UnitOfWork.SimulationRepo.GetNoTreatmentBeforeCommitted(simulationId);
            Assert.False(afterRemove);
        }

    }
}
