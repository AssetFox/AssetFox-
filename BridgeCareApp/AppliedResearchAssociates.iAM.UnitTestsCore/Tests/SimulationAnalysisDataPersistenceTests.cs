using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public void TestCreateExplorer()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupAll();

            // Act
            testHelper.CreateAttributeCriteriaAndEquationJoins();

            // Assert
            var explorer = testHelper.StandAloneSimulation.Network.Explorer;
            var dataSourceExplorer = testHelper.AttributeRepo.GetExplorer();
            AssertExplorerProperties(explorer, dataSourceExplorer);

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertExplorerProperties(Explorer explorer, Explorer dataSourceExplorer)
        {
            Assert.Equal(explorer.AllAttributes.Count(), dataSourceExplorer.AllAttributes.Count());
            Assert.Equal(explorer.CalculatedFields.Count(), dataSourceExplorer.CalculatedFields.Count());
            Assert.Equal(explorer.NumberAttributes.Count(), dataSourceExplorer.NumberAttributes.Count());
            Assert.Equal(explorer.TextAttributes.Count(), dataSourceExplorer.TextAttributes.Count());
            Assert.Equal(explorer.NumericAttributes.Count(), dataSourceExplorer.NumericAttributes.Count());

            explorer.CalculatedFields.ToList().ForEach(calculatedField =>
            {
                var dataSourceCalculatedField = dataSourceExplorer.CalculatedFields
                    .SingleOrDefault(_ => _.Name == calculatedField.Name);
                Assert.NotNull(dataSourceCalculatedField);
                Assert.Equal(calculatedField.IsDecreasingWithDeterioration, dataSourceCalculatedField.IsDecreasingWithDeterioration);
                Assert.Equal(calculatedField.ValueSources.Count(), dataSourceCalculatedField.ValueSources.Count());
                calculatedField.ValueSources.ForEach(valueSource =>
                {
                    var dataSourceValueSource = dataSourceCalculatedField.ValueSources
                        .SingleOrDefault(_ => _.Equation.Expression == valueSource.Equation.Expression &&
                                              _.Criterion.Expression == valueSource.Criterion.Expression);
                    Assert.NotNull(dataSourceValueSource);
                });
            });

            explorer.NumberAttributes.ForEach(numberAttribute =>
            {
                var dataSourceNumberAttribute = dataSourceExplorer.NumberAttributes
                    .SingleOrDefault(_ => _.Name == numberAttribute.Name);
                Assert.NotNull(dataSourceNumberAttribute);
                Assert.Equal(numberAttribute.IsDecreasingWithDeterioration, dataSourceNumberAttribute.IsDecreasingWithDeterioration);
                Assert.Equal(numberAttribute.Minimum, dataSourceNumberAttribute.Minimum);
                Assert.Equal(numberAttribute.Maximum, dataSourceNumberAttribute.Maximum);
                Assert.Equal(numberAttribute.DefaultValue, dataSourceNumberAttribute.DefaultValue);
            });

            explorer.TextAttributes.ForEach(textAttribute =>
            {
                var dataSourceTextAttribute = dataSourceExplorer.TextAttributes
                    .SingleOrDefault(_ => _.Name == textAttribute.Name);
                Assert.NotNull(dataSourceTextAttribute);
                Assert.Equal(textAttribute.DefaultValue, dataSourceTextAttribute.DefaultValue);
            });

            explorer.NumericAttributes.ForEach(numericAttribute =>
            {
                var dataSourceNumericAttribute = dataSourceExplorer.NumericAttributes
                    .SingleOrDefault(_ => _.Name == numericAttribute.Name);
                Assert.NotNull(dataSourceNumericAttribute);
                Assert.Equal(numericAttribute.IsDecreasingWithDeterioration, dataSourceNumericAttribute.IsDecreasingWithDeterioration);
            });
        }

        [Fact]
        public void TestCreateNetwork()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForNetwork();

            // Act
            testHelper.CreateNetwork();

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo.GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            AssertNetworkProperties(testHelper.StandAloneSimulation.Network, dataSourceNetwork);

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertNetworkProperties(Network network, Network dataSourceNetwork)
        {
            Assert.NotNull(dataSourceNetwork);

            var facilities = network.Facilities.ToList();
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
                    Assert.Equal(section.HistoricalAttributes.Count(), dataSourceSection.HistoricalAttributes.Count());

                    var numberAttributes = network.Explorer.NumberAttributes.ToList();
                    numberAttributes.ForEach(numberAttribute =>
                    {
                        if (section.HistoricalAttributes.Any(_ => _.Name == numberAttribute.Name))
                        {
                            var dataSourceNumberAttribute = dataSourceNetwork.Explorer.NumberAttributes
                                .SingleOrDefault(_ => _.Name == numberAttribute.Name);
                            Assert.NotNull(dataSourceNumberAttribute);
                            var history = section.GetHistory(numberAttribute);
                            var dataSourceHistory = dataSourceSection.GetHistory(dataSourceNumberAttribute);
                            Assert.Equal(history.Values.Count(), dataSourceHistory.Values.Count());
                            Assert.Equal(history.MostRecentValue, dataSourceHistory.MostRecentValue);
                            history.Keys.ForEach(historyKey =>
                            {
                                Assert.Equal(history[historyKey], dataSourceHistory[historyKey]);
                            });
                        }
                    });

                    var textAttributes = network.Explorer.TextAttributes.ToList();
                    textAttributes.ForEach(textAttribute =>
                    {
                        if (section.HistoricalAttributes.Any(_ => _.Name == textAttribute.Name))
                        {
                            var dataSourceTextAttribute = dataSourceNetwork.Explorer.TextAttributes
                                .SingleOrDefault(_ => _.Name == textAttribute.Name);
                            Assert.NotNull(dataSourceTextAttribute);
                            var history = section.GetHistory(textAttribute);
                            var dataSourceHistory = dataSourceSection.GetHistory(dataSourceTextAttribute);
                            Assert.Equal(history.Values.Count(), dataSourceHistory.Values.Count());
                            Assert.Equal(history.MostRecentValue, dataSourceHistory.MostRecentValue);
                            history.Keys.ForEach(historyKey =>
                            {
                                Assert.Equal(history[historyKey], dataSourceHistory[historyKey]);
                            });
                        }
                    });
                });
            });
        }

        [Fact]
        public void TestCreateSimulationEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForSimulation();

            // Act
            testHelper.SimulationRepo.CreateSimulation(testHelper.StandAloneSimulation);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulations = dataSourceNetwork.Simulations.ToList();
            AssertSimulationProperties(testHelper.StandAloneSimulation, dataSourceSimulations);
            

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertSimulationProperties(Simulation simulation, List<Simulation> dataSourceSimulations)
        {
            Assert.Single(dataSourceSimulations);
            Assert.Equal(simulation.Name, dataSourceSimulations[0].Name);
            Assert.Equal(simulation.NumberOfYearsOfTreatmentOutlook, dataSourceSimulations[0].NumberOfYearsOfTreatmentOutlook);
        }

        [Fact]
        public void TestCreateAnalysisMethodEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForAnalysisMethod();

            // Act
            testHelper.AnalysisMethodRepo.CreateAnalysisMethod(testHelper.StandAloneSimulation.AnalysisMethod, testHelper.StandAloneSimulation.Name);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
            testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(dataSourceSimulation);
            var dataSourceAnalysisMethod = dataSourceSimulation.AnalysisMethod;
            AssertAnalysisMethodProperties(testHelper.StandAloneSimulation.AnalysisMethod, dataSourceAnalysisMethod);

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertAnalysisMethodProperties(AnalysisMethod analysisMethod, AnalysisMethod dataSourceAnalysisMethod)
        {
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
        }

        [Fact]
        public void TestCreatePerformanceCurveEntities()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForPerformanceCurves();

            // Act
            testHelper.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{testHelper.StandAloneSimulation.Name} Performance Curve Library", testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurves(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), testHelper.StandAloneSimulation.Name);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.PerformanceCurveRepo.GetSimulationPerformanceCurves(dataSourceSimulation);
            var dataSourcePerformanceCurves = dataSourceSimulation.PerformanceCurves.ToList();
            AssertPerformanCurveProperties(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), dataSourcePerformanceCurves);

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertPerformanCurveProperties(List<PerformanceCurve> performanceCurves, List<PerformanceCurve> dataSourcePerformanceCurves)
        {
            Assert.NotNull(dataSourcePerformanceCurves);
            Assert.Equal(performanceCurves.Count(), dataSourcePerformanceCurves.Count());

            performanceCurves.ForEach(curve =>
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
        }

        [Fact]
        public void TestCreateSimulationEntityOutput()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForSimulationOutput();

            var runner = new SimulationRunner(testHelper.StandAloneSimulation);
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

            // Act
            testHelper.SimulationOutputRepo.CreateSimulationOutput(testHelper.StandAloneSimulation.Name, testHelper.StandAloneSimulation.Results);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.SimulationOutputRepo.GetSimulationOutput(dataSourceSimulation);
            AssertSimulationOutput(testHelper.StandAloneSimulation.Results, dataSourceSimulation.Results);


            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertSimulationOutput(SimulationOutput simulationOutput, SimulationOutput dataSourceSimulationOutput)
        {
            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString = JsonConvert.SerializeObject(simulationOutput, settings);
            var dataSourceSimulationOutputString = JsonConvert.SerializeObject(dataSourceSimulationOutput, settings);
            Assert.Equal(simulationOutputString, dataSourceSimulationOutputString);
        }

        [Fact]
        public void TestCreateInvestmentPlanEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForInvestmentPlan();

            // Act
            testHelper.InvestmentPlanRepo.CreateInvestmentPlan(testHelper.StandAloneSimulation.InvestmentPlan, testHelper.StandAloneSimulation.Name);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
            var dataSourceInvestmentPlan = dataSourceSimulation.InvestmentPlan;
            AssertInvestmentPlanProperties(testHelper.StandAloneSimulation.InvestmentPlan, dataSourceInvestmentPlan);

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertInvestmentPlanProperties(InvestmentPlan investmentPlan, InvestmentPlan dataSourceInvestmentPlan)
        {
            Assert.NotNull(dataSourceInvestmentPlan);
            Assert.IsType<InvestmentPlan>(dataSourceInvestmentPlan);

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

                Assert.Equal(budget.YearlyAmounts.Count(), dataSourceBudget.YearlyAmounts.Count());
                budget.YearlyAmounts.ForEach((yearlyAmount, index) =>
                {
                    Assert.Equal(yearlyAmount.Value, dataSourceBudget.YearlyAmounts[index].Value);
                });

                Assert.Equal(budgetConditions.Count(), dataSourceBudgetConditions.Count());
                var budgetCondition = budgetConditions.Single(_ => _.Budget.Name == budget.Name);
                var dataSourceBudgetCondition = dataSourceBudgetConditions.SingleOrDefault(_ => _.Budget.Name == dataSourceBudget.Name);
                Assert.NotNull(dataSourceBudgetCondition);
                Assert.Equal(budgetCondition.Criterion.Expression, dataSourceBudgetCondition.Criterion.Expression);
            });

            Assert.Equal(cashFlowRules.Count(), dataSourceCashFlowRules.Count());
            cashFlowRules.ForEach(cashFlowRule =>
            {
                var dataSourceCashFlowRule = dataSourceCashFlowRules.Find(__ => __.Name == cashFlowRule.Name);
                Assert.NotNull(dataSourceCashFlowRule);
                Assert.Equal(cashFlowRule.Criterion.Expression, dataSourceCashFlowRule.Criterion.Expression);

                var dataSourceDistributionRules = dataSourceCashFlowRule.DistributionRules.ToList();

                Assert.Equal(cashFlowRule.DistributionRules.Count(), dataSourceDistributionRules.Count());
                cashFlowRule.DistributionRules.ForEach((distributionRule, index) =>
                {
                    Assert.Equal(distributionRule.CostCeiling, dataSourceDistributionRules[index].CostCeiling);
                    Assert.Equal(distributionRule.Expression, dataSourceDistributionRules[index].Expression);
                });
            });
        }

        [Fact]
        public void TestCreateCommittedProjectEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForCommittedProjects();

            // Act
            testHelper.CommittedProjectRepo.CreateCommittedProjects(testHelper.StandAloneSimulation.CommittedProjects.ToList(), testHelper.StandAloneSimulation.Name);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
            testHelper.CommittedProjectRepo.GetSimulationCommittedProjects(dataSourceSimulation);
            AssertCommittedProjectProperties(testHelper.StandAloneSimulation.CommittedProjects.ToList(), dataSourceSimulation.CommittedProjects.ToList());

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertCommittedProjectProperties(List<CommittedProject> committedProjects, List<CommittedProject> dataSourceCommittedProjects)
        {
            Assert.Equal(committedProjects.Count(), dataSourceCommittedProjects.Count());

            committedProjects.ForEach(committedProject =>
            {
                var dataSourceCommittedProject = dataSourceCommittedProjects
                    .SingleOrDefault(_ => _.Name == committedProject.Name);
                Assert.NotNull(dataSourceCommittedProject);
                Assert.Equal(committedProject.ShadowForAnyTreatment, dataSourceCommittedProject.ShadowForAnyTreatment);
                Assert.Equal(committedProject.ShadowForSameTreatment, dataSourceCommittedProject.ShadowForSameTreatment);
                Assert.Equal(committedProject.Cost, dataSourceCommittedProject.Cost);

                Assert.Equal(committedProject.Budget.Name, dataSourceCommittedProject.Budget.Name);
                Assert.Equal(committedProject.Budget.YearlyAmounts.Count(), dataSourceCommittedProject.Budget.YearlyAmounts.Count());
                committedProject.Budget.YearlyAmounts.ForEach((yearlyAmount, index) =>
                {
                    var dataSourceYearlyAmount = dataSourceCommittedProject.Budget.YearlyAmounts[index];
                    Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmount.Value);
                });

                Assert.Equal(committedProject.Consequences.Count(), dataSourceCommittedProject.Consequences.Count());
                var dataSourceConsequences = dataSourceCommittedProject.Consequences.ToList();
                committedProject.Consequences.ForEach((consequence, index) =>
                {
                    var dataSourceConsequence = dataSourceConsequences[index];
                    Assert.Equal(consequence.Change.Expression, dataSourceConsequence.Change.Expression);
                    Assert.Equal(consequence.Attribute.Name, dataSourceConsequence.Attribute.Name);
                });
            });
        }

        [Fact]
        public void TestCreateSelectableTreatmentEntities()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForSelectableTreatments();

            // Act
            testHelper.SelectableTreatmentRepo
                .CreateTreatmentLibrary($"{testHelper.StandAloneSimulation.Name} Simulation Treatment Library", testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateSelectableTreatments(testHelper.StandAloneSimulation.Treatments.ToList(), testHelper.StandAloneSimulation.Name);

            // Assert
            var explorer = testHelper.AttributeRepo.GetExplorer();
            var dataSourceNetwork = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
            var dataSourceSimulation = dataSourceNetwork.Simulations.First();
            testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
            testHelper.SelectableTreatmentRepo.GetSimulationTreatments(dataSourceSimulation);
            AssertSelectableTreatmentProperties(testHelper.StandAloneSimulation.Treatments.ToList(), dataSourceSimulation.Treatments.ToList());

            // CleanUp
            testHelper.CleanUp();
        }

        private void AssertSelectableTreatmentProperties(List<SelectableTreatment> treatments, List<SelectableTreatment> dataSourceTreatments,
            bool withSchedulings = false, bool withSupersessions = false)
        {
            Assert.Equal(treatments.Count(), dataSourceTreatments.Count());

            treatments.ForEach(treatment =>
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

                if (withSchedulings)
                {
                    var treatmentSchedulings = treatment.Schedulings.ToList();
                    var dataSourceTreatmentSchedulings = dataSourceTreatment.Schedulings.ToList();
                    Assert.Equal(treatmentSchedulings.Count(), dataSourceTreatmentSchedulings.Count());
                    treatmentSchedulings.ForEach((treatmentScheduling, index) =>
                    {
                        Assert.Equal(treatmentScheduling.OffsetToFutureYear, dataSourceTreatmentSchedulings[index].OffsetToFutureYear);
                    });
                }

                if (withSupersessions)
                {
                    var treatmentSupersessions = treatment.Supersessions.ToList();
                    var dataSourceTreatmentSupersessions = dataSourceTreatment.Supersessions.ToList();
                    Assert.Equal(treatmentSupersessions.Count(), dataSourceTreatmentSupersessions.Count());
                    dataSourceTreatmentSupersessions.ForEach((supersession, index) =>
                    {
                        Assert.Equal(supersession.Criterion.Expression, dataSourceTreatmentSupersessions[index].Criterion.Expression);
                    });
                }
            });
        }

        [Fact]
        public void TestFullSimulationAnalysisIntegrationWithoutCommittedProjectsSchedulingsOrSupersessions()
        {
            // Arrange
            ITestOutputHelper outputHelper = new TestOutputHelper();
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForFullSimulationAnalysisIntegration();

            // Act
            testHelper.CreateAttributeCriteriaAndEquationJoins();
            testHelper.CreateNetwork();
            testHelper.SimulationRepo.CreateSimulation(testHelper.StandAloneSimulation);
            testHelper.InvestmentPlanRepo.CreateInvestmentPlan(testHelper.StandAloneSimulation.InvestmentPlan, testHelper.StandAloneSimulation.Name);
            /*testHelper.AddCommittedProjects();
            testHelper.AddTreatmentSchedulings();
            testHelper.AddTreatmentSupersessions();*/
            testHelper.AnalysisMethodRepo.CreateAnalysisMethod(testHelper.StandAloneSimulation.AnalysisMethod, testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{testHelper.StandAloneSimulation.Name} Performance Curve Library", testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurves(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), testHelper.StandAloneSimulation.Name);
            //testHelper.CommittedProjectRepo.CreateCommittedProjects(testHelper.StandAloneSimulation.CommittedProjects.ToList(), testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateTreatmentLibrary($"{testHelper.StandAloneSimulation.Name} Simulation Treatment Library", testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateSelectableTreatments(testHelper.StandAloneSimulation.Treatments.ToList(), testHelper.StandAloneSimulation.Name);

            var explorer = testHelper.AttributeRepo.GetExplorer();
            var network = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(network);
            network.Simulations.ForEach(simulation =>
            {
                testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
                testHelper.PerformanceCurveRepo.GetSimulationPerformanceCurves(simulation);
                //testHelper.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);
                testHelper.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
            });

            // Assert
            AssertExplorerProperties(testHelper.StandAloneSimulation.Network.Explorer, explorer);
            AssertNetworkProperties(testHelper.StandAloneSimulation.Network, network);
            AssertSimulationProperties(testHelper.StandAloneSimulation, network.Simulations.ToList());
            var dataSourceSimulation = network.Simulations.ToList().First();
            AssertInvestmentPlanProperties(testHelper.StandAloneSimulation.InvestmentPlan, dataSourceSimulation.InvestmentPlan);
            AssertAnalysisMethodProperties(testHelper.StandAloneSimulation.AnalysisMethod, dataSourceSimulation.AnalysisMethod);
            AssertPerformanCurveProperties(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), dataSourceSimulation.PerformanceCurves.ToList());
            AssertSelectableTreatmentProperties(testHelper.StandAloneSimulation.Treatments.ToList(), dataSourceSimulation.Treatments.ToList());

            var simulation = testHelper.StandAloneSimulation;
            simulation.ClearResults();
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
                outputHelper.WriteLine("Simulation is running...");
            }

            dataSourceSimulation.ClearResults();
            var dataSourceRunner = new SimulationRunner(dataSourceSimulation);
            var dataSourceSimulationIsRunning = true;
            dataSourceRunner.Information += (sender, eventArgs) => {
                if (eventArgs.Message == "Simulation complete.")
                {
                    dataSourceSimulationIsRunning = false;
                }
            };
            dataSourceRunner.Run();

            while (dataSourceSimulationIsRunning)
            {
                outputHelper.WriteLine("Data source simulation is running...");
            }

            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString = JsonConvert.SerializeObject(simulation.Results, settings);
            var dataSourceSimulationOutput = JsonConvert.SerializeObject(dataSourceSimulation.Results, settings);
            Assert.Equal(simulationOutputString, dataSourceSimulationOutput);

            // Cleanup
            testHelper.CleanUp();
        }

        [Fact]
        public void TestFullSimulationAnalysisIntegrationWithoutCommittedProjectsWithSchedulingsAndSupersessions()
        {
            // Arrange
            ITestOutputHelper outputHelper = new TestOutputHelper();
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForFullSimulationAnalysisIntegration();

            // Act
            testHelper.CreateAttributeCriteriaAndEquationJoins();
            testHelper.CreateNetwork();
            testHelper.SimulationRepo.CreateSimulation(testHelper.StandAloneSimulation);
            testHelper.InvestmentPlanRepo.CreateInvestmentPlan(testHelper.StandAloneSimulation.InvestmentPlan, testHelper.StandAloneSimulation.Name);
            testHelper.AddTreatmentSchedulings();
            testHelper.AddTreatmentSupersessions();
            testHelper.AnalysisMethodRepo.CreateAnalysisMethod(testHelper.StandAloneSimulation.AnalysisMethod, testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{testHelper.StandAloneSimulation.Name} Performance Curve Library", testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurves(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateTreatmentLibrary($"{testHelper.StandAloneSimulation.Name} Simulation Treatment Library", testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateSelectableTreatments(testHelper.StandAloneSimulation.Treatments.ToList(), testHelper.StandAloneSimulation.Name);

            var explorer = testHelper.AttributeRepo.GetExplorer();
            var network = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(network);
            network.Simulations.ForEach(simulation =>
            {
                testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
                testHelper.PerformanceCurveRepo.GetSimulationPerformanceCurves(simulation);
                testHelper.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
            });

            // Assert
            AssertExplorerProperties(testHelper.StandAloneSimulation.Network.Explorer, explorer);
            AssertNetworkProperties(testHelper.StandAloneSimulation.Network, network);
            AssertSimulationProperties(testHelper.StandAloneSimulation, network.Simulations.ToList());
            var dataSourceSimulation = network.Simulations.ToList().First();
            AssertInvestmentPlanProperties(testHelper.StandAloneSimulation.InvestmentPlan, dataSourceSimulation.InvestmentPlan);
            AssertAnalysisMethodProperties(testHelper.StandAloneSimulation.AnalysisMethod, dataSourceSimulation.AnalysisMethod);
            AssertPerformanCurveProperties(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), dataSourceSimulation.PerformanceCurves.ToList());
            AssertSelectableTreatmentProperties(testHelper.StandAloneSimulation.Treatments.ToList(), dataSourceSimulation.Treatments.ToList());

            var simulation = testHelper.StandAloneSimulation;
            simulation.ClearResults();
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
                outputHelper.WriteLine("Simulation is running...");
            }

            dataSourceSimulation.ClearResults();
            var dataSourceRunner = new SimulationRunner(dataSourceSimulation);
            var dataSourceSimulationIsRunning = true;
            dataSourceRunner.Information += (sender, eventArgs) => {
                if (eventArgs.Message == "Simulation complete.")
                {
                    dataSourceSimulationIsRunning = false;
                }
            };
            dataSourceRunner.Run();

            while (dataSourceSimulationIsRunning)
            {
                outputHelper.WriteLine("Data source simulation is running...");
            }

            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString = JsonConvert.SerializeObject(simulation.Results, settings);
            var dataSourceSimulationOutput = JsonConvert.SerializeObject(dataSourceSimulation.Results, settings);
            Assert.Equal(simulationOutputString, dataSourceSimulationOutput);

            // Cleanup
            testHelper.CleanUp();
        }

        [Fact]
        public void TestFullSimulationAnalysisIntegrationWithCommittedProjectsSchedulingsAndSupersessions()
        {
            // Arrange
            ITestOutputHelper outputHelper = new TestOutputHelper();
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            testHelper.SetupForFullSimulationAnalysisIntegration();

            // Act
            testHelper.CreateAttributeCriteriaAndEquationJoins();
            testHelper.CreateNetwork();
            testHelper.SimulationRepo.CreateSimulation(testHelper.StandAloneSimulation);
            testHelper.InvestmentPlanRepo.CreateInvestmentPlan(testHelper.StandAloneSimulation.InvestmentPlan, testHelper.StandAloneSimulation.Name);
            testHelper.AddCommittedProjects();
            testHelper.AddTreatmentSchedulings();
            testHelper.AddTreatmentSupersessions();
            testHelper.AnalysisMethodRepo.CreateAnalysisMethod(testHelper.StandAloneSimulation.AnalysisMethod, testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{testHelper.StandAloneSimulation.Name} Performance Curve Library", testHelper.StandAloneSimulation.Name);
            testHelper.PerformanceCurveRepo.CreatePerformanceCurves(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), testHelper.StandAloneSimulation.Name);
            testHelper.CommittedProjectRepo.CreateCommittedProjects(testHelper.StandAloneSimulation.CommittedProjects.ToList(), testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateTreatmentLibrary($"{testHelper.StandAloneSimulation.Name} Simulation Treatment Library", testHelper.StandAloneSimulation.Name);
            testHelper.SelectableTreatmentRepo.CreateSelectableTreatments(testHelper.StandAloneSimulation.Treatments.ToList(), testHelper.StandAloneSimulation.Name);

            var explorer = testHelper.AttributeRepo.GetExplorer();
            var network = testHelper.NetworkRepo
                .GetSimulationAnalysisNetwork(testHelper.StandAloneSimulation.Network.Name, explorer);
            testHelper.SimulationRepo.GetAllInNetwork(network);
            network.Simulations.ForEach(simulation =>
            {
                testHelper.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
                testHelper.PerformanceCurveRepo.GetSimulationPerformanceCurves(simulation);
                testHelper.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);
                testHelper.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
            });

            // Assert
            AssertExplorerProperties(testHelper.StandAloneSimulation.Network.Explorer, explorer);
            AssertNetworkProperties(testHelper.StandAloneSimulation.Network, network);
            AssertSimulationProperties(testHelper.StandAloneSimulation, network.Simulations.ToList());
            var dataSourceSimulation = network.Simulations.ToList().First();
            AssertInvestmentPlanProperties(testHelper.StandAloneSimulation.InvestmentPlan, dataSourceSimulation.InvestmentPlan);
            AssertAnalysisMethodProperties(testHelper.StandAloneSimulation.AnalysisMethod, dataSourceSimulation.AnalysisMethod);
            AssertPerformanCurveProperties(testHelper.StandAloneSimulation.PerformanceCurves.ToList(), dataSourceSimulation.PerformanceCurves.ToList());
            AssertSelectableTreatmentProperties(testHelper.StandAloneSimulation.Treatments.ToList(), dataSourceSimulation.Treatments.ToList());

            var simulation = testHelper.StandAloneSimulation;
            simulation.ClearResults();
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
                outputHelper.WriteLine("Simulation is running...");
            }

            dataSourceSimulation.ClearResults();
            var dataSourceRunner = new SimulationRunner(dataSourceSimulation);
            var dataSourceSimulationIsRunning = true;
            dataSourceRunner.Information += (sender, eventArgs) => {
                if (eventArgs.Message == "Simulation complete.")
                {
                    dataSourceSimulationIsRunning = false;
                }
            };
            dataSourceRunner.Run();

            while (dataSourceSimulationIsRunning)
            {
                outputHelper.WriteLine("Data source simulation is running...");
            }

            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString = JsonConvert.SerializeObject(simulation.Results, settings);
            var dataSourceSimulationOutput = JsonConvert.SerializeObject(dataSourceSimulation.Results, settings);
            Assert.Equal(simulationOutputString, dataSourceSimulationOutput);

            // Cleanup
            testHelper.CleanUp();
        }
    }
}
