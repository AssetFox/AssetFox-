using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    public class CompleteSimulationCloner
    {
        public static CompleteSimulationDTO Clone(CompleteSimulationDTO completeSimulation, CloneSimulationDTO cloneRequest)
        {
            var cloneAnalysisMethod = AnalysisMethodCloner.Clone(completeSimulation.AnalysisMethod);
            var cloneBudgetPriorities = BudgetPriorityCloner.CloneList(completeSimulation.BudgetPriorities);
            var cloneCashFlowFule = CashFlowRuleCloner.CloneList(completeSimulation.CashFlowRules);
            var cloneInvestmentPlan = InvestmentPlanCloner.Clone(completeSimulation.InvestmentPlan);
            var cloneReportIndex = ReportIndexCloner.CloneList(completeSimulation.ReportIndexes);
            var cloneScenarioPerformanceCurvesImportResult = ScenarioPerformanceCurvesImportResultCloner.CloneList(completeSimulation.PerformanceCurves);
            var cloneCalculatedTribute = CalculatedAttributeCloner.CloneList(completeSimulation.CalculatedAttributes);
            var cloneRemainingLifeLimits = RemainingLifeLimitCloner.CloneList(completeSimulation.RemainingLifeLimits);
            var cloneTreatment = TreatmentCloner.CloneList(completeSimulation.Treatments);
            var cloneTargetConditionGoal = TargetConditionGoalCloner.CloneList(completeSimulation.TargetConditionGoals);
            var cloneDeficientConditionGoal = DeficientConditionGoalCloner.CloneList(completeSimulation.DeficientConditionGoals);
            var cloneBudget = BudgetCloner.CloneList(completeSimulation.Budgets);
            var cloneBaseCommittedProject = BaseCommittedProjectCloner.CloneList(completeSimulation.CommittedProjects);


           var clone = new CompleteSimulationDTO
            {
                NoTreatmentBeforeCommittedProjects = completeSimulation.NoTreatmentBeforeCommittedProjects,
                Name = cloneRequest.scenarioName,
                NetworkId = cloneRequest.networkId,
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
            };
            return clone;

        }
        
    }
}
