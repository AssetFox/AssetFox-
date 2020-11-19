using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
using Xunit;


namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationAnalysisDataPersistenceTests
    {
        [Fact]
        public void TestCreateLiteSimulationEntity()
        {
            // Arrange
            var testHelper = new SimulationAnalysisDataPersistenceTestHelper();
            var simulation = testHelper.GetStandAloneSimulation();
            testHelper.CreateNetwork(simulation.Network.Name);
            testHelper.TestCreateSimulationEntitySetup();

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
            var dataSourceAnalysisMethod = testHelper.AnalysisMethodRepo.GetSimulationAnalysisMethod(simulation.Name);
            Assert.NotNull(dataSourceAnalysisMethod);
            Assert.IsType<AnalysisMethod>(dataSourceAnalysisMethod);
            Assert.Equal(simulation.AnalysisMethod.Weighting.Name, dataSourceAnalysisMethod.Weighting.Name);
            Assert.Equal(simulation.AnalysisMethod.OptimizationStrategy, dataSourceAnalysisMethod.OptimizationStrategy);
            Assert.Equal(simulation.AnalysisMethod.SpendingStrategy, dataSourceAnalysisMethod.SpendingStrategy);
            Assert.Equal(simulation.AnalysisMethod.Description, dataSourceAnalysisMethod.Description);
            Assert.Equal(simulation.AnalysisMethod.ShouldDeteriorateDuringCashFlow, dataSourceAnalysisMethod.ShouldDeteriorateDuringCashFlow);
            Assert.Equal(simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets, dataSourceAnalysisMethod.ShouldUseExtraFundsAcrossBudgets);
            Assert.Equal(simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts, dataSourceAnalysisMethod.ShouldApplyMultipleFeasibleCosts);

            // CleanUp
            testHelper.CleanUp();
        }
    }
}
