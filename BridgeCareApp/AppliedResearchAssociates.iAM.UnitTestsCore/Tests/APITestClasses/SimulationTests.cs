using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal
    ;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class SimulationTests
    {
        private readonly TestHelper _testHelper;
        private static SimulationAnalysisService _simulationAnalysisService;
        private SimulationController _controller;

        private UserEntity _testUserEntity;
        private SimulationEntity _testSimulationToClone;
        private const string SimulationName = "Simulation";

        public SimulationTests()
        {
            _testHelper = TestHelper.Instance;
            if (!_testHelper.DbContext.Attribute.Any())
            {
                _testHelper.CreateAttributes();
                _testHelper.CreateNetwork();
                _testHelper.CreateSimulation();
                _testHelper.CreateCalculatedAttributeLibrary();
                _testHelper.SetupDefaultHttpContext();
            }
            _simulationAnalysisService =
                new SimulationAnalysisService(_testHelper.UnitOfWork, new());            
        }

        private void CreateAuthorizedController() =>
            _controller = new SimulationController(_simulationAnalysisService,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateUnauthorizedController() =>
            _controller = new SimulationController(_simulationAnalysisService,
                _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateTestData()
        {
            if (!_testHelper.UnitOfWork.Context.User.Any(u => u.Username == "Clone Tester"))
            {
                _testUserEntity = new UserEntity { Id = Guid.NewGuid(), Username = "Clone Tester" };
                _testHelper.UnitOfWork.Context.AddEntity(_testUserEntity);
                _testHelper.UnitOfWork.SetUser(_testUserEntity.Username);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }

            if (!_testHelper.UnitOfWork.Context.Simulation.Any(s => s.Name == SimulationName))
            {
                var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
                var budgetId = Guid.NewGuid();
                _testSimulationToClone = new SimulationEntity
                {
                    Id = Guid.NewGuid(),
                    Name = SimulationName,
                    NumberOfYearsOfTreatmentOutlook = 1,
                    NetworkId = _testHelper.TestNetwork.Id,
                    SimulationUserJoins = new List<SimulationUserEntity>
                {
                    new SimulationUserEntity
                    {
                        UserId = _testHelper.UnitOfWork.UserEntity.Id,
                        CanModify = true,
                        IsOwner = true,
                        CreatedBy = _testHelper.UnitOfWork.UserEntity.Id,
                        LastModifiedBy = _testHelper.UnitOfWork.UserEntity.Id
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
                        new CommittedProjectEntity
                        {
                            Id = Guid.NewGuid(),
                            Cost = 500000,
                            Name = "Committed Project",
                            Year = DateTime.Now.Year,
                            ShadowForAnyTreatment = 1,
                            ShadowForSameTreatment = 1,
                            ScenarioBudgetId = budgetId,
                            MaintainableAsset = new MaintainableAssetEntity
                            {
                                Id = Guid.NewGuid(),
                                NetworkId = _testHelper.TestNetwork.Id,
                                FacilityName = "Facility",
                                SectionName = "Section",
                                SpatialWeighting = "SpatialWeighting",
                                MaintainableAssetLocation = new MaintainableAssetLocationEntity
                                {
                                    Id = Guid.NewGuid(),
                                    Discriminator = "SectionLocation",
                                    LocationIdentifier = "FacilitySection",
                                }
                            },
                            CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>
                            {
                                new CommittedProjectConsequenceEntity
                                {
                                    Id = Guid.NewGuid(), AttributeId = attribute.Id, ChangeValue = "+1"
                                }
                            }
                        }
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
                _testHelper.UnitOfWork.Context.AddEntity(_testSimulationToClone);
                _testHelper.UnitOfWork.Context.SaveChanges();
            }
        }

        [Fact]
        public async void ShouldDeleteSimulation()
        {
            // Arrange
            CreateAuthorizedController();

            var getResult = await _controller.GetSimulations();
            var dtos = (List<SimulationDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<SimulationDTO>));

            var dto = dtos[0];

            // Act
            await _controller.DeleteSimulation(dto.Id);

            // Assert
            Assert.True(!_testHelper.UnitOfWork.Context.Simulation.Any(_ => _.Id == dto.Id));
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.GetSimulations();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var dto = _testHelper.TestSimulation.ToDto(null);
            dto.Id = Guid.NewGuid();
            var result = await _controller.CreateSimulation(_testHelper.TestNetwork.Id, dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void ShouldReturnOkResultOnPut()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.UpdateSimulation(_testHelper.TestSimulation.ToDto(null));

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.DeleteSimulation(Guid.Empty);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void ShouldGetAllSimulations()
        {
            // Arrange
            CreateAuthorizedController();

            // Act
            var result = await _controller.GetSimulations();

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);

            var dtos = (List<SimulationDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<SimulationDTO>));
            Assert.Single(dtos);

            Assert.Equal(_testHelper.TestSimulation.Id, dtos[0].Id);
        }

        [Fact]
        public async void ShouldCreateSimulation()
        {
            // Arrange
            CreateAuthorizedController();

            var newSimulationDTO = _testHelper.TestSimulation.ToDto(null);
            newSimulationDTO.Id = Guid.NewGuid();
            newSimulationDTO.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = _testHelper.TestUser.Id,
                        Username = _testHelper.TestUser.Username,
                        CanModify = true,
                        IsOwner = true
                    }
                };

            // Act
            var result =
                await _controller.CreateSimulation(_testHelper.TestNetwork.Id, newSimulationDTO) as OkObjectResult;
            var dto = (SimulationDTO)Convert.ChangeType(result!.Value, typeof(SimulationDTO));

            // Assert
            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var simulationEntity = _testHelper.UnitOfWork.Context.Simulation
                    .Include(_ => _.SimulationUserJoins)
                    .ThenInclude(_ => _.User)
                    .SingleOrDefault(_ => _.Id == dto.Id);

                Assert.NotNull(simulationEntity);
                Assert.Equal(dto.Users[0].UserId, simulationEntity.CreatedBy);

                var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
                Assert.Single(simulationUsers);
                Assert.Equal(dto.Users[0].UserId, simulationUsers[0].UserId);
            };
        }

        [Fact]
        public async void ShouldUpdateSimulation()
        {
            // Arrange
            CreateAuthorizedController();
            _testHelper.UnitOfWork.Context.AddEntity(_testHelper.TestUser);
            _testHelper.UnitOfWork.Context.SaveChanges();

            var getResult = await _controller.GetSimulations();
            var dtos = (List<SimulationDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                typeof(List<SimulationDTO>));

            var simulationDTO = dtos[0];
            simulationDTO.Name = "Updated Name";
            simulationDTO.Users = new List<SimulationUserDTO>
                {
                    new SimulationUserDTO
                    {
                        UserId = _testHelper.TestUser.Id,
                        Username = _testHelper.TestUser.Username,
                        CanModify = true,
                        IsOwner = true
                    }
                };

            // Act
            var result = await _controller.UpdateSimulation(simulationDTO);
            var dto = (SimulationDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(SimulationDTO));

            // Assert
            var simulationEntity = _testHelper.UnitOfWork.Context.Simulation
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Single(_ => _.Id == dto.Id);

            Assert.Equal(dto.Name, simulationEntity.Name);

            var simulationUsers = simulationEntity.SimulationUserJoins.ToList();
            Assert.Single(simulationUsers);
            Assert.Equal(dto.Users[0].UserId, simulationUsers[0].UserId);
        }

        [Fact]
        public async void ShouldCloneSimulation()
        {
            // Arrange
            CreateAuthorizedController();
            CreateTestData();

            // Act
            var result = await _controller.CloneSimulation(_testSimulationToClone.Id);

            // Assert
            var okObjResult = result as OkObjectResult;
            Assert.NotNull(okObjResult.Value);
            var dto = (SimulationDTO)Convert.ChangeType(okObjResult.Value, typeof(SimulationDTO));

            var timer = new Timer { Interval = 5000 };
            timer.Elapsed += delegate
            {
                var originalSimulation = _testHelper.UnitOfWork.Context.Simulation.AsNoTracking().AsSplitQuery()
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

                var clonedSimulation = _testHelper.UnitOfWork.Context.Simulation.AsNoTracking().AsSplitQuery()
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
                Assert.Equal(clonedSimulation.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId,
                    originalSimulation.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.CriterionLibraryId);
                Assert.NotEqual(clonedSimulation.InvestmentPlan.Id, originalSimulation.InvestmentPlan.Id);
                Assert.Equal(clonedSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod,
                    originalSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod);
                var clonedCommittedProjects = clonedSimulation.CommittedProjects.ToList();
                var originalCommittedProjects = originalSimulation.CommittedProjects.ToList();
                Assert.Equal(clonedCommittedProjects.Count, originalCommittedProjects.Count);
                Assert.NotEqual(clonedCommittedProjects[0].Id, originalCommittedProjects[0].Id);
                Assert.Equal(clonedCommittedProjects[0].Name, originalCommittedProjects[0].Name);
                var clonedCommittedProjectConsequences =
                    clonedCommittedProjects[0].CommittedProjectConsequences.ToList();
                var originalCommittedProjectConsequences =
                    originalCommittedProjects[0].CommittedProjectConsequences.ToList();
                Assert.Equal(clonedCommittedProjectConsequences.Count, originalCommittedProjectConsequences.Count);
                Assert.NotEqual(clonedCommittedProjectConsequences[0].Id,
                    originalCommittedProjectConsequences[0].Id);
                Assert.Equal(clonedCommittedProjectConsequences[0].AttributeId,
                    originalCommittedProjectConsequences[0].AttributeId);
                var clonedSimulationUsers = clonedSimulation.SimulationUserJoins.ToList();
                var originalSimulationUsers = originalSimulation.SimulationUserJoins.ToList();
                Assert.Equal(clonedSimulationUsers.Count, originalSimulationUsers.Count);
                Assert.NotEqual(clonedSimulationUsers[0].SimulationId, originalSimulationUsers[0].SimulationId);
                Assert.Equal(clonedSimulationUsers[0].IsOwner, originalSimulationUsers[0].IsOwner);

                Assert.Equal(clonedSimulation.PerformanceCurves.Count, originalSimulation.PerformanceCurves.Count);
                var clonedCurveIds = clonedSimulation.PerformanceCurves.Select(_ => _.Id).ToList();
                var originalCurveIds = originalSimulation.PerformanceCurves.Select(_ => _.Id).ToList();
                var curveIdsDiff = clonedCurveIds.Except(originalCurveIds).ToList();
                Assert.NotEmpty(curveIdsDiff);
                Assert.Equal(curveIdsDiff.Count, clonedSimulation.PerformanceCurves.Count);
                Assert.True(clonedSimulation.PerformanceCurves.All(_ => curveIdsDiff.Contains(_.Id)));

                var clonedCriteria =
                    clonedSimulation.PerformanceCurves.Where(_ =>
                            _.CriterionLibraryScenarioPerformanceCurveJoin != null)
                        .Select(_ => _.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary).ToList();
                var originalCriteria =
                    originalSimulation.PerformanceCurves.Where(_ =>
                            _.CriterionLibraryScenarioPerformanceCurveJoin != null)
                        .Select(_ => _.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary).ToList();
                Assert.Equal(clonedCriteria.Count, originalCriteria.Count);
                var clonedCriteriaIds = clonedCriteria.Select(_ => _.Id).ToList();
                var originalCriteriaIds = originalCriteria.Select(_ => _.Id).ToList();
                var criteriaIdsDiff = clonedCriteriaIds.Except(originalCriteriaIds).ToList();
                Assert.NotEmpty(criteriaIdsDiff);
                Assert.Equal(criteriaIdsDiff.Count, clonedCriteria.Count);
                Assert.True(clonedCriteria.All(_ => criteriaIdsDiff.Contains(_.Id)));

                var clonedEquations =
                    clonedSimulation.PerformanceCurves.Where(_ =>
                            _.ScenarioPerformanceCurveEquationJoin != null)
                        .Select(_ => _.ScenarioPerformanceCurveEquationJoin.Equation).ToList();
                var originalEquations =
                    originalSimulation.PerformanceCurves.Where(_ =>
                            _.ScenarioPerformanceCurveEquationJoin != null)
                        .Select(_ => _.ScenarioPerformanceCurveEquationJoin.Equation).ToList();
                Assert.Equal(clonedCriteria.Count, originalCriteria.Count);
                var clonedEquationIds = clonedEquations.Select(_ => _.Id).ToList();
                var originalEquationIds = originalEquations.Select(_ => _.Id).ToList();
                var equationIdsDiff = clonedEquationIds.Except(originalEquationIds).ToList();
                Assert.NotEmpty(equationIdsDiff);
                Assert.Equal(equationIdsDiff.Count, clonedCriteria.Count);
                Assert.True(clonedEquations.All(_ => equationIdsDiff.Contains(_.Id)));

                Assert.Equal(clonedSimulation.SelectableTreatments.Count,
                    originalSimulation.SelectableTreatments.Count);
                var clonedTreatmentIds = clonedSimulation.SelectableTreatments.Select(_ => _.Id).ToList();
                var originalTreatmentIds = originalSimulation.SelectableTreatments.Select(_ => _.Id).ToList();
                var TreatmentIdsDiff = clonedTreatmentIds.Except(originalTreatmentIds).ToList();
                Assert.NotEmpty(TreatmentIdsDiff);
                Assert.Equal(TreatmentIdsDiff.Count, clonedSimulation.SelectableTreatments.Count);
                Assert.True(clonedSimulation.SelectableTreatments.All(_ => TreatmentIdsDiff.Contains(_.Id)));

                clonedCriteria =
                    clonedSimulation.SelectableTreatments.Where(_ =>
                            _.CriterionLibraryScenarioSelectableTreatmentJoin != null)
                        .Select(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary).ToList();
                originalCriteria =
                    originalSimulation.SelectableTreatments.Where(_ =>
                            _.CriterionLibraryScenarioSelectableTreatmentJoin != null)
                        .Select(_ => _.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary).ToList();
                Assert.Equal(clonedCriteria.Count, originalCriteria.Count);

                clonedCriteriaIds = clonedCriteria.Select(_ => _.Id).ToList();
                originalCriteriaIds = originalCriteria.Select(_ => _.Id).ToList();
                criteriaIdsDiff = clonedCriteriaIds.Except(originalCriteriaIds).ToList();
                Assert.NotEmpty(criteriaIdsDiff);
                Assert.Equal(criteriaIdsDiff.Count, clonedCriteria.Count);
                Assert.True(clonedCriteria.All(_ => criteriaIdsDiff.Contains(_.Id)));
                    //[TODO]: add more scenarios for the treatment
                };
        }
    }
}
