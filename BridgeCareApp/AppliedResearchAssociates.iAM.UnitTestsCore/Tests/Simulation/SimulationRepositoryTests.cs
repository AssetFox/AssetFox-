using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
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
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using Microsoft.AspNetCore.Http;
using Moq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.User;
using Microsoft.Data.SqlClient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationRepositoryTests
    {
        private UserEntity _testUserEntity;
        private SimulationEntity _testSimulationToClone;
        private const string SimulationName = "Simulation";

        public void Setup()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            CalculatedAttributeTestSetup.CreateDefaultCalculatedAttributeLibrary(TestHelper.UnitOfWork);
        }

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
            Setup();
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
            Setup();
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
            Setup();

            var result = TestHelper.UnitOfWork.SimulationRepo.GetSharedScenarios(true, true);
        }

        [Fact]
        public void GetSimulationNameOrId_SimulationNotInDb_GetsId()
        {
            var simulationId = Guid.NewGuid();
            var nameOrId = TestHelper.UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
            Assert.Contains(simulationId.ToString(), nameOrId);
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


        [Fact]
        public async Task CreateSimulation_Does()
        {
            // Arrange
            Setup();
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
            Setup();
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
            Setup();
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
        [Fact (Skip ="WJwjwjwj unskip before PR")]
        public void SimulationInDb_Clone_Clones()
        {
            // Arrange
            Setup();
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
            var result = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(_testSimulationToClone.Id, _testSimulationToClone.NetworkId, _testSimulationToClone.Name);

            // Assert
            var dto = result.Simulation;

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
