using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.TestHelpers.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using BridgeCareCore.Services.SimulationCloning;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class OldStyleSimulationCloningTests
    {
        private SimulationEntity _testSimulationToClone;
        private UserEntity _testUserEntity;
        private const string SimulationName = "Simulation";

        private static ICompleteSimulationCloningService CreateCompleteSimulationCloningService()
        {
            var service = new CompleteSimulationCloningService(TestHelper.UnitOfWork);
            return service;
        }

        [Fact]
        public void SimulationInDb_Clone_Clones()
        {
            // Arrange
            SimulationRepositoryTestSetup.Setup();
            CreateTestData();
            var simulationDto = TestHelper.UnitOfWork.SimulationRepo.GetSimulation(_testSimulationToClone.Id);
            var cloneSimulationDto = new CloneSimulationDTO
            {
                NetworkId = _testSimulationToClone.NetworkId,
                SourceScenarioId = _testSimulationToClone.Id,
                Id = Guid.NewGuid(),
                ScenarioName = _testSimulationToClone.Name,
            };

            // Act
            //var result = TestHelper.UnitOfWork.SimulationRepo.CloneSimulation(_testSimulationToClone.Id, _testSimulationToClone.NetworkId, _testSimulationToClone.Name);
            //var result = TestHelper.UnitOfWork.CompleteSimulationRepo.Clone(cloneSimulationDto);
            var cloningService = CreateCompleteSimulationCloningService();
            var result = cloningService.Clone(cloneSimulationDto);

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

            foreach (var treatment in originalSimulation.SelectableTreatments)
            {
                treatment.ScenarioTreatmentSupersessions = new List<ScenarioTreatmentSupersessionEntity>();
                treatment.ScenarioTreatmentSchedulings = new List<ScenarioTreatmentSchedulingEntity>();
            }

            Assert.NotEqual(clonedSimulation.Id, originalSimulation.Id);
            Assert.Equal(clonedSimulation.Name, originalSimulation.Name);
            Assert.NotEqual(clonedSimulation.AnalysisMethod.Id, originalSimulation.AnalysisMethod.Id);
            Assert.Equal(clonedSimulation.AnalysisMethod.Benefit.AttributeId,
                originalSimulation.AnalysisMethod.Benefit.AttributeId);
            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var serializeOriginal = JsonConvert.SerializeObject(originalSimulation, Formatting.Indented, jsonSettings);
            var serializeClone = JsonConvert.SerializeObject(clonedSimulation, Formatting.Indented, jsonSettings);
            var originalLines = StringExtensions.ToLines(serializeOriginal);
            var cloneLines = StringExtensions.ToLines(serializeClone);
            var lastModifiedDate = @"""LastModifiedDate""";
            var createdDate = @"""CreatedDate""";
            var createdBy = @"""CreatedBy""";
            var lastModifiedBy = @"""LastModifiedBy""";
            var category = @"""Category""";
            var id = @"Id""";
            var ignoreProperties = new List<string> { createdDate, createdBy, lastModifiedDate, lastModifiedBy, category };
            var expectedCreatedBy = "";
            var expectedLastModifiedBy = "";
            var numberOfYearsOfTreatmentOutlook = @"""NumberOfYearsOfTreatmentOutlook""";
            var byProperties = new List<string> { createdBy, lastModifiedBy };
            for (int i = 0; i < originalLines.Count; i++)
            {
                var originalLine = originalLines[i];
                var cloneLine = cloneLines[i];
                var trimOriginal = originalLine.Trim();
                var trimClone = cloneLine.Trim();
                if (trimOriginal.StartsWith(createdBy))
                {
                    Assert.StartsWith(createdBy, trimClone);
                    if (expectedCreatedBy == "")
                    {
                        expectedCreatedBy = trimClone;
                    }
                    else
                    {
                        Assert.Equal(expectedCreatedBy, trimClone);
                    }
                }
                if (trimClone.StartsWith(lastModifiedBy))
                {
                    Assert.StartsWith(lastModifiedBy, trimClone);
                    if (expectedLastModifiedBy == "")
                    {
                        expectedLastModifiedBy = trimClone;
                    }
                    else
                    {
                        Assert.Equal(expectedLastModifiedBy, trimClone);
                    }
                }
                var bothLines = originalLine + Environment.NewLine + cloneLine;

                if (originalLine != cloneLine)
                {
                    bool shouldContinue = false;
                    foreach (var ignore in ignoreProperties)
                    {
                        if (originalLine.Trim().StartsWith(ignore) && cloneLine.Trim().StartsWith(ignore))
                        {
                            shouldContinue = true;
                            break;
                        }
                    }
                    if (shouldContinue)
                    {
                        continue;
                    }
                    if (originalLine.Contains(id) && cloneLine.Contains(id))
                    {
                        continue;
                    }
                    if (trimClone.StartsWith(numberOfYearsOfTreatmentOutlook) && trimOriginal.StartsWith(numberOfYearsOfTreatmentOutlook) && trimClone.EndsWith("100,"))
                    {
                        continue;
                    }
                    Assert.Equal(originalLine, cloneLine);
                }
            }
            Assert.Equal(originalLines.Count, cloneLines.Count);
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
                    ScenarioBudgetId = budgetId
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
    }
}
