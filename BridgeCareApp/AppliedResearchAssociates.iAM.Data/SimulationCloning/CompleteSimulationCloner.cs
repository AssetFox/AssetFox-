using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    public class CompleteSimulationCloner
    {
        public static CompleteSimulationDTO Clone(CompleteSimulationDTO completeSimulation, CloneSimulationDTO cloneRequest, Guid ownerId)
        {
            var cloneAnalysisMethod = AnalysisMethodCloner.Clone(completeSimulation.AnalysisMethod, ownerId);
            var cloneBudgetPriorities = BudgetPriorityCloner.CloneList(completeSimulation.BudgetPriorities, ownerId);
            var cloneCashFlowFule = CashFlowRuleCloner.CloneList(completeSimulation.CashFlowRules, ownerId);
            var cloneInvestmentPlan = InvestmentPlanCloner.Clone(completeSimulation.InvestmentPlan);
            var cloneReportIndex = ReportIndexCloner.CloneList(completeSimulation.ReportIndexes);
            var cloneScenarioPerformanceCurvesImportResult = ScenarioPerformanceCurvesImportResultCloner.CloneListNullPropagating(completeSimulation.PerformanceCurves);
            var cloneCalculatedTribute = CalculatedAttributeCloner.CloneList(completeSimulation.CalculatedAttributes);


            var cloneRemainingLifeLimits = RemainingLifeLimitCloner.CloneList(completeSimulation.RemainingLifeLimits, ownerId);
            var cloneTreatment = TreatmentCloner.CloneList(completeSimulation.Treatments, ownerId);
            var cloneTargetConditionGoal = TargetConditionGoalCloner.CloneList(completeSimulation.TargetConditionGoals, ownerId);
            var cloneDeficientConditionGoal = DeficientConditionGoalCloner.CloneList(completeSimulation.DeficientConditionGoals, ownerId);
            var cloneBudget = BudgetCloner.CloneList(completeSimulation.Budgets, ownerId);
            var budgetIdMap = new Dictionary<Guid, Guid>();
            for (int budgetIndex = 0; budgetIndex < cloneBudget.Count; budgetIndex++)
            {
                budgetIdMap[completeSimulation.Budgets[budgetIndex].Id] = cloneBudget[budgetIndex].Id;
            }

            var cloneBaseCommittedProject = BaseCommittedProjectCloner.CloneList(completeSimulation.CommittedProjects, budgetIdMap);


           var clone = new CompleteSimulationDTO
            {
                NoTreatmentBeforeCommittedProjects = completeSimulation.NoTreatmentBeforeCommittedProjects,
                Name = cloneRequest.ScenarioName,
                NetworkId = cloneRequest.NetworkId,
                //figure out where the properties come from
                AnalysisMethod = cloneAnalysisMethod,
                ReportIndexes = cloneReportIndex,
                PerformanceCurves = cloneScenarioPerformanceCurvesImportResult,
                CalculatedAttributes = cloneCalculatedTribute,
                BudgetPriorities = cloneBudgetPriorities,
                CashFlowRules = cloneCashFlowFule,
                RemainingLifeLimits = cloneRemainingLifeLimits,
                Treatments = cloneTreatment,
                TargetConditionGoals = cloneTargetConditionGoal,
                DeficientConditionGoals = cloneDeficientConditionGoal,
                Budgets = cloneBudget,
                CommittedProjects = cloneBaseCommittedProject,
                Id = cloneRequest.Id,
            };
            return clone;

        }
        
    }
}
