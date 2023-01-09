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

        //[Fact]
        //public async Task ShouldGetLibraryNoData()
        //{
        //    // Arrange
        //    Setup();
        //    var controller = CreateAuthorizedController();
        //    CreateLibraryTestData();

        //    // Act
        //    var result = await controller.GetBudgetPriorityLibraries();

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var dtos = (List<BudgetPriorityLibraryDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(List<BudgetPriorityLibraryDTO>));
        //    Assert.Contains(dtos, b => b.Name == BudgetPriorityLibraryEntityName);
        //    var budgetPriorityLibraryDTO = dtos.FirstOrDefault(b => b.Name == BudgetPriorityLibraryEntityName && b.Id == _testBudgetPriorityLibrary.Id);
        //}

        //[Fact]
        //public async Task ShouldGetScenarioData()
        //{
        //    // Arrange
        //    Setup();
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var controller = CreateAuthorizedController();
        //    CreateScenarioTestData(simulation.Id);

        //    // Act
        //    var result = await controller.GetScenarioBudgetPriorities(simulation.Id);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var dtos = (List<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(List<BudgetPriorityDTO>));
        //    Assert.Single(dtos);
        //    Assert.Equal(_testScenarioBudgetPriority.Id, dtos[0].Id);
        //    Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
        //    Assert.Equal(_testScenarioBudgetPriority.Year, dtos[0].Year);

        //    Assert.Single(dtos[0].BudgetPercentagePairs);
        //    Assert.Equal(_testBudgetPercentagePair.Id, dtos[0].BudgetPercentagePairs[0].Id);
        //    Assert.Equal(_testBudgetPercentagePair.Percentage, dtos[0].BudgetPercentagePairs[0].Percentage);
        //    Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dtos[0].BudgetPercentagePairs[0].BudgetId);
        //    Assert.Equal(_testScenarioBudget.Name, dtos[0].BudgetPercentagePairs[0].BudgetName);

        //    Assert.Equal(_testScenarioBudgetPriority.CriterionLibraryScenarioBudgetPriorityJoin.CriterionLibraryId, dtos[0].CriterionLibrary.Id);
        //}

        //[Fact]
        //public async Task GetScenarioBudgetPriorityPageData()
        //{
        //    // Arrange
        //    Setup();
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var controller = CreateAuthorizedController();
        //    CreateScenarioTestData(simulation.Id);
        //    var request = new PagingRequestModel<BudgetPriorityDTO>();
        //    // Act
        //    var result = await controller.GetScenarioBudgetPriorityPage(simulation.Id, request);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(PagingPageModel<BudgetPriorityDTO>));
        //    var dtos = page.Items;
        //    Assert.Single(dtos);
        //    Assert.Equal(_testScenarioBudgetPriority.Id, dtos[0].Id);
        //    Assert.Equal(_testScenarioBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
        //    Assert.Equal(_testScenarioBudgetPriority.Year, dtos[0].Year);

        //    Assert.Single(dtos[0].BudgetPercentagePairs);
        //    Assert.Equal(_testBudgetPercentagePair.Id, dtos[0].BudgetPercentagePairs[0].Id);
        //    Assert.Equal(_testBudgetPercentagePair.Percentage, dtos[0].BudgetPercentagePairs[0].Percentage);
        //    Assert.Equal(_testBudgetPercentagePair.ScenarioBudgetId, dtos[0].BudgetPercentagePairs[0].BudgetId);
        //    Assert.Equal(_testScenarioBudget.Name, dtos[0].BudgetPercentagePairs[0].BudgetName);
        //}

        //[Fact]
        //public async Task ShouldGetLibraryBudgetPriorityPageData()
        //{
        //    // Arrange
        //    Setup();
        //    var controller = CreateAuthorizedController();
        //    CreateLibraryTestData();
        //    var dto = _testBudgetPriorityLibrary.ToDto();
        //    var request = new PagingRequestModel<BudgetPriorityDTO>();
        //    // Act
        //    var result = await controller.GetLibraryBudgetPriortyPage(dto.Id, request);

        //    // Assert
        //    var okObjResult = result as OkObjectResult;
        //    Assert.NotNull(okObjResult.Value);

        //    var page = (PagingPageModel<BudgetPriorityDTO>)Convert.ChangeType(okObjResult.Value,
        //        typeof(PagingPageModel<BudgetPriorityDTO>));
        //    var dtos = page.Items;
        //    Assert.Single(dtos);
        //    Assert.Equal(_testBudgetPriority.Id, dtos[0].Id);
        //    Assert.Equal(_testBudgetPriority.PriorityLevel, dtos[0].PriorityLevel);
        //    Assert.Equal(_testBudgetPriority.Year, dtos[0].Year);
        //}

        //[Fact]
        //public async Task ShouldModifyLibraryData()
        //{
        //    // Arrange
        //    Setup();
        //    var controller = CreateAuthorizedController();
        //    CreateLibraryTestData();

        //    // Arrange
        //    _testBudgetPriorityLibrary.BudgetPriorities = new List<BudgetPriorityEntity> { _testBudgetPriority };

        //    var dto = _testBudgetPriorityLibrary.ToDto();
        //    dto.Description = "Updated Description";
        //    var updatedPriority = dto.BudgetPriorities[0];
        //    updatedPriority.PriorityLevel = 2;
        //    updatedPriority.Year = DateTime.Now.Year + 1;
        //    updatedPriority.CriterionLibrary = new CriterionLibraryDTO();

        //    var request = new LibraryUpsertPagingRequestModel<BudgetPriorityLibraryDTO, BudgetPriorityDTO>()
        //    {
        //        Library = dto,
        //        PagingSync = new PagingSyncModel<BudgetPriorityDTO>()
        //        {
        //            UpdateRows = new List<BudgetPriorityDTO>() { updatedPriority }
        //        }
        //    };

        //    // Act
        //    await controller.UpsertBudgetPriorityLibrary(request);

        //    // Assert
        //    var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().Single(l => l.Id == dto.Id);
        //    Assert.Equal(dto.Description, modifiedDto.Description);

        //    Assert.Equal(dto.BudgetPriorities[0].PriorityLevel, modifiedDto.BudgetPriorities[0].PriorityLevel);
        //    Assert.Equal(dto.BudgetPriorities[0].Year, modifiedDto.BudgetPriorities[0].Year);
        //    Assert.Equal(dto.BudgetPriorities[0].CriterionLibrary.Id,
        //        modifiedDto.BudgetPriorities[0].CriterionLibrary.Id);
        //}

        //[Fact]
        //public async Task ShouldModifyScenarioData()
        //{
        //    // Arrange
        //    Setup();
        //    var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
        //    var controller = CreateAuthorizedController();
        //    CreateScenarioTestData(simulation.Id);

        //    // Arrange
        //    _testScenarioBudgetPriority.BudgetPercentagePairs =
        //        new List<BudgetPercentagePairEntity> { _testBudgetPercentagePair };
        //    var dtos = new List<BudgetPriorityDTO> { _testScenarioBudgetPriority.ToDto() };
        //    var updatedPriorty = dtos[0];
        //    updatedPriorty.PriorityLevel = 2;
        //    updatedPriorty.Year = DateTime.Now.Year + 1;
        //    updatedPriorty.CriterionLibrary = new CriterionLibraryDTO();
        //    updatedPriorty.BudgetPercentagePairs[0].Percentage = 90;
        //    var request = new PagingSyncModel<BudgetPriorityDTO>()
        //    {
        //        UpdateRows = new List<BudgetPriorityDTO>() { updatedPriorty }
        //    };
        //    // Act
        //    await controller.UpsertScenarioBudgetPriorities(simulation.Id, request);

        //    // Assert
        //    var modifiedDto = TestHelper.UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulation.Id)[0];
        //    Assert.Equal(dtos[0].PriorityLevel, modifiedDto.PriorityLevel);
        //    Assert.Equal(dtos[0].Year, modifiedDto.Year);
        //    Assert.Equal(dtos[0].CriterionLibrary.Id, modifiedDto.CriterionLibrary.Id);
        //    Assert.Equal(dtos[0].BudgetPercentagePairs[0].Percentage, modifiedDto.BudgetPercentagePairs[0].Percentage);
        //}

        //[Fact]
        //public async Task ShouldDeleteLibraryData()
        //{
        //    // Arrange
        //    Setup();
        //    var controller = CreateAuthorizedController();
        //    CreateLibraryTestData();

        //    // Act
        //    var result = await controller.DeleteBudgetPriorityLibrary(_testBudgetPriorityLibrary.Id);

        //    // Assert
        //    Assert.IsType<OkResult>(result);

        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.BudgetPriorityLibrary.Any(_ => _.Id == _testBudgetPriorityLibrary.Id));
        //    Assert.True(!TestHelper.UnitOfWork.Context.BudgetPriority.Any(_ => _.Id == _testBudgetPriority.Id));
        //    Assert.True(
        //        !TestHelper.UnitOfWork.Context.CriterionLibraryBudgetPriority.Any(_ =>
        //            _.BudgetPriorityId == _testBudgetPriority.Id));
        //}

        //[Fact]
        //public async Task UserIsViewBudgetPriorityFromLibraryAuthorized()
        //{
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name, BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewAnyFromLibraryAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewBudgetPriorityFromLibrary);
        //    // Assert
        //    Assert.True(allowed.Succeeded);

        //}
        //[Fact]
        //public async Task UserIsModifyBudgetPriorityFromScenarioAuthorized()
        //{
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ModifyBudgetPriorityFromScenario,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyAnyFromScenarioAccess,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityModifyPermittedFromScenarioAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyBudgetPriorityFromScenario);
        //    // Assert
        //    Assert.True(allowed.Succeeded);
        //}
        //[Fact]
        //public async Task UserIsDeleteBudgetPriorityFromLibraryAuthorized()
        //{
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.DeleteBudgetPriorityFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.AnnouncementModifyAccess,
        //                                              BridgeCareCore.Security.SecurityConstants.Claim.AttributesUpdateAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteBudgetPriorityFromLibrary);
        //    // Assert
        //    Assert.False(allowed.Succeeded);
        //}
        //[Fact]
        //public async Task UserIsViewBudgetPriorityFromLibraryAuthorized_B2C()
        //{
        //    // Arrange
        //    var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
        //    {
        //        services.AddAuthorization(options =>
        //        {
        //            options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
        //                policy => policy.RequireClaim(ClaimTypes.Name, BridgeCareCore.Security.SecurityConstants.Claim.BudgetPriorityViewAnyFromLibraryAccess));
        //        });
        //    });
        //    var roleClaimsMapper = new RoleClaimsMapper();
        //    var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
        //    // Act
        //    var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewBudgetPriorityFromLibrary);
        //    // Assert
        //    Assert.True(allowed.Succeeded);
        //}

    }
}
