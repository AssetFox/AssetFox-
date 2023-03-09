using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.Analysis;
using Microsoft.SqlServer.Management.Smo;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.BudgetPriority;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class BudgetPriorityRepositoryTests
    {
        private ScenarioBudgetEntity _testScenarioBudget;
        private ScenarioBudgetPriorityEntity _testScenarioBudgetPriority;
        private BudgetPercentagePairEntity _testBudgetPercentagePair;
        private BudgetPriorityLibraryEntity _testBudgetPriorityLibrary;
        private BudgetPriorityEntity _testBudgetPriority;
        private const string BudgetPriorityLibraryEntityName = "BudgetPriorityLibraryEntity";

        private void Setup()
        {
            var unitOfWork = TestHelper.UnitOfWork;
            AttributeTestSetup.CreateAttributes(unitOfWork);
            NetworkTestSetup.CreateNetwork(unitOfWork);
        }



        private void CreateLibraryTestData()
        {
            _testBudgetPriorityLibrary = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = BudgetPriorityLibraryEntityName };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPriorityLibrary);


            _testBudgetPriority = new BudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                BudgetPriorityLibraryId = _testBudgetPriorityLibrary.Id,
                PriorityLevel = 1,
                CriterionLibraryBudgetPriorityJoin = new CriterionLibraryBudgetPriorityEntity
                {
                    BudgetPriority = _testBudgetPriority,
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPriority);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData(Guid simulationId)
        {
            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "ScenarioBudgetEntity"
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testScenarioBudgetPriority = new ScenarioBudgetPriorityEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                PriorityLevel = 1,
                CriterionLibraryScenarioBudgetPriorityJoin = new CriterionLibraryScenarioBudgetPriorityEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Budget Priority Criterion",
                        IsSingleUse = true,
                        MergedCriteriaExpression = ""
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudgetPriority);


            _testBudgetPercentagePair = new BudgetPercentagePairEntity
            {
                Id = Guid.NewGuid(),
                ScenarioBudgetPriorityId = _testScenarioBudgetPriority.Id,
                ScenarioBudgetId = _testScenarioBudget.Id,
                Percentage = 100
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetPercentagePair);
        }


        [Fact]
        public void ShouldReturnOkResultOnLibraryGet()
        {
            // Arrange
            Setup();

            // Act
            var result = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriortyLibrariesNoChildren();

            // Assert
        }

        [Fact]
        public void ShouldReturnOkResultOnScenarioGet()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            var result = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id);

            // Assert
        }

        [Fact]
        public void ShouldReturnOkResultOnLibraryPost()
        {
            // Arrange
            Setup();
            var dto = new BudgetPriorityLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                BudgetPriorities = new List<BudgetPriorityDTO>()
            };

            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);

            // Assert
        }

        [Fact]
        public void ShouldReturnOkResultOnScenarioPost()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var dtos = new List<BudgetPriorityDTO>();

            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(dtos, simulation.Id);

            // Assert
        }

        [Fact]
        public void ShouldReturnOkResultOnDelete()
        {
            Setup();
            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(Guid.Empty);

            // Assert
        }

        [Fact]
        public void ShouldGetLibraryNoData()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();

            // Assert
            Assert.Contains(dtos, b => b.Name == BudgetPriorityLibraryEntityName);
            var budgetPriorityLibraryDTO = dtos.FirstOrDefault(b => b.Name == BudgetPriorityLibraryEntityName && b.Id == _testBudgetPriorityLibrary.Id);
        }

        [Fact]
        public void ShouldGetScenarioData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id);

            // Assert
            var dto = dtos.Single();
            Assert.Equal(_testScenarioBudgetPriority.Id, dto.Id);
            Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dto.PriorityLevel);
            Assert.Equal(_testScenarioBudgetPriority.Year, dto.Year);

            Assert.Single(dto.BudgetPercentagePairs);
            Assert.Equal(_testBudgetPercentagePair.Id, dto.BudgetPercentagePairs[0].Id);
            Assert.Equal(_testBudgetPercentagePair.Percentage, dto.BudgetPercentagePairs[0].Percentage);
            Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dto.BudgetPercentagePairs[0].BudgetId);
            Assert.Equal(_testScenarioBudget.Name, dto.BudgetPercentagePairs[0].BudgetName);

            Assert.Equal(_testScenarioBudgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId, dto.CriterionLibrary.Id);
        }

        [Fact]
        public void ShouldModifyLibraryData()
        {
            // Arrange
            Setup();
            CreateLibraryTestData();

            // Arrange
            _testBudgetPriorityLibrary.BudgetPriorities = new List<BudgetPriorityEntity> { _testBudgetPriority };

            var dto = _testBudgetPriorityLibrary.ToDto();
            dto.Description = "Updated Description";
            var updatedPriority = dto.BudgetPriorities[0];
            updatedPriority.PriorityLevel = 2;
            updatedPriority.Year = DateTime.Now.Year + 1;
            updatedPriority.CriterionLibrary = new CriterionLibraryDTO();

            var updateRows = new List<BudgetPriorityDTO>() { updatedPriority };

            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(updateRows, dto.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().Single(l => l.Id == dto.Id);
            Assert.Equal(dto.Description, modifiedDto.Description);

            Assert.Equal(dto.BudgetPriorities[0].PriorityLevel, modifiedDto.BudgetPriorities[0].PriorityLevel);
            Assert.Equal(dto.BudgetPriorities[0].Year, modifiedDto.BudgetPriorities[0].Year);
            Assert.Equal(dto.BudgetPriorities[0].CriterionLibrary.Id,
                modifiedDto.BudgetPriorities[0].CriterionLibrary.Id);
        }

        [Fact]
        public void ShouldModifyScenarioData()
        {
            // Arrange
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);
            _testScenarioBudgetPriority.BudgetPercentagePairs =
                new List<BudgetPercentagePairEntity> { _testBudgetPercentagePair };
            var dtos = new List<BudgetPriorityDTO> { _testScenarioBudgetPriority.ToDto() };
            var updatedPriorty = dtos[0];
            updatedPriorty.PriorityLevel = 2;
            updatedPriorty.Year = DateTime.Now.Year + 1;
            updatedPriorty.CriterionLibrary = new CriterionLibraryDTO();
            updatedPriorty.BudgetPercentagePairs[0].Percentage = 90;
            var updateDtos = new List<BudgetPriorityDTO>() { updatedPriorty };

            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(updateDtos, simulation.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id)[0];
            Assert.Equal(dtos[0].PriorityLevel, modifiedDto.PriorityLevel);
            Assert.Equal(dtos[0].Year, modifiedDto.Year);
            Assert.Equal(dtos[0].CriterionLibrary.Id, modifiedDto.CriterionLibrary.Id);
            Assert.Equal(dtos[0].BudgetPercentagePairs[0].Percentage, modifiedDto.BudgetPercentagePairs[0].Percentage);
        }

        [Fact]
        public void ShouldDeleteLibraryData()
        {
            Setup();
            CreateLibraryTestData();

            // Act
            TestHelper.UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(_testBudgetPriorityLibrary.Id);

            // Assert
            Assert.False(TestHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == _testBudgetPriorityLibrary.Id));
            Assert.False(TestHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == _testBudgetPriority.Id));
            Assert.False(TestHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
                    _.BudgetPriorityId == _testBudgetPriority.Id));
        }
        [Fact]
        public async Task UpdateBudgetPriorityLibraryWithUserAccessChange_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetPriorityLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetPriorityLibraryUserTestSetup.SetUsersOfBudgetPriorityLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            Assert.Equal(LibraryAccessLevel.Modify, libraryUserBefore.AccessLevel);
            libraryUserBefore.AccessLevel = LibraryAccessLevel.Read;

            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            var libraryUserAfter = libraryUsersAfter.Single();
            Assert.Equal(LibraryAccessLevel.Read, libraryUserAfter.AccessLevel);
        }
        [Fact]
        public async Task UpdateBudgetPriorityLibraryUsers_RequestAccessRemoval_Does()
        {
            var user = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetPriorityLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetPriorityLibraryUserTestSetup.SetUsersOfBudgetPriorityLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user.Id);
            var libraryUsersBefore = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            var libraryUserBefore = libraryUsersBefore.Single();
            libraryUsersBefore.Remove(libraryUserBefore);

            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteUsers(library.Id, libraryUsersBefore);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            Assert.Empty(libraryUsersAfter);
        }
        [Fact]
        public async Task UpdateLibraryUsers_AddAccessForUser_Does()
        {
            var user1 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var user2 = await UserTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            var library = BudgetPriorityLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork);
            BudgetPriorityLibraryUserTestSetup.SetUsersOfBudgetPriorityLibrary(TestHelper.UnitOfWork, library.Id, LibraryAccessLevel.Modify, user1.Id);
            var usersBefore = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            var newUser = new LibraryUserDTO
            {
                AccessLevel = LibraryAccessLevel.Read,
                UserId = user2.Id,
            };
            usersBefore.Add(newUser);

            TestHelper.UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteUsers(library.Id, usersBefore);

            var libraryUsersAfter = TestHelper.UnitOfWork.BudgetPriorityRepo.GetLibraryUsers(library.Id);
            var user1After = libraryUsersAfter.Single(u => u.UserId == user1.Id);
            var user2After = libraryUsersAfter.Single(u => u.UserId == user2.Id);
            Assert.Equal(LibraryAccessLevel.Modify, user1After.AccessLevel);
            Assert.Equal(LibraryAccessLevel.Read, user2After.AccessLevel);
        }
    }
}
