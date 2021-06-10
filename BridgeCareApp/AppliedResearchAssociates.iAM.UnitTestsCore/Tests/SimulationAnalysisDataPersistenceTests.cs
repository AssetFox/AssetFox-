using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationAnalysisDataPersistenceTests
    {
        private readonly SimulationAnalysisDataPersistenceTestHelper _testHelper;
        private const int SimulationId = 1171;
        private const int SimulationIdWithCommitted = 1260;

        public SimulationAnalysisDataPersistenceTests() => _testHelper = new SimulationAnalysisDataPersistenceTestHelper();

        [Fact]
        public void TestCreateExplorer()
        {
            try
            {
                // Arrange
                _testHelper.CreateAttributes();
                _testHelper.SetStandAloneSimulation(SimulationId);
                

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.CreateAttributeCriteriaAndEquationJoins();
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.StandAloneSimulation.Network.Explorer;
                var dataSourceExplorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                AssertExplorerProperties(explorer, dataSourceExplorer);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
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
            try
            {
                // Arrange
                _testHelper.CreateAttributes();
                _testHelper.SetStandAloneSimulation(SimulationId);
                
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.CreateNetwork();
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                AssertNetworkProperties(_testHelper.StandAloneSimulation.Network, dataSourceNetwork);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertNetworkProperties(Network network, Network dataSourceNetwork)
        {
            Assert.NotNull(dataSourceNetwork);
            Assert.Equal(network.Id, dataSourceNetwork.Id);

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
                    var dataSourceSection = dataSourceSections.SingleOrDefault(_ => $"{_.Facility.Name}{_.Name}" == $"{section.Facility.Name}{section.Name}");
                    Assert.NotNull(dataSourceSection);
                    Assert.Equal(section.SpatialWeighting.Expression, dataSourceSection.SpatialWeighting.Expression);
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
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupForSimulation();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.SimulationRepo.CreateSimulation(_testHelper.StandAloneSimulation);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulations = dataSourceNetwork.Simulations.ToList();
                AssertSimulationProperties(_testHelper.StandAloneSimulation, dataSourceSimulations);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertSimulationProperties(Simulation simulation, List<Simulation> dataSourceSimulations)
        {
            Assert.Single(dataSourceSimulations);
            Assert.Equal(simulation.Id, dataSourceSimulations[0].Id);
            Assert.Equal(simulation.Name, dataSourceSimulations[0].Name);
            Assert.Equal(simulation.NumberOfYearsOfTreatmentOutlook, dataSourceSimulations[0].NumberOfYearsOfTreatmentOutlook);
        }

        [Fact]
        public void TestCreateAnalysisMethodEntity()
        {
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupWithInvestmentPlan();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.AnalysisMethodRepo.CreateAnalysisMethod(_testHelper.StandAloneSimulation.AnalysisMethod, _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
                _testHelper.UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(dataSourceSimulation);
                var dataSourceAnalysisMethod = dataSourceSimulation.AnalysisMethod;
                AssertAnalysisMethodProperties(_testHelper.StandAloneSimulation.AnalysisMethod, dataSourceAnalysisMethod);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertAnalysisMethodProperties(AnalysisMethod analysisMethod, AnalysisMethod dataSourceAnalysisMethod)
        {
            Assert.NotNull(dataSourceAnalysisMethod);
            Assert.Equal(analysisMethod.Id, dataSourceAnalysisMethod.Id);
            Assert.Equal(analysisMethod.Weighting.Name, dataSourceAnalysisMethod.Weighting.Name);
            Assert.Equal(analysisMethod.Weighting.IsDecreasingWithDeterioration, dataSourceAnalysisMethod.Weighting.IsDecreasingWithDeterioration);
            Assert.Equal(analysisMethod.OptimizationStrategy, dataSourceAnalysisMethod.OptimizationStrategy);
            Assert.Equal(analysisMethod.SpendingStrategy, dataSourceAnalysisMethod.SpendingStrategy);
            Assert.Equal(analysisMethod.Description, dataSourceAnalysisMethod.Description);
            Assert.Equal(analysisMethod.ShouldDeteriorateDuringCashFlow, dataSourceAnalysisMethod.ShouldDeteriorateDuringCashFlow);
            Assert.Equal(analysisMethod.ShouldUseExtraFundsAcrossBudgets, dataSourceAnalysisMethod.ShouldUseExtraFundsAcrossBudgets);
            Assert.Equal(analysisMethod.ShouldApplyMultipleFeasibleCosts, dataSourceAnalysisMethod.ShouldApplyMultipleFeasibleCosts);
            Assert.Equal(analysisMethod.Filter.Expression, dataSourceAnalysisMethod.Filter.Expression);

            Assert.Equal(analysisMethod.Benefit.Id, dataSourceAnalysisMethod.Benefit.Id);
            Assert.Equal(analysisMethod.Benefit.Attribute.Name, dataSourceAnalysisMethod.Benefit.Attribute.Name);
            Assert.Equal(analysisMethod.Benefit.Attribute.IsDecreasingWithDeterioration, dataSourceAnalysisMethod.Benefit.Attribute.IsDecreasingWithDeterioration);
            Assert.Equal(analysisMethod.Benefit.Limit, dataSourceAnalysisMethod.Benefit.Limit);

            var budgetPriorities = analysisMethod.BudgetPriorities.ToList();
            var dataSourceBudgetPriorities = dataSourceAnalysisMethod.BudgetPriorities.ToList();
            Assert.Equal(budgetPriorities.Count(), dataSourceBudgetPriorities.Count());
            budgetPriorities.ForEach(priority =>
            {
                var dataSourcePriority = dataSourceBudgetPriorities.SingleOrDefault(_ => _.Id == priority.Id);
                Assert.NotNull(dataSourcePriority);
                Assert.Equal(priority.Criterion.Expression, dataSourcePriority.Criterion.Expression);
                Assert.Equal(priority.PriorityLevel, dataSourcePriority.PriorityLevel);
                Assert.Equal(priority.Year, dataSourcePriority.Year);

                var budgetPercentagePairs = priority.BudgetPercentagePairs.ToList();
                var dataSourceBudgetPercentagePairs = dataSourcePriority.BudgetPercentagePairs.ToList();
                Assert.Equal(budgetPercentagePairs.Count(), dataSourceBudgetPercentagePairs.Count());
                budgetPercentagePairs.ForEach(budgetPercentagePair =>
                {
                    var dataSourceBudgetPercentagePair = dataSourceBudgetPercentagePairs
                        .SingleOrDefault(_ => _.Id == budgetPercentagePair.Id);
                    Assert.NotNull(dataSourceBudgetPercentagePair);
                    Assert.Equal(budgetPercentagePair.Percentage, dataSourceBudgetPercentagePair.Percentage);
                    Assert.Equal(budgetPercentagePair.Budget.Id, dataSourceBudgetPercentagePair.Budget.Id);
                    Assert.Equal(budgetPercentagePair.Budget.Name, dataSourceBudgetPercentagePair.Budget.Name);
                    var yearlyAmounts = budgetPercentagePair.Budget.YearlyAmounts.ToList();

                    var dataSourceYearlyAmounts = dataSourceBudgetPercentagePair.Budget.YearlyAmounts.ToList();
                    Assert.Equal(yearlyAmounts.Count(), dataSourceYearlyAmounts.Count());
                    yearlyAmounts.ForEach(yearlyAmount =>
                    {
                        var dataSourceYearlyAmount = dataSourceYearlyAmounts
                            .SingleOrDefault(_ => _.Id == yearlyAmount.Id);
                        Assert.NotNull(dataSourceYearlyAmount);
                        Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmount.Value);
                    });
                });
            });

            var targetConditionGoals = analysisMethod.TargetConditionGoals.ToList();
            var dataSourceTargetConditionGoals = dataSourceAnalysisMethod.TargetConditionGoals.ToList();
            Assert.Equal(targetConditionGoals.Count(), dataSourceTargetConditionGoals.Count());
            targetConditionGoals.ForEach(targetConditionGoal =>
            {
                var dataSourceTargetConditionGoal = dataSourceTargetConditionGoals
                    .SingleOrDefault(_ => _.Id == targetConditionGoal.Id);
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
                    .SingleOrDefault(_ => _.Id == deficientConditionGoal.Id);
                Assert.NotNull(dataSourceDeficientConditionGoal);
                Assert.Equal(deficientConditionGoal.Attribute.Name, dataSourceDeficientConditionGoal.Attribute.Name);
                Assert.Equal(deficientConditionGoal.AllowedDeficientPercentage, dataSourceDeficientConditionGoal.AllowedDeficientPercentage);
                Assert.Equal(deficientConditionGoal.DeficientLimit, dataSourceDeficientConditionGoal.DeficientLimit);
                Assert.Equal(deficientConditionGoal.Criterion.Expression, dataSourceDeficientConditionGoal.Criterion.Expression);
            });

            var remainingLifeLimits = analysisMethod.RemainingLifeLimits.ToList();
            var dataSourceRemainingLifeLimits = dataSourceAnalysisMethod.RemainingLifeLimits.ToList();
            Assert.Equal(remainingLifeLimits.Count(), dataSourceRemainingLifeLimits.Count());
            remainingLifeLimits.ForEach(remainingLifeLimit =>
            {
                var dataSourceRemainingLifeLimit = dataSourceRemainingLifeLimits
                    .SingleOrDefault(_ => _.Id == remainingLifeLimit.Id);
                Assert.NotNull(dataSourceRemainingLifeLimit);
                Assert.Equal(remainingLifeLimit.Attribute.Name, dataSourceRemainingLifeLimit.Attribute.Name);
                Assert.Equal(remainingLifeLimit.Attribute.IsDecreasingWithDeterioration, dataSourceRemainingLifeLimit.Attribute.IsDecreasingWithDeterioration);
                Assert.Equal(remainingLifeLimit.Value, dataSourceRemainingLifeLimit.Value);
                Assert.Equal(remainingLifeLimit.Criterion.Expression, dataSourceRemainingLifeLimit.Criterion.Expression);
            });
        }

        [Fact]
        public void TestCreatePerformanceCurveEntities()
        {
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupForAll();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{_testHelper.StandAloneSimulation.Name} Performance Curve Library", _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.PerformanceCurveRepo.CreatePerformanceCurves(_testHelper.StandAloneSimulation.PerformanceCurves.ToList(), _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.PerformanceCurveRepo.SimulationPerformanceCurves(dataSourceSimulation);
                var dataSourcePerformanceCurves = dataSourceSimulation.PerformanceCurves.ToList();
                AssertPerformanCurveProperties(_testHelper.StandAloneSimulation.PerformanceCurves.ToList(), dataSourcePerformanceCurves);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertPerformanCurveProperties(List<PerformanceCurve> performanceCurves, List<PerformanceCurve> dataSourcePerformanceCurves)
        {
            Assert.NotNull(dataSourcePerformanceCurves);
            Assert.Equal(performanceCurves.Count(), dataSourcePerformanceCurves.Count());

            performanceCurves.ForEach(curve =>
            {
                var dataSourceCurve = dataSourcePerformanceCurves.SingleOrDefault(_ => _.Id == curve.Id);
                Assert.NotNull(dataSourceCurve);
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
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupForAll();

                var runner = new SimulationRunner(_testHelper.StandAloneSimulation);
                var simulationIsRunning = true;
                runner.Information += (sender, eventArgs) =>
                {
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
                _testHelper.UnitOfWork.SimulationOutputRepo.CreateSimulationOutput(_testHelper.StandAloneSimulation.Id, _testHelper.StandAloneSimulation.Results);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.SimulationOutputRepo.GetSimulationOutput(dataSourceSimulation);
                AssertSimulationOutput(_testHelper.StandAloneSimulation.Results, dataSourceSimulation.Results);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
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
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupForAll();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(_testHelper.StandAloneSimulation.InvestmentPlan, _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
                var dataSourceInvestmentPlan = dataSourceSimulation.InvestmentPlan;
                AssertInvestmentPlanProperties(_testHelper.StandAloneSimulation.InvestmentPlan, dataSourceInvestmentPlan);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertInvestmentPlanProperties(InvestmentPlan investmentPlan, InvestmentPlan dataSourceInvestmentPlan)
        {
            Assert.NotNull(dataSourceInvestmentPlan);
            Assert.Equal(investmentPlan.Id, dataSourceInvestmentPlan.Id);

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
                var dataSourceBudget = dataSourceBudgets.Find(_ => _.Id == budget.Id);
                Assert.NotNull(dataSourceBudget);

                Assert.Equal(budget.YearlyAmounts.Count(), dataSourceBudget.YearlyAmounts.Count());
                budget.YearlyAmounts.ForEach(yearlyAmount =>
                {
                    var dataSourceYearlyAmount = dataSourceBudget.YearlyAmounts
                        .SingleOrDefault(_ => _.Id == yearlyAmount.Id);
                    Assert.NotNull(dataSourceYearlyAmount);
                    Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmount.Value);
                });

                Assert.Equal(budgetConditions.Count(), dataSourceBudgetConditions.Count());
                var budgetCondition = budgetConditions.Single(_ => _.Budget.Id == budget.Id);
                var dataSourceBudgetCondition = dataSourceBudgetConditions.SingleOrDefault(_ => _.Budget.Id == dataSourceBudget.Id);
                Assert.NotNull(dataSourceBudgetCondition);
                Assert.Equal(budgetCondition.Criterion.Expression, dataSourceBudgetCondition.Criterion.Expression);
            });

            Assert.Equal(cashFlowRules.Count(), dataSourceCashFlowRules.Count());
            cashFlowRules.ForEach(cashFlowRule =>
            {
                var dataSourceCashFlowRule = dataSourceCashFlowRules.SingleOrDefault(_ => _.Id == cashFlowRule.Id);
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
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationIdWithCommitted);
                _testHelper.ReduceNumberOfFacilitiesAndSectionsWithCommittedProjects(_testHelper.StandAloneSimulation);
                _testHelper.SetupWithInvestmentPlan();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.CommittedProjectRepo.CreateCommittedProjects(
                    _testHelper.StandAloneSimulation.CommittedProjects.ToList(), _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
                _testHelper.UnitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(dataSourceSimulation);
                AssertCommittedProjectProperties(_testHelper.StandAloneSimulation.CommittedProjects.ToList(), dataSourceSimulation.CommittedProjects.ToList());
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertCommittedProjectProperties(List<CommittedProject> committedProjects, List<CommittedProject> dataSourceCommittedProjects)
        {
            Assert.Equal(committedProjects.Count(), dataSourceCommittedProjects.Count());

            committedProjects.ForEach(committedProject =>
            {
                var dataSourceCommittedProject = dataSourceCommittedProjects
                    .SingleOrDefault(_ => _.Id == committedProject.Id);
                Assert.NotNull(dataSourceCommittedProject);
                Assert.Equal(committedProject.ShadowForAnyTreatment, dataSourceCommittedProject.ShadowForAnyTreatment);
                Assert.Equal(committedProject.ShadowForSameTreatment, dataSourceCommittedProject.ShadowForSameTreatment);
                Assert.Equal(committedProject.Cost, dataSourceCommittedProject.Cost);

                Assert.Equal(committedProject.Budget.Id, dataSourceCommittedProject.Budget.Id);
                Assert.Equal(committedProject.Budget.Name, dataSourceCommittedProject.Budget.Name);
                Assert.Equal(committedProject.Budget.YearlyAmounts.Count(), dataSourceCommittedProject.Budget.YearlyAmounts.Count());
                committedProject.Budget.YearlyAmounts.ForEach(yearlyAmount =>
                {
                    var dataSourceYearlyAmount = dataSourceCommittedProject.Budget.YearlyAmounts
                        .SingleOrDefault(_ => _.Id == yearlyAmount.Id);
                    Assert.NotNull(dataSourceYearlyAmount);
                    Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmount.Value);
                });

                Assert.Equal(committedProject.Consequences.Count(), dataSourceCommittedProject.Consequences.Count());
                var dataSourceConsequences = dataSourceCommittedProject.Consequences.ToList();
                committedProject.Consequences.ForEach(consequence =>
                {
                    var dataSourceConsequence = dataSourceConsequences.SingleOrDefault(_ => _.Id == consequence.Id);
                    Assert.NotNull(dataSourceConsequence);
                    Assert.Equal(consequence.Change.Expression, dataSourceConsequence.Change.Expression);
                    Assert.Equal(consequence.Attribute.Name, dataSourceConsequence.Attribute.Name);
                });
            });
        }

        [Fact]
        public void TestCreateSelectableTreatmentEntities()
        {
            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);
                _testHelper.SetupForSelectableTreatments();

                // Act
                _testHelper.UnitOfWork.BeginTransaction();
                _testHelper.UnitOfWork.SelectableTreatmentRepo
                    .CreateTreatmentLibrary($"{_testHelper.StandAloneSimulation.Name} Simulation Treatment Library", _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.SelectableTreatmentRepo.CreateSelectableTreatments(_testHelper.StandAloneSimulation.Treatments.ToList(), _testHelper.StandAloneSimulation.Id);
                _testHelper.UnitOfWork.Commit();

                // Assert
                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var dataSourceNetwork = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(dataSourceNetwork);
                var dataSourceSimulation = dataSourceNetwork.Simulations.First();
                _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(dataSourceSimulation);
                _testHelper.UnitOfWork.SelectableTreatmentRepo.GetSimulationTreatments(dataSourceSimulation);
                AssertSelectableTreatmentProperties(_testHelper.StandAloneSimulation.Treatments.ToList(), dataSourceSimulation.Treatments.ToList());
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
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

                var budgets = treatment.Budgets.ToList();
                var dataSourceBudgets = dataSourceTreatment.Budgets.ToList();
                Assert.Equal(budgets.Count(), dataSourceBudgets.Count());
                budgets.ForEach(budget =>
                {
                    var dataSourceBudget = dataSourceBudgets.SingleOrDefault(_ => _.Id == budget.Id);
                    Assert.NotNull(dataSourceBudget);
                    Assert.Equal(budget.YearlyAmounts.Count(), dataSourceBudget.YearlyAmounts.Count());
                    budget.YearlyAmounts.ForEach(yearlyAmount =>
                    {
                        var dataSourceYearlyAmount = dataSourceBudget.YearlyAmounts
                            .SingleOrDefault(_ => _.Id == yearlyAmount.Id);
                        Assert.NotNull(dataSourceYearlyAmount);
                        Assert.Equal(yearlyAmount.Value, dataSourceYearlyAmount.Value);
                    });
                });

                var consequences = treatment.Consequences.ToList();
                var dataSourceConsequences = dataSourceTreatment.Consequences.ToList();
                Assert.Equal(consequences.Count(), dataSourceConsequences.Count());
                consequences.ForEach(consequence =>
                {
                    var dataSourceConsequence = dataSourceConsequences.SingleOrDefault(_ => _.Id == consequence.Id);
                    Assert.NotNull(dataSourceConsequence);
                    Assert.Equal(consequence.Attribute.Name, dataSourceConsequence.Attribute.Name);
                    Assert.Equal(consequence.Change.Expression, dataSourceConsequence.Change.Expression);
                    Assert.Equal(consequence.Equation.Expression, dataSourceConsequence.Equation.Expression);
                    Assert.Equal(consequence.Criterion.Expression, dataSourceConsequence.Criterion.Expression);
                });

                var costs = treatment.Costs.ToList();
                var dataSourceCosts = dataSourceTreatment.Costs.ToList();
                Assert.Equal(costs.Count(), dataSourceCosts.Count());
                costs.ForEach(cost =>
                {
                    var dataSourceCost = dataSourceCosts.SingleOrDefault(_ => _.Id == cost.Id);
                    Assert.NotNull(dataSourceCost);
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
                    treatmentSchedulings.ForEach(treatmentScheduling =>
                    {
                        var dataSourceTreatmentScheduling = dataSourceTreatmentSchedulings
                            .SingleOrDefault(_ => _.Id == treatmentScheduling.Id);
                        Assert.NotNull(dataSourceTreatmentScheduling);
                        Assert.Equal(treatmentScheduling.OffsetToFutureYear, dataSourceTreatmentScheduling.OffsetToFutureYear);
                    });
                }

                if (withSupersessions)
                {
                    var treatmentSupersessions = treatment.Supersessions.ToList();
                    var dataSourceTreatmentSupersessions = dataSourceTreatment.Supersessions.ToList();
                    Assert.Equal(treatmentSupersessions.Count(), dataSourceTreatmentSupersessions.Count());
                    dataSourceTreatmentSupersessions.ForEach(supersession =>
                    {
                        var dataSourceSupersession = dataSourceTreatmentSupersessions
                            .SingleOrDefault(_ => _.Id == supersession.Id);
                        Assert.NotNull(dataSourceSupersession);
                        Assert.Equal(supersession.Criterion.Expression, dataSourceSupersession.Criterion.Expression);
                    });
                }
            });
        }

        [Fact]
        public void TestLegacySimulationSynchronizerWithSimulationHavingCommittedProjects()
        {
            var testOutputHelper = new TestOutputHelper();

            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationIdWithCommitted);
                _testHelper.ReduceNumberOfFacilitiesAndSectionsWithCommittedProjects(_testHelper.StandAloneSimulation);

                // Act
                _testHelper.SynchronizeLegacySimulationWithCommittedProjects(SimulationIdWithCommitted);

                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var networks = _testHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
                _testHelper.StandAloneSimulation.Network.Id = networks.First().Id;
                var network = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(network);
                network.Simulations.ForEach(simulation =>
                {
                    _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                    _testHelper.UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
                    _testHelper.UnitOfWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
                    _testHelper.UnitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);
                    _testHelper.UnitOfWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
                });

                var simulation = _testHelper.StandAloneSimulation;
                simulation.ClearResults();
                var runner = new SimulationRunner(simulation);
                var simulationIsRunning = true;
                runner.Information += (sender, eventArgs) =>
                {
                    if (eventArgs.Message == "Simulation complete.")
                    {
                        simulationIsRunning = false;
                    }
                };
                runner.Run();
                while (simulationIsRunning)
                {
                    testOutputHelper.WriteLine("Simulation is running...");
                }

                network.Simulations.First().ClearResults();
                var dataSourceRunner = new SimulationRunner(network.Simulations.First());
                var dataSourceSimulationIsRunning = true;
                dataSourceRunner.Information += (sender, eventArgs) =>
                {
                    if (eventArgs.Message == "Simulation complete.")
                    {
                        dataSourceSimulationIsRunning = false;
                    }
                };
                dataSourceRunner.Run();

                while (dataSourceSimulationIsRunning)
                {
                    testOutputHelper.WriteLine("Data source simulation is running...");
                }

                // Assert
                AssertSimulationOutputsEqual(simulation.Results, network.Simulations.First().Results);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public void TestLegacySimulationSynchronizerWithSimulationHavingNoCommittedProjects()
        {
            var testOutputHelper = new TestOutputHelper();

            try
            {
                // Arrange
                _testHelper.SetStandAloneSimulation(SimulationId);
                _testHelper.ReduceNumberOfFacilitiesAndSections(_testHelper.StandAloneSimulation);

                // Act
                _testHelper.SynchronizeLegacySimulation(SimulationId);

                var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
                var networks = _testHelper.UnitOfWork.NetworkRepo.GetAllNetworks();
                _testHelper.StandAloneSimulation.Network.Id = networks.First().Id;
                var network = _testHelper.UnitOfWork.NetworkRepo
                    .GetSimulationAnalysisNetwork(_testHelper.StandAloneSimulation.Network.Id, explorer);
                _testHelper.UnitOfWork.SimulationRepo.GetAllInNetwork(network);
                network.Simulations.ForEach(simulation =>
                {
                    _testHelper.UnitOfWork.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
                    _testHelper.UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation);
                    _testHelper.UnitOfWork.PerformanceCurveRepo.SimulationPerformanceCurves(simulation);
                    _testHelper.UnitOfWork.CommittedProjectRepo.GetSimulationCommittedProjects(simulation);
                    _testHelper.UnitOfWork.SelectableTreatmentRepo.GetSimulationTreatments(simulation);
                });

                var dataSourceSimulation = network.Simulations.ToList().First();

                var simulation = _testHelper.StandAloneSimulation;
                simulation.ClearResults();
                var runner = new SimulationRunner(simulation);
                var simulationIsRunning = true;
                runner.Information += (sender, eventArgs) =>
                {
                    if (eventArgs.Message == "Simulation complete.")
                    {
                        simulationIsRunning = false;
                    }
                };
                runner.Run();

                while (simulationIsRunning)
                {
                    testOutputHelper.WriteLine("Simulation is running...");
                }

                dataSourceSimulation.ClearResults();
                var dataSourceRunner = new SimulationRunner(dataSourceSimulation);
                var dataSourceSimulationIsRunning = true;
                dataSourceRunner.Information += (sender, eventArgs) =>
                {
                    if (eventArgs.Message == "Simulation complete.")
                    {
                        dataSourceSimulationIsRunning = false;
                    }
                };
                dataSourceRunner.Run();

                while (dataSourceSimulationIsRunning)
                {
                    testOutputHelper.WriteLine("Data source simulation is running...");
                }

                // Assert
                AssertSimulationOutputsEqual(simulation.Results, dataSourceSimulation.Results);
            }
            finally
            {
                // CleanUp
                _testHelper.CleanUp();
            }
        }

        private void AssertSimulationOutputsEqual(SimulationOutput output, SimulationOutput dataSourceOutput)
        {
            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString =
                JsonConvert.SerializeObject(SortAllSimulationOutputProperties(output), settings);
            var dataSourceSimulationOutputString =
                JsonConvert.SerializeObject(SortAllSimulationOutputProperties(dataSourceOutput), settings);
            Assert.Equal(simulationOutputString, dataSourceSimulationOutputString);
        }

        private SimulationOutput SortAllSimulationOutputProperties(SimulationOutput output)
        {
            var simulationOutput = new SimulationOutput { InitialConditionOfNetwork = output.InitialConditionOfNetwork };

            var years = output.Years.OrderBy(_ => _.Year).Select(year =>
            {
                var sections = year.Sections.OrderBy(_ => _.SectionName).Select(section =>
                {
                    var valuePerNumAttr = section.ValuePerNumericAttribute.OrderBy(_ => _.Key)
                        .ToDictionary(_ => _.Key, _ => _.Value);
                    section.ValuePerNumericAttribute.Clear();
                    section.ValuePerNumericAttribute.CopyFrom(valuePerNumAttr);

                    var valuePerTxtAttr = section.ValuePerTextAttribute.OrderBy(_ => _.Key)
                        .ToDictionary(_ => _.Key, _ => _.Value);
                    section.ValuePerTextAttribute.Clear();
                    section.ValuePerTextAttribute.CopyFrom(valuePerTxtAttr);

                    var treatmentConsiderations = section.TreatmentConsiderations.OrderBy(_ => _.TreatmentName)
                        .Select(treatmentConsiderationDetail =>
                        {
                            var budgetUsages = treatmentConsiderationDetail.BudgetUsages.OrderBy(_ => _.BudgetName);
                            treatmentConsiderationDetail.BudgetUsages.Clear();
                            treatmentConsiderationDetail.BudgetUsages.AddRange(budgetUsages);

                            var cashFlowConsiderations =
                                treatmentConsiderationDetail.CashFlowConsiderations.OrderBy(_ => _.CashFlowRuleName);
                            treatmentConsiderationDetail.CashFlowConsiderations.Clear();
                            treatmentConsiderationDetail.CashFlowConsiderations.AddRange(cashFlowConsiderations);
                            return treatmentConsiderationDetail;
                        });
                    section.TreatmentConsiderations.Clear();
                    section.TreatmentConsiderations.AddRange(treatmentConsiderations);

                    var treatmentOptions = section.TreatmentOptions.OrderBy(_ => _.TreatmentName);
                    section.TreatmentOptions.Clear();
                    section.TreatmentOptions.AddRange(treatmentOptions);

                    var treatmentRejections = section.TreatmentRejections.OrderBy(_ => _.TreatmentName);
                    section.TreatmentRejections.Clear();
                    section.TreatmentRejections.AddRange(treatmentRejections);

                    var treatmentSchedulingCollisions =
                        section.TreatmentSchedulingCollisions.OrderBy(_ => _.NameOfUnscheduledTreatment);
                    section.TreatmentSchedulingCollisions.Clear();
                    section.TreatmentSchedulingCollisions.AddRange(treatmentSchedulingCollisions);

                    return section;
                }).ToList();
                year.Sections.Clear();
                year.Sections.AddRange(sections);

                var budgets = year.Budgets.OrderBy(_ => _.BudgetName);
                year.Budgets.Clear();
                year.Budgets.AddRange(budgets);

                var deficientConditionGoals = year.DeficientConditionGoals.OrderBy(_ => _.GoalName);
                year.DeficientConditionGoals.Clear();
                year.DeficientConditionGoals.AddRange(deficientConditionGoals);

                var targetConditionGoals = year.TargetConditionGoals.OrderBy(_ => _.GoalName);
                year.TargetConditionGoals.Clear();
                year.TargetConditionGoals.AddRange(targetConditionGoals);

                return year;
            }).ToList();
            simulationOutput.Years.Clear();
            simulationOutput.Years.AddRange(years);

            var initialSectionSummaries = output.InitialSectionSummaries.OrderBy(_ => _.SectionName).Select(
                sectionSummary =>
                {
                    var valuePerNumAttr = sectionSummary.ValuePerNumericAttribute.OrderBy(_ => _.Key)
                        .ToDictionary(_ => _.Key, _ => _.Value);
                    sectionSummary.ValuePerNumericAttribute.Clear();
                    sectionSummary.ValuePerNumericAttribute.CopyFrom(valuePerNumAttr);

                    var valuePerTxtAttr = sectionSummary.ValuePerTextAttribute.OrderBy(_ => _.Key)
                        .ToDictionary(_ => _.Key, _ => _.Value);
                    sectionSummary.ValuePerTextAttribute.Clear();
                    sectionSummary.ValuePerTextAttribute.CopyFrom(valuePerTxtAttr);

                    return sectionSummary;
                });
            simulationOutput.InitialSectionSummaries.Clear();
            simulationOutput.InitialSectionSummaries.AddRange(initialSectionSummaries);

            return simulationOutput;
        }
    }
}
