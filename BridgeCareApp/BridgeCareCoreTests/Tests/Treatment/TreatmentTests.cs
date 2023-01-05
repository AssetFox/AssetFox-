using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Treatment;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using BridgeCareCore.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Treatment;
using NuGet.LibraryModel;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCoreTests.Tests
{
    public class TreatmentTests
    {
        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }

        private TreatmentController CreateAuthorizedController()
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new TreatmentController(TreatmentServiceMocks.EmptyMock.Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            return controller;
        }

        private TreatmentController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new TreatmentController(TreatmentServiceMocks.EmptyMock.Object, EsecSecurityMocks.Admin, TestHelper.UnitOfWork,
                hubService, accessor, _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }

        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private ScenarioBudgetEntity CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            TestHelper.UnitOfWork.Context.AddEntity(budget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            TestHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = budget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            TestHelper.UnitOfWork.Context.SaveChanges();
            return budget;
        }

        [Fact]
        public async Task ShouldGetSelectedTreatmentByIdWithDataUnfixable()
        {
            // Arrange
            Setup();
            var controller = CreateAuthorizedController();
            CreateLibraryTestData();

            // Act
            var result = await controller.GetSelectedTreatmentById(_testTreatment.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (TreatmentDTO)Convert.ChangeType(okObjResult.Value,
                typeof(TreatmentDTO));        

            Assert.Equal(_testTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);

            Assert.Equal(_testTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, dto.Costs[0].Id);
        }
        [Fact]
        public async Task ShouldGetScenarioSelectedTreatmentByIdWithDataUnfixable()
        {
            Setup();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var controller = CreateAuthorizedController();
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var result = await controller.GetScenarioSelectedTreatmentById(_testScenarioTreatment.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dto = (TreatmentDTO)Convert.ChangeType(okObjResult.Value, typeof(TreatmentDTO));

            Assert.Equal(_testScenarioTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);
            Assert.Single(dto.BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dto.Costs[0].Id);
            Assert.Contains(budget.Id, dto.BudgetIds);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioGet()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var dtos = new List<TreatmentDTO> { dto };
            treatmentRepo.Setup(tr => tr.GetScenarioSelectableTreatments(simulationId)).Returns(dtos);

            var result = await controller.GetScenarioSelectedTreatments(simulationId);

            // Assert
            var value = (result as OkObjectResult).Value;
            var actualId = (value as List<TreatmentDTO>).Single().Id;
            Assert.Equal(treatmentId, actualId);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryPost()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = true,
                Library = dto,
            };
            treatmentService.Setup(ts => ts.GetSyncedLibraryDataset(It.IsAny<LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>>())).Returns(dto);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService);
            // Act
            var result = await controller.UpsertTreatmentLibrary(libraryRequest);

            //Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnScenarioPost()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = true,
                Library = dto,
            };
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService);
            var dtos = new List<TreatmentDTO>();
            var simulation = new SimulationDTO { Id = simulationId };

            var pageSync = new PagingSyncModel<TreatmentDTO>();
            treatmentService.Setup(ts => ts.GetSyncedScenarioDataset(simulationId, pageSync)).Returns(dtos);

            // Act
            var result = await controller.UpsertScenarioSelectedTreatments(simulationId, pageSync);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnLibraryDelete()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);

            // Act
            var result = await controller.DeleteTreatmentLibrary(Guid.Empty);

            // Assert
            ActionResultAssertions.Ok(result);
        }

        [Fact]
        public async Task ShouldGetLibraryTreatmentData()
        {
            // Arrange

            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var libraryId = Guid.NewGuid();
            var dto = new TreatmentLibraryDTO
            {
                Id = libraryId,
            };
            var dtos = new List<TreatmentLibraryDTO> { dto };
            treatmentRepo.Setup(tr => tr.GetAllTreatmentLibrariesNoChildren()).Returns(dtos);

            // Act
            var result = await controller.GetTreatmentLibraries();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);
            var actualDtos = okObjResult.Value;
            Assert.Equal(dtos, actualDtos);
        }

        [Fact]
        public async Task ShouldGetScenarioTreatmentData()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var controller = TestTreatmentControllerSetup.Create(unitOfWork);
            var treatment = new TreatmentDTO
            {
                Id = Guid.NewGuid(),
            };
            var expectedResult = new List<TreatmentDTO> { treatment };
            var simulationId = Guid.NewGuid();
            treatmentRepo.Setup(tr => tr.GetScenarioSelectableTreatments(simulationId)).Returns(expectedResult);

            // Act
            var result = await controller.GetScenarioSelectedTreatments(simulationId);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.Equal(expectedResult, okObjResult.Value);
        }

        [Fact]
        public async Task ShouldModifyLibraryTreatmentData()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService);
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentBefore = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var treatmentAfter = new TreatmentDTO
            {
                Id = treatmentId,
                Description = "Updated description",
            };
            var treatmentsBefore = new List<TreatmentDTO> { treatmentBefore };
            var treatmentsAfter = new List<TreatmentDTO> { treatmentAfter };
            var libraryBefore = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsBefore,
            };
            var libraryAfter = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsAfter,
            };
 
            var sync = new PagingSyncModel<TreatmentDTO>()
            {
                UpdateRows = new List<TreatmentDTO>() { treatmentAfter },
                LibraryId = libraryId,
            };

            var libraryRequest = new LibraryUpsertPagingRequestModel<TreatmentLibraryDTO, TreatmentDTO>()
            {
                IsNewLibrary = false,
                Library = libraryBefore,
                PagingSync = sync
            };
            treatmentService.Setup(ts => ts.GetSyncedLibraryDataset(libraryRequest)).Returns(libraryAfter);

            // Act
            var result = await controller.UpsertTreatmentLibrary(libraryRequest);

            // Assert
            var treatmentInvocation = treatmentRepo.SingleInvocationWithName(nameof(ISelectableTreatmentRepository.UpsertOrDeleteTreatments));
            var libraryInvocation = treatmentRepo.SingleInvocationWithName(nameof(ISelectableTreatmentRepository.UpsertTreatmentLibrary));
            Assert.Equal(libraryAfter, libraryInvocation.Arguments.Single());
            Assert.Equal(treatmentsAfter, treatmentInvocation.Arguments[0]);
            Assert.Equal(libraryId, treatmentInvocation.Arguments[1]);
        }

        [Fact]
        public async Task ShouldModifyScenarioTreatmentData()
        {
            var unitOfWork = UnitOfWorkMocks.New();
            var _ = UserRepositoryMocks.EveryoneExists(unitOfWork);
            var treatmentRepo = SelectableTreatmentRepositoryMocks.New(unitOfWork);
            var treatmentService = TreatmentServiceMocks.EmptyMock;
            var controller = TestTreatmentControllerSetup.Create(unitOfWork, treatmentService);
            var libraryId = Guid.NewGuid();
            var simulationId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var treatmentBefore = new TreatmentDTO
            {
                Id = treatmentId,
            };
            var treatmentAfter = new TreatmentDTO
            {
                Id = treatmentId,
                Description = "Updated description",
            };
            var treatmentsBefore = new List<TreatmentDTO> { treatmentBefore };
            var treatmentsAfter = new List<TreatmentDTO> { treatmentAfter };
            var libraryBefore = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsBefore,
            };
            var libraryAfter = new TreatmentLibraryDTO
            {
                Id = libraryId,
                Treatments = treatmentsAfter,
            };

            var sync = new PagingSyncModel<TreatmentDTO>()
            {
                UpdateRows = new List<TreatmentDTO>() { treatmentAfter },
                LibraryId = libraryId,
            };
            treatmentService.Setup(ts => ts.GetSyncedScenarioDataset(simulationId, sync)).Returns(treatmentsAfter);

            var result = await controller.UpsertScenarioSelectedTreatments(simulationId, sync);
            ActionResultAssertions.Ok(result);
            var call = treatmentService.SingleInvocationWithName(nameof(ITreatmentService.GetSyncedScenarioDataset));
            Assert.Equal(simulationId, call.Arguments[0]);
            Assert.Equal(sync, call.Arguments[1]);
        }

        [Fact]
        public async Task ShouldDeleteLibraryData()
        {
            //// Arrange
            //Setup();
            //var controller = CreateAuthorizedController();
            //CreateLibraryTestData();

            //// Act
            //var result = await controller.DeleteTreatmentLibrary(_testTreatmentLibrary.Id);

            //// Assert
            //Assert.IsType<OkResult>(result);

            //Assert.True(
            //    !TestHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            //Assert.True(!TestHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            //Assert.True(!TestHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            //Assert.True(
            //    !TestHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
            //        _.Id == _testTreatmentConsequence.Id));
        }
        [Fact]
        public async Task UserIsViewTreatmentFromLibraryAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTreatmentFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsModifyTreatementFromScenarioAuthorized()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ModifyTreatmentFromScenario,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentModifyAnyFromScenarioAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentModifyPermittedFromScenarioAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ModifyTreatmentFromScenario);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteTreatmentFromLibraryAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentDeletePermittedFromLibraryAccess,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentDeleteAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteTreatmentFromLibrary);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewTreatmentFromLibraryAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.TreatmentViewAnyFromLibraryAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewTreatmentFromLibrary);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
