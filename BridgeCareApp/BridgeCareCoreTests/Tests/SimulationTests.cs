using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Xunit;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCoreTests.Tests
{
    public class SimulationTests
    {
        private SimulationController _controller;

        private UserEntity _testUserEntity;
        private SimulationEntity _testSimulationToClone;
        private const string SimulationName = "Simulation";
        private static readonly Guid UserId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();
        private readonly Mock<ISimulationQueueService> _mockSimulationQueueService = new();

        private async Task<UserDTO> AddTestUser()
        {
            var randomName = RandomStrings.Length11();
            var role = "PD-BAMS-Administrator";
            TestHelper.UnitOfWork.AddUser(randomName, true);
            var returnValue = await TestHelper.UnitOfWork.UserRepo.GetUserByUserName(randomName);
            return returnValue;
        }

        public SimulationAnalysisService Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(TestHelper.UnitOfWork);

            var simulationAnalysisService =
                new SimulationAnalysisService(TestHelper.UnitOfWork, new());
            return simulationAnalysisService;
        }

        private void CreateAuthorizedController(SimulationAnalysisService simulationAnalysisService)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new SimulationController(
                simulationAnalysisService,
                new SimulationService(TestHelper.UnitOfWork, new SimulationRepository(TestHelper.UnitOfWork)),
                _mockSimulationQueueService.Object,
                EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object);
        }

        private void CreateUnauthorizedController(SimulationAnalysisService simulationAnalysisService)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            _controller = new SimulationController(simulationAnalysisService,
                new SimulationService(TestHelper.UnitOfWork, new SimulationRepository(TestHelper.UnitOfWork)),
                _mockSimulationQueueService.Object,
                EsecSecurityMocks.Dbe,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object);
        }


        private SimulationController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var service = Setup();
            var controller = new SimulationController(service,
                new SimulationService(TestHelper.UnitOfWork, new SimulationRepository(TestHelper.UnitOfWork)),
                _mockSimulationQueueService.Object,
                EsecSecurityMocks.Dbe,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockClaimHelper.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        private void CreateTestData()
        {
            if (!TestHelper.UnitOfWork.Context.User.Any(u => u.Username == "Clone Tester"))
            {
                _testUserEntity = new UserEntity { Id = Guid.NewGuid(), Username = "Clone Tester" };
                TestHelper.UnitOfWork.Context.AddEntity(_testUserEntity);
                TestHelper.UnitOfWork.SetUser(_testUserEntity.Username);
                TestHelper.UnitOfWork.Context.SaveChanges();
            }

            if (!TestHelper.UnitOfWork.Context.Simulation.Any(s => s.Name == SimulationName))
            {
                var attribute = TestHelper.UnitOfWork.Context.Attribute.First();
                var budgetId = Guid.NewGuid();
                var committedProjectEnity = new CommittedProjectEntity
                {
                    Id = Guid.NewGuid(),
                    Cost = 500000,
                    Name = "Committed Project",
                    Year = DateTime.Now.Year,
                    ShadowForAnyTreatment = 1,
                    ShadowForSameTreatment = 1,
                    ScenarioBudgetId = budgetId,
                    CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>
                    {
                        new CommittedProjectConsequenceEntity
                        {
                            Id = Guid.NewGuid(), AttributeId = attribute.Id, ChangeValue = "+1"
                        }
                    }
                };
                committedProjectEnity.CommittedProjectLocation = new CommittedProjectLocationEntity(Guid.NewGuid(), DataPersistenceConstants.SectionLocation, "FacilitySection")
                {
                    CommittedProjectId = committedProjectEnity.Id
                };
                _testSimulationToClone = new SimulationEntity
                {
                    Id = Guid.NewGuid(),
                    Name = SimulationName,
                    NumberOfYearsOfTreatmentOutlook = 1,
                    NetworkId = NetworkTestSetup.NetworkId,
                    SimulationUserJoins = new List<SimulationUserEntity>
                {
                    new SimulationUserEntity
                    {
                        UserId = TestHelper.UnitOfWork.UserEntity.Id,
                        CanModify = true,
                        IsOwner = true,
                        CreatedBy = TestHelper.UnitOfWork.UserEntity.Id,
                        LastModifiedBy = TestHelper.UnitOfWork.UserEntity.Id
                    }
                },
                    AnalysisMethod =
                        new AnalysisMethodEntity
                        {
                            Id = Guid.NewGuid(),
                            AttributeId = attribute.Id,
                            Benefit =
                                new BenefitEntity { Id = Guid.NewGuid(), AttributeId = attribute.Id, Limit = 1 },
                            CriterionLibraryAnalysisMethodJoin = new CriterionLibraryAnalysisMethodEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Analysis Method Criterion",
                                    MergedCriteriaExpression = "Analysis Method Criterion Expression",
                                    IsSingleUse = true
                                }
                            }
                        },
                    Budgets =
                        new List<ScenarioBudgetEntity>
                        {
                        new ScenarioBudgetEntity
                        {
                            Id = budgetId,
                            Name = "Cloned Budget",
                            ScenarioBudgetAmounts =
                                new List<ScenarioBudgetAmountEntity>
                                {
                                    new ScenarioBudgetAmountEntity
                                    {
                                        Id = Guid.NewGuid(), Value = 500000, Year = DateTime.Now.Year
                                    }
                                },
                            CriterionLibraryScenarioBudgetJoin = new CriterionLibraryScenarioBudgetEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Budget Criterion",
                                    MergedCriteriaExpression = "Budget Criterion Expression",
                                    IsSingleUse = true
                                }
                            }
                        }
                        },
                    BudgetPriorities = new List<ScenarioBudgetPriorityEntity>
                {
                    new ScenarioBudgetPriorityEntity
                    {
                        Id = Guid.NewGuid(),
                        PriorityLevel = 1,
                        BudgetPercentagePairs =
                            new List<BudgetPercentagePairEntity>
                            {
                                new BudgetPercentagePairEntity {ScenarioBudgetId = budgetId}
                            },
                        CriterionLibraryScenarioBudgetPriorityJoin =
                            new CriterionLibraryScenarioBudgetPriorityEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Budget Priority Criterion",
                                    MergedCriteriaExpression =
                                        "Budget Priority Criterion Expression",
                                    IsSingleUse = true
                                }
                            }
                    }
                },
                    CashFlowRules = new List<ScenarioCashFlowRuleEntity>
                {
                    new ScenarioCashFlowRuleEntity
                    {
                        Id = Guid.NewGuid(), Name = "Cash Flow Rule",
                        ScenarioCashFlowDistributionRules =
                            new List<ScenarioCashFlowDistributionRuleEntity>
                            {
                                new ScenarioCashFlowDistributionRuleEntity
                                {
                                    Id = Guid.NewGuid(),
                                    CostCeiling = 500000,
                                    YearlyPercentages = "100",
                                    DurationInYears = 1
                                }
                            },
                        CriterionLibraryScenarioCashFlowRuleJoin =
                            new CriterionLibraryScenarioCashFlowRuleEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Cash Flow Rule Criterion",
                                    MergedCriteriaExpression = "Cash Flow Rule Criterion Expression",
                                    IsSingleUse = true
                                }
                            }
                    }
                },
                    InvestmentPlan =
                        new InvestmentPlanEntity
                        {
                            Id = Guid.NewGuid(),
                            InflationRatePercentage = 3,
                            MinimumProjectCostLimit = 500000,
                            FirstYearOfAnalysisPeriod = DateTime.Now.Year,
                            NumberOfYearsInAnalysisPeriod = 1
                        },
                    CommittedProjects =
                        new List<CommittedProjectEntity>
                        {
                            committedProjectEnity
                        },
                    PerformanceCurves =
                        new List<ScenarioPerformanceCurveEntity>
                        {
                        new ScenarioPerformanceCurveEntity
                        {
                            Id = Guid.NewGuid(),
                            AttributeId = attribute.Id,
                            Name = "Performance Curve",
                            ScenarioPerformanceCurveEquationJoin =
                                new ScenarioPerformanceCurveEquationEntity
                                {
                                    Equation = new EquationEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        Expression = "Performance Curve Equation Expression"
                                    }
                                },
                            CriterionLibraryScenarioPerformanceCurveJoin =
                                new CriterionLibraryScenarioPerformanceCurveEntity
                                {
                                    CriterionLibrary = new CriterionLibraryEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Performance Curve Criterion",
                                        MergedCriteriaExpression =
                                            "Performance Curve Criterion Expression",
                                        IsSingleUse = true
                                    }
                                }
                        }
                        },
                    RemainingLifeLimits =
                        new List<ScenarioRemainingLifeLimitEntity>
                        {
                        new ScenarioRemainingLifeLimitEntity
                        {
                            Id = Guid.NewGuid(),
                            AttributeId = attribute.Id,
                            Value = 500000,
                            CriterionLibraryScenarioRemainingLifeLimitJoin =
                                new CriterionLibraryScenarioRemainingLifeLimitEntity
                                {
                                    CriterionLibrary = new CriterionLibraryEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Remaining Life Limit Criterion",
                                        MergedCriteriaExpression =
                                            "Remaining Life Limit Criterion Expression",
                                        IsSingleUse = true
                                    }
                                }
                        }
                        },
                    ScenarioDeficientConditionGoals =
                        new List<ScenarioDeficientConditionGoalEntity>
                        {
                        new ScenarioDeficientConditionGoalEntity
                        {
                            Id = Guid.NewGuid(),
                            AttributeId = attribute.Id,
                            Name = "Deficient Condition Goal",
                            DeficientLimit = 1,
                            AllowedDeficientPercentage = 1,
                            CriterionLibraryScenarioDeficientConditionGoalJoin =
                                new CriterionLibraryScenarioDeficientConditionGoalEntity
                                {
                                    CriterionLibrary = new CriterionLibraryEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = "Deficient Condition Goal Criterion",
                                        MergedCriteriaExpression =
                                            "Deficient Condition Goal Criterion Expression",
                                        IsSingleUse = true
                                    }
                                }
                        }
                        },
                    ScenarioTargetConditionalGoals = new List<ScenarioTargetConditionGoalEntity>
                {
                    new ScenarioTargetConditionGoalEntity
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = attribute.Id,
                        Name = "Target Condition Goal",
                        Target = 1,
                        CriterionLibraryScenarioTargetConditionGoalJoin =
                            new CriterionLibraryScenarioTargetConditionGoalEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Target Condition Goal Criterion",
                                    MergedCriteriaExpression =
                                        "Target Condition Goal Criterion Expression",
                                    IsSingleUse = true
                                }
                            }
                    }
                },
                    SelectableTreatments = new List<ScenarioSelectableTreatmentEntity>
                {
                    new ScenarioSelectableTreatmentEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Treatment",
                        ShadowForAnyTreatment = 1,
                        ShadowForSameTreatment = 1,
                        CriterionLibraryScenarioSelectableTreatmentJoin =
                            new CriterionLibraryScenarioSelectableTreatmentEntity
                            {
                                CriterionLibrary = new CriterionLibraryEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Treatment Criterion",
                                    MergedCriteriaExpression =
                                        "Treatment Criterion Expression",
                                    IsSingleUse = true
                                }
                            },
                        ScenarioSelectableTreatmentScenarioBudgetJoins =
                            new List<ScenarioSelectableTreatmentScenarioBudgetEntity>
                            {
                                new ScenarioSelectableTreatmentScenarioBudgetEntity {ScenarioBudgetId = budgetId}
                            },
                        ScenarioTreatmentConsequences = new List<ScenarioConditionalTreatmentConsequenceEntity>
                        {
                            new ScenarioConditionalTreatmentConsequenceEntity
                            {
                                Id = Guid.NewGuid(),
                                AttributeId = attribute.Id,
                                ChangeValue = "+1",
                                ScenarioConditionalTreatmentConsequenceEquationJoin =
                                    new ScenarioConditionalTreatmentConsequenceEquationEntity
                                    {
                                        Equation = new EquationEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Expression = "Treatment Consequence Equation Expression"
                                        }
                                    },
                                CriterionLibraryScenarioConditionalTreatmentConsequenceJoin =
                                    new CriterionLibraryScenarioConditionalTreatmentConsequenceEntity
                                    {
                                        CriterionLibrary = new CriterionLibraryEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Name = "Treatment Consequence Criterion",
                                            MergedCriteriaExpression =
                                                "Treatment Consequence Expression",
                                            IsSingleUse = true
                                        }
                                    }
                            }
                        },
                        ScenarioTreatmentCosts = new List<ScenarioTreatmentCostEntity>
                        {
                            new ScenarioTreatmentCostEntity
                            {
                                Id = Guid.NewGuid(),
                                ScenarioTreatmentCostEquationJoin =
                                    new ScenarioTreatmentCostEquationEntity
                                    {
                                        Equation = new EquationEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Expression = "Treatment Cost Equation Expression"
                                        }
                                    },
                                CriterionLibraryScenarioTreatmentCostJoin =
                                    new CriterionLibraryScenarioTreatmentCostEntity
                                    {
                                        CriterionLibrary = new CriterionLibraryEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Name = "Treatment Cost Criterion",
                                            MergedCriteriaExpression =
                                                "Treatment Cost Expression",
                                            IsSingleUse = true
                                        }
                                    }
                            }
                        },
                        ScenarioTreatmentSchedulings =
                            new List<ScenarioTreatmentSchedulingEntity>
                            {
                                new ScenarioTreatmentSchedulingEntity {Id = Guid.NewGuid(), OffsetToFutureYear = 1}
                            },
                        ScenarioTreatmentSupersessions = new List<ScenarioTreatmentSupersessionEntity>
                        {
                            new ScenarioTreatmentSupersessionEntity
                            {
                                Id = Guid.NewGuid(),
                                CriterionLibraryScenarioTreatmentSupersessionJoin =
                                    new CriterionLibraryScenarioTreatmentSupersessionEntity
                                    {
                                        CriterionLibrary = new CriterionLibraryEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Name = "Treatment Supersession Criterion",
                                            MergedCriteriaExpression =
                                                "Treatment Supersession Expression",
                                            IsSingleUse = true
                                        }
                                    }
                            }
                        }
                    }
                }
                };
                TestHelper.UnitOfWork.Context.AddEntity(_testSimulationToClone);
                TestHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        [Fact] // Seems to be some connection with other tests here. For example, WJ had a failure in an "unrelated" attribute import test that fried it.
        public async Task ShouldDeleteSimulation()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            await _controller.DeleteSimulationOperation(simulation.Id);
            // Assert
            Assert.True(!TestHelper.UnitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id));
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGetUserScenariosPage()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await _controller.GetUserScenariosPage(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGetSharedScenariosPage()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var result = await _controller.GetSharedScenariosPage(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.TestSimulation();
            var result = await _controller.CreateSimulation(NetworkTestSetup.NetworkId, simulation);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPut()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            // Act
            var result = await _controller.UpdateSimulation(simulation);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnDelete()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);

            // Act
            var result = await _controller.DeleteSimulationOperation(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldGetSimulationCreatedByUser()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, owner:TestHelper.UnitOfWork.CurrentUser.Id);
            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            // Act
            var userResult = await _controller.GetUserScenariosPage(request);

            // Assert
            var okObjResult = userResult as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = ((PagingPageModel<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(PagingPageModel<SimulationDTO>))).Items;
            var dto = dtos.Single(dto => dto.Id == simulation.Id);
        }

        [Fact]
        public async Task ShouldGetSimulationSharedWithUser()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 100,
                search = "",
                sortColumn = ""
            };

            // Act
            var sharedResult = await _controller.GetSharedScenariosPage(request);

            // Assert
            var okObjResult = sharedResult as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = ((PagingPageModel<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(PagingPageModel<SimulationDTO>))).Items;
            Assert.NotEmpty(dtos);
            var dtoFromThisTest = dtos.Single(dto => dto.Id == simulation.Id);
            Assert.True(dtos.All(_ => _.Owner != TestHelper.UnitOfWork.CurrentUser.Username));
        }

        [Fact]
        public async Task ShouldCreateSimulation()
        {
            var service = Setup();
            // Arrange
            CreateAuthorizedController(service);
            var newSimulationDto = SimulationTestSetup.TestSimulation();
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
            var result =
                await _controller.CreateSimulation(NetworkTestSetup.NetworkId, newSimulationDto) as OkObjectResult;
            var dto = (SimulationDTO)Convert.ChangeType(result!.Value, typeof(SimulationDTO));

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
        public async Task ShouldUpdateSimulation()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);
            TestHelper.UnitOfWork.Context.SaveChanges();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, owner: TestHelper.UnitOfWork.CurrentUser.Id);

            var request = new PagingRequestModel<SimulationDTO>()
            {
                isDescending = false,
                Page = 1,
                RowsPerPage = 5,
                search = "",
                sortColumn = ""
            };

            var getResult = await _controller.GetUserScenariosPage(request);

            var dtos = ((PagingPageModel<SimulationDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(PagingPageModel<SimulationDTO>))).Items;

            var simulationDTO = dtos.Single(s => s.Id == simulation.Id);
            simulationDTO.Name = "Updated Name";
            var testUser = await AddTestUser();
            simulationDTO.Users = new List<SimulationUserDTO>
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
            var result = await _controller.UpdateSimulation(simulationDTO);
            var dto = (SimulationDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(SimulationDTO));

            // Assert
            var simulationEntity = TestHelper.UnitOfWork.Context.Simulation
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Single(_ => _.Id == dto.Id);

            Assert.Equal(dto.Name, simulationEntity.Name);

            var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
            Assert.True(simulationUsers.Count == 2);
            Assert.Equal(dto.Users.Single(_ => _.UserId != TestHelper.UnitOfWork.CurrentUser.Id).UserId,
                simulationUsers.Single(_ => _.UserId != TestHelper.UnitOfWork.CurrentUser.Id).UserId);
        }

        [Fact]
        public async Task ShouldCloneSimulation()
        {
            // Arrange
            var service = Setup();
            CreateAuthorizedController(service);
            CreateTestData();
            var simulationDto = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(_testSimulationToClone.Id);
            var cloneSimulationDto = new CloneSimulationDTO
            {
                networkId = _testSimulationToClone.NetworkId,
                scenarioId = _testSimulationToClone.Id,
                Id = Guid.NewGuid(),
                scenarioName = _testSimulationToClone.Name,
            };

            // Act
            var result = await _controller.CloneSimulation(cloneSimulationDto);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);
            var dto = (SimulationDTO)Convert.ChangeType(okObjResult.Value, typeof(SimulationDTO));

            var originalSimulation = TestHelper.UnitOfWork.Context.Simulation.AsNoTracking().AsSplitQuery()
                // analysis method
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.Benefit)
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budgets
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budget priorities
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // cash flow rules
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.ScenarioCashFlowDistributionRules)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryScenarioCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // investment plan
                .Include(_ => _.InvestmentPlan)
                // committed projects
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.CommittedProjectConsequences)
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.ScenarioBudget)
                // performance curves
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // remaining life limits
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // selectable treatments
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSupersessions)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentSupersessionJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                // deficient condition goals
                .Include(_ => _.ScenarioDeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // target condition goals
                .Include(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SimulationUserJoins)
                .Single(_ => _.Id == _testSimulationToClone.Id);

            var clonedSimulation = TestHelper.UnitOfWork.Context.Simulation.AsNoTracking().AsSplitQuery()
                // analysis method
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.Benefit)
                .Include(_ => _.AnalysisMethod)
                .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budgets
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // budget priorities
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.BudgetPercentagePairs)
                .ThenInclude(_ => _.ScenarioBudget)
                .Include(_ => _.BudgetPriorities)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetPriorityJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // cash flow rules
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.ScenarioCashFlowDistributionRules)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryScenarioCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // investment plan
                .Include(_ => _.InvestmentPlan)
                // committed projects
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.CommittedProjectConsequences)
                .Include(_ => _.CommittedProjects)
                .ThenInclude(_ => _.ScenarioBudget)
                // performance curves
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.ScenarioPerformanceCurveEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.PerformanceCurves)
                .ThenInclude(_ => _.CriterionLibraryScenarioPerformanceCurveJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // remaining life limits
                .Include(_ => _.RemainingLifeLimits)
                .ThenInclude(_ => _.CriterionLibraryScenarioRemainingLifeLimitJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // selectable treatments
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.ScenarioTreatmentCostEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentCosts)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentCostJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSchedulings)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioTreatmentSupersessions)
                .ThenInclude(_ => _.CriterionLibraryScenarioTreatmentSupersessionJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SelectableTreatments)
                .ThenInclude(_ => _.ScenarioSelectableTreatmentScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                // deficient condition goals
                .Include(_ => _.ScenarioDeficientConditionGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioDeficientConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                // target condition goals
                .Include(_ => _.ScenarioTargetConditionalGoals)
                .ThenInclude(_ => _.CriterionLibraryScenarioTargetConditionGoalJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.SimulationUserJoins)
                .Single(_ => _.Id == dto.Id);

            Assert.NotEqual(clonedSimulation.Id, originalSimulation.Id);
            Assert.Equal(clonedSimulation.Name, originalSimulation.Name);
            Assert.NotEqual(clonedSimulation.AnalysisMethod.Id, originalSimulation.AnalysisMethod.Id);
            Assert.Equal(clonedSimulation.AnalysisMethod.Benefit.AttributeId,
                originalSimulation.AnalysisMethod.Benefit.AttributeId);
        }
        [Fact]
        public async Task UserIsViewSimulationAuthorized()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewSimulation,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.SimulationViewPermittedAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Editor }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewSimulation);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteSimulationAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.DeleteSimulation,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.SimulationDeleteAnyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.DeleteSimulation);
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsRunSimulationAuthorized()
        {
            // Admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.RunSimulation,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.SimulationRunAnyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.RunSimulation);
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewSimulationAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policy.ViewSimulation,
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.SimulationViewAnyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, Policy.ViewSimulation);
            // Assert
            Assert.True(allowed.Succeeded);
        }
    }
}
