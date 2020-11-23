using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataAccess;
using DA = AppliedResearchAssociates.iAM.DataAssignment;
using DM = AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using MoreEnumerable = MoreLinq.MoreEnumerable;


namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationAnalysisDataPersistenceTests
    {
        [Fact]
        public void TestCreateNetwork()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.TestCreateNetworkSetup();
            var simulation = testHelper.GetStandAloneSimulation();

            // Act
            testHelper.CreateNetwork(simulation);
            testHelper.FacilityRepo.CreateFacilities(simulation.Network.Facilities.ToList(), simulation.Network.Name);

            // Assert
            var dataSourceNetwork = testHelper.NetworkRepo.GetSimulationAnalysisNetwork(simulation.Network.Name);
            Assert.NotNull(dataSourceNetwork);

            var facilities = simulation.Network.Facilities.ToList();
            var dataSourceFacilities = dataSourceNetwork.Facilities.ToList();
            Assert.Equal(facilities.Count(), dataSourceFacilities.Count());
            facilities.ForEach(facility =>
            {
                var dataSourceFacility = dataSourceFacilities.SingleOrDefault(_ => _.Name == facility.Name);
                Assert.NotNull(dataSourceFacility);
                var sections = facility.Sections.ToList();
                var dataSourceSections = dataSourceFacility.Sections.ToList();
                Assert.Equal(sections.Count(), dataSourceSections.Count());
                sections.ForEach(section =>
                {
                    var dataSourceSection = dataSourceSections.SingleOrDefault(_ => _.Name == section.Name);
                    Assert.NotNull(dataSourceSection);
                    Assert.Equal(section.Area, dataSourceSection.Area);
                    Assert.Equal(section.AreaUnit, dataSourceSection.AreaUnit);
                });
            });
        }

        [Fact]
        public void TestCreateLiteSimulationEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreateSimulationEntitySetup(simulation);

            // Act
            testHelper.SimulationRepo.CreateSimulation(simulation);

            // Assert
            var dataSourceSimulations = testHelper.SimulationRepo
                .GetAllInNetwork(simulation.Network.Name).ToList();
            Assert.Single(dataSourceSimulations);
            Assert.Equal(simulation.Name, dataSourceSimulations[0].Name);
            Assert.Equal(simulation.NumberOfYearsOfTreatmentOutlook, dataSourceSimulations[0].NumberOfYearsOfTreatmentOutlook);

            // CleanUp
            testHelper.CleanUp();
        }

        [Fact]
        public void TestCreateSimulationOutput()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreateSimulationEntitySetup(simulation);
            var runner = new SimulationRunner(simulation);
            var simulationIsRunning = true;
            runner.Information += (sender, eventArgs) => {
                if (eventArgs.Message == "Simulation complete.")
                {
                    simulationIsRunning = false;
                }
            };
            runner.Run();

            while (simulationIsRunning)
            {
                ITestOutputHelper outputHelper = new TestOutputHelper();
                outputHelper.WriteLine("Simulation is running...");
            }

            var folderPathForNewAnalysis = $"DownloadedReports\\{simulation.Name}_NewAnalysis";
            var relativeFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForNewAnalysis);
            Directory.CreateDirectory(relativeFolderPath);

            var outputFile = $"Network {simulation.Network.Name} - Simulation {simulation.Name}.json";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderPathForNewAnalysis, outputFile);
            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();

            var resultObject = JsonConvert.SerializeObject(simulation.Results, settings);
            File.WriteAllText(filePath, resultObject);

            // Act
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            testHelper.SimulationRepo.CreateSimulation(simulation);

            // Assert
            var dataSourceSimulations = testHelper.SimulationRepo
                .GetAllInNetwork(simulation.Network.Name).ToList();
            Assert.Single(dataSourceSimulations);
            Assert.Equal(simulation.Name, dataSourceSimulations[0].Name);
            Assert.Equal(simulation.NumberOfYearsOfTreatmentOutlook, dataSourceSimulations[0].NumberOfYearsOfTreatmentOutlook);

            // CleanUp
            testHelper.CleanUp();
        }

        [Fact]
        public void TestCreateInvestmentPlanEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreateInvestmentPlanEntitySetup(simulation);

            // Act
            testHelper.InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Name);

            // Assert
            var dataSourceInvestmentPlan = testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation.Name);
            Assert.NotNull(dataSourceInvestmentPlan);
            Assert.IsType<InvestmentPlan>(dataSourceInvestmentPlan);

            var investmentPlan = simulation.InvestmentPlan;
            Assert.Equal(investmentPlan.FirstYearOfAnalysisPeriod, dataSourceInvestmentPlan.FirstYearOfAnalysisPeriod);
            Assert.Equal(investmentPlan.InflationRatePercentage, dataSourceInvestmentPlan.InflationRatePercentage);
            Assert.Equal(investmentPlan.MinimumProjectCostLimit, dataSourceInvestmentPlan.MinimumProjectCostLimit);
            Assert.Equal(investmentPlan.NumberOfYearsInAnalysisPeriod, dataSourceInvestmentPlan.NumberOfYearsInAnalysisPeriod);

            var budgets = investmentPlan.Budgets.ToList();
            var dataSourceBudgets = dataSourceInvestmentPlan.Budgets.ToList();
            var budgetConditions = investmentPlan.BudgetConditions.ToList();
            var dataSourceBudgetConditions = dataSourceInvestmentPlan.BudgetConditions.ToList();
            var cashFlowRules = investmentPlan.CashFlowRules.ToList();
            var dataSourceCashFlowRules = dataSourceInvestmentPlan.CashFlowRules.ToList();

            Assert.Equal(budgets.Count(), dataSourceBudgets.Count());
            budgets.ForEach(budget =>
            {
                var dataSourceBudget = dataSourceBudgets.Find(_ => _.Name == budget.Name);

                Assert.NotNull(dataSourceBudget);

                if (budget.YearlyAmounts.Any())
                {
                    Assert.Equal(budget.YearlyAmounts.Count(), dataSourceBudget.YearlyAmounts.Count());

                    for (var index = 0; index < budget.YearlyAmounts.Count(); index++)
                    {
                        Assert.Equal(budget.YearlyAmounts[index].Value, dataSourceBudget.YearlyAmounts[index].Value);
                        index++;
                    }
                }

                if (budgetConditions.Any())
                {
                    Assert.Equal(budgetConditions.Count(), dataSourceBudgetConditions.Count());

                    var budgetCondition = budgetConditions.Single(_ => _.Budget.Name == budget.Name);
                    var dataSourceBudgetCondition = dataSourceBudgetConditions.Find(_ => _.Budget.Name == dataSourceBudget.Name);

                    Assert.NotNull(dataSourceBudgetCondition);
                    Assert.Equal(budgetCondition.Criterion.Expression, dataSourceBudgetCondition.Criterion.Expression);
                }
            });

            Assert.Equal(cashFlowRules.Count(), dataSourceCashFlowRules.Count());
            cashFlowRules.ForEach(cashFlowRule =>
            {
                var dataSourceCashFlowRule = dataSourceCashFlowRules.Find(__ => __.Name == cashFlowRule.Name);

                Assert.NotNull(dataSourceCashFlowRule);
                Assert.Equal(cashFlowRule.Criterion.Expression, dataSourceCashFlowRule.Criterion.Expression);

                if (cashFlowRule.DistributionRules.Any())
                {
                    var dataSourceDistributionRules = dataSourceCashFlowRule.DistributionRules.ToList();

                    Assert.Equal(cashFlowRule.DistributionRules.Count(), dataSourceDistributionRules.Count());

                    cashFlowRule.DistributionRules.ForEach((distributionRule, index) =>
                    {
                        Assert.Equal(distributionRule.CostCeiling, dataSourceDistributionRules[index].CostCeiling);
                        Assert.Equal(distributionRule.Expression, dataSourceDistributionRules[index].Expression);
                    });
                }
            });

            // CleanUp
            testHelper.CleanUp();
        }

        [Fact]
        public void TestCreatePerformanceCurveEntities()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreatePerformanceCurveEntitiesSetup(simulation);

            // Act
            testHelper.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Name);

            // Assert
            var dataSourcePerformanceCurves = testHelper.PerformanceCurveRepo
                .GetSimulationPerformanceCurves(simulation.Name);
            Assert.NotNull(dataSourcePerformanceCurves);
            Assert.Equal(simulation.PerformanceCurves.Count(), dataSourcePerformanceCurves.Count());

            simulation.PerformanceCurves.ForEach(curve =>
            {
                var dataSourceCurve = dataSourcePerformanceCurves.Find(_ => _.Name == curve.Name);
                Assert.NotNull(dataSourceCurve);
                Assert.IsType<PerformanceCurve>(dataSourceCurve);
                Assert.Equal(curve.Attribute.Name, dataSourceCurve.Attribute.Name);
                Assert.Equal(curve.Attribute.IsDecreasingWithDeterioration, dataSourceCurve.Attribute.IsDecreasingWithDeterioration);
                Assert.Equal(curve.Attribute.DefaultValue, dataSourceCurve.Attribute.DefaultValue);
                Assert.Equal(curve.Attribute.Maximum, dataSourceCurve.Attribute.Maximum);
                Assert.Equal(curve.Attribute.Minimum, dataSourceCurve.Attribute.Minimum);
                Assert.Equal(curve.Criterion.Expression, dataSourceCurve.Criterion.Expression);
                Assert.Equal(curve.Equation.Expression, dataSourceCurve.Equation.Expression);
                Assert.Equal(curve.Shift, dataSourceCurve.Shift);
            });

            // CleanUp
            testHelper.CleanUp();
        }

        [Fact]
        public void TestCreateSelectableTreatmentEntities()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreateSelectableTreatmentEntitiesSetup(simulation);

            // Act
            testHelper.SelectableTreatmentRepo
                .CreateTreatmentLibrary($"{simulation.Name} Simulation Treatment Library", simulation.Name);
            testHelper.SelectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Name);

            // Assert
            var dataSourceTreatments = testHelper.SelectableTreatmentRepo.GetSimulationTreatments(simulation.Name);
            Assert.Equal(simulation.Treatments.Count(), dataSourceTreatments.Count());

            simulation.Treatments.ForEach(treatment =>
            {
                var dataSourceTreatment = dataSourceTreatments.SingleOrDefault(_ => _.Name == treatment.Name);
                Assert.NotNull(dataSourceTreatment);
                Assert.Equal(treatment.ShadowForAnyTreatment, dataSourceTreatment.ShadowForAnyTreatment);
                Assert.Equal(treatment.ShadowForSameTreatment, dataSourceTreatment.ShadowForSameTreatment);
                Assert.Equal(treatment.Description, dataSourceTreatment.Description);

                var budgets = treatment.Budgets.OrderBy(_ => _.Name).ToList();
                var dataSourceBudgets = dataSourceTreatment.Budgets.OrderBy(_ => _.Name).ToList();
                Assert.Equal(budgets.Count(), dataSourceBudgets.Count());
                budgets.ForEach(budget =>
                {
                    var dataSourceBudget = dataSourceBudgets.SingleOrDefault(_ => _.Name == budget.Name);
                    Assert.NotNull(dataSourceBudget);
                    Assert.Equal(budget.YearlyAmounts.Count(), dataSourceBudget.YearlyAmounts.Count());
                    budget.YearlyAmounts.ForEach((budgetAmount, index) =>
                    {
                        Assert.Equal(budgetAmount.Value, dataSourceBudget.YearlyAmounts[index].Value);
                    });
                });

                var consequences = treatment.Consequences.ToList();
                var dataSourceConsequences = dataSourceTreatment.Consequences.ToList();
                Assert.Equal(consequences.Count(), dataSourceConsequences.Count());
                consequences.ForEach((consequence, index) =>
                {
                    var dataSourceConsequence = dataSourceConsequences[index];
                    Assert.Equal(consequence.Attribute.Name, dataSourceConsequence.Attribute.Name);
                    Assert.Equal(consequence.Change.Expression, dataSourceConsequence.Change.Expression);
                    Assert.Equal(consequence.Equation.Expression, dataSourceConsequence.Equation.Expression);
                    Assert.Equal(consequence.Criterion.Expression, dataSourceConsequence.Criterion.Expression);
                });

                var costs = treatment.Costs.ToList();
                var dataSourceCosts = dataSourceTreatment.Costs.ToList();
                Assert.Equal(costs.Count(), dataSourceCosts.Count());
                costs.ForEach((cost, index) =>
                {
                    var dataSourceCost = dataSourceCosts[index];
                    Assert.Equal(cost.Equation.Expression, dataSourceCost.Equation.Expression);
                    Assert.Equal(cost.Criterion.Expression, dataSourceCost.Criterion.Expression);
                });

                var feasibilityCriteria = treatment.FeasibilityCriteria.ToList();
                var dataSourceFeasibilityCriteria = dataSourceTreatment.FeasibilityCriteria.ToList();
                Assert.Equal(feasibilityCriteria.Count(), dataSourceFeasibilityCriteria.Count());
                feasibilityCriteria.ForEach((feasibility, index) =>
                {
                    Assert.Equal(feasibility.Expression, dataSourceFeasibilityCriteria[index].Expression);
                });

                var treatmentSchedulings = treatment.Schedulings.ToList();
                var dataSourceTreatmentSchedulings = dataSourceTreatment.Schedulings.ToList();
                Assert.Equal(treatmentSchedulings.Count(), dataSourceTreatmentSchedulings.Count());
                treatmentSchedulings.ForEach((treatmentScheduling, index) =>
                {
                    Assert.Equal(treatmentScheduling.OffsetToFutureYear, dataSourceTreatmentSchedulings[index].OffsetToFutureYear);
                });

                var treatmentSupersessions = treatment.Supersessions.ToList();
                var dataSourceTreatmentSupersessions = dataSourceTreatment.Supersessions.ToList();
                Assert.Equal(treatmentSupersessions.Count(), dataSourceTreatmentSupersessions.Count());
                dataSourceTreatmentSupersessions.ForEach((supersession, index) =>
                {
                    Assert.Equal(supersession.Criterion.Expression, dataSourceTreatmentSupersessions[index].Criterion.Expression);
                });
            });

            // CleanUp
            testHelper.CleanUp();
        }

        [Fact]
        public void TestCreateAnalysisMethodEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.TestCreateAnalysisMethodEntitySetup(simulation);

            // Act
            testHelper.AnalysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Name);

            // Assert
            var analysisMethod = simulation.AnalysisMethod;
            var dataSourceAnalysisMethod = testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation.Name);
            Assert.NotNull(dataSourceAnalysisMethod);
            Assert.IsType<AnalysisMethod>(dataSourceAnalysisMethod);
            Assert.Equal(analysisMethod.Weighting.Name, dataSourceAnalysisMethod.Weighting.Name);
            Assert.Equal(analysisMethod.Weighting.IsDecreasingWithDeterioration, dataSourceAnalysisMethod.Weighting.IsDecreasingWithDeterioration);
            Assert.Equal(analysisMethod.OptimizationStrategy, dataSourceAnalysisMethod.OptimizationStrategy);
            Assert.Equal(analysisMethod.SpendingStrategy, dataSourceAnalysisMethod.SpendingStrategy);
            Assert.Equal(analysisMethod.Description, dataSourceAnalysisMethod.Description);
            Assert.Equal(analysisMethod.ShouldDeteriorateDuringCashFlow, dataSourceAnalysisMethod.ShouldDeteriorateDuringCashFlow);
            Assert.Equal(analysisMethod.ShouldUseExtraFundsAcrossBudgets, dataSourceAnalysisMethod.ShouldUseExtraFundsAcrossBudgets);
            Assert.Equal(analysisMethod.ShouldApplyMultipleFeasibleCosts, dataSourceAnalysisMethod.ShouldApplyMultipleFeasibleCosts);
            Assert.Equal(analysisMethod.Filter.Expression, dataSourceAnalysisMethod.Filter.Expression);

            Assert.Equal(analysisMethod.Benefit.Attribute.Name, dataSourceAnalysisMethod.Benefit.Attribute.Name);
            Assert.Equal(analysisMethod.Benefit.Attribute.IsDecreasingWithDeterioration, dataSourceAnalysisMethod.Benefit.Attribute.IsDecreasingWithDeterioration);
            Assert.Equal(analysisMethod.Benefit.Limit, dataSourceAnalysisMethod.Benefit.Limit);

            var budgetPriorities = analysisMethod.BudgetPriorities.ToList();
            var dataSourceBudgetPriorities = dataSourceAnalysisMethod.BudgetPriorities.ToList();
            Assert.Equal(budgetPriorities.Count(), dataSourceBudgetPriorities.Count());
            budgetPriorities.ForEach((priority, index) =>
            {
                var dataSourcePriority = dataSourceBudgetPriorities[index];
                Assert.Equal(priority.Criterion.Expression, dataSourcePriority.Criterion.Expression);
                Assert.Equal(priority.PriorityLevel, dataSourcePriority.PriorityLevel);
                Assert.Equal(priority.Year, dataSourcePriority.Year);

                var budgetPercentagePairs = priority.BudgetPercentagePairs.ToList();
                var dataSourceBudgetPercentagePairs = dataSourcePriority.BudgetPercentagePairs.ToList();
                Assert.Equal(budgetPercentagePairs.Count(), dataSourceBudgetPercentagePairs.Count());
                budgetPercentagePairs.ForEach((budgetPercentagePair, subIndex) =>
                {
                    var dataSourceBudgetPercentagePair = dataSourceBudgetPercentagePairs[subIndex];
                    Assert.Equal(budgetPercentagePair.Percentage, dataSourceBudgetPercentagePair.Percentage);
                    Assert.Equal(budgetPercentagePair.Budget.Name, dataSourceBudgetPercentagePair.Budget.Name);
                    var yearlyAmounts = budgetPercentagePair.Budget.YearlyAmounts.ToList();

                    var dataSourceYearlyAmounts = dataSourceBudgetPercentagePair.Budget.YearlyAmounts.ToList();
                    Assert.Equal(yearlyAmounts.Count(), dataSourceYearlyAmounts.Count());
                    yearlyAmounts.ForEach((yearlyAmount, subSubIndex) =>
                    {
                        Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmounts[subSubIndex].Value);
                    });
                });
            });

            var targetConditionGoals = analysisMethod.TargetConditionGoals.ToList();
            var dataSourceTargetConditionGoals = dataSourceAnalysisMethod.TargetConditionGoals.ToList();
            Assert.Equal(targetConditionGoals.Count(), dataSourceTargetConditionGoals.Count());
            targetConditionGoals.ForEach(targetConditionGoal =>
            {
                var dataSourceTargetConditionGoal = dataSourceTargetConditionGoals
                    .SingleOrDefault(_ => _.Name == targetConditionGoal.Name);
                Assert.NotNull(dataSourceTargetConditionGoal);
                Assert.Equal(targetConditionGoal.Attribute.Name, dataSourceTargetConditionGoal.Attribute.Name);
                Assert.Equal(targetConditionGoal.Target, dataSourceTargetConditionGoal.Target);
                Assert.Equal(targetConditionGoal.Year, dataSourceTargetConditionGoal.Year);
                Assert.Equal(targetConditionGoal.Criterion.Expression, dataSourceTargetConditionGoal.Criterion.Expression);
            });

            var deficientConditionGoals = analysisMethod.DeficientConditionGoals.ToList();
            var dataSourceDeficientConditionGoals = dataSourceAnalysisMethod.DeficientConditionGoals.ToList();
            Assert.Equal(deficientConditionGoals.Count(), dataSourceDeficientConditionGoals.Count());
            deficientConditionGoals.ForEach(deficientConditionGoal =>
            {
                var dataSourceDeficientConditionGoal = dataSourceDeficientConditionGoals
                    .SingleOrDefault(_ => _.Name == deficientConditionGoal.Name);
                Assert.NotNull(dataSourceDeficientConditionGoal);
                Assert.Equal(deficientConditionGoal.Attribute.Name, dataSourceDeficientConditionGoal.Attribute.Name);
                Assert.Equal(deficientConditionGoal.AllowedDeficientPercentage, dataSourceDeficientConditionGoal.AllowedDeficientPercentage);
                Assert.Equal(deficientConditionGoal.DeficientLimit, dataSourceDeficientConditionGoal.DeficientLimit);
                Assert.Equal(deficientConditionGoal.Criterion.Expression, dataSourceDeficientConditionGoal.Criterion.Expression);
            });

            var remainingLifeLimits = analysisMethod.RemainingLifeLimits.ToList();
            var dataSourceRemainingLifeLimits = dataSourceAnalysisMethod.RemainingLifeLimits.ToList();
            Assert.Equal(remainingLifeLimits.Count(), dataSourceRemainingLifeLimits.Count());
            remainingLifeLimits.ForEach((remainingLifeLimit, index) =>
            {
                var dataSourceRemainingLifeLimit = dataSourceRemainingLifeLimits[index];
                Assert.Equal(remainingLifeLimit.Attribute.Name, dataSourceRemainingLifeLimit.Attribute.Name);
                Assert.Equal(remainingLifeLimit.Attribute.IsDecreasingWithDeterioration, dataSourceRemainingLifeLimit.Attribute.IsDecreasingWithDeterioration);
                Assert.Equal(remainingLifeLimit.Value, dataSourceRemainingLifeLimit.Value);
                Assert.Equal(remainingLifeLimit.Criterion.Expression, dataSourceRemainingLifeLimit.Criterion.Expression);
            });

            // CleanUp
            testHelper.CleanUp();
        }
    }
}
