using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SimulationCloning;

namespace BridgeCareCore.Services
{
    public class CompleteSimulationCloner
    {
        public static CompleteSimulationDTO Clone(CompleteSimulationDTO completeSimulation, CloneSimulationDTO cloneRequest, Guid ownerId, string ownerName)
        {
            var cloneAnalysisMethod = AnalysisMethodCloner.Clone(completeSimulation.AnalysisMethod, ownerId);            
            var cloneCashFlowFule = CashFlowRuleCloner.CloneList(completeSimulation.CashFlowRules, ownerId);
            var cloneInvestmentPlan = InvestmentPlanCloner.Clone(completeSimulation.InvestmentPlan);
            var cloneReportIndex = ReportIndexCloner.CloneList(completeSimulation.ReportIndexes);
            var clonePerformanceCurves = PerformanceCurvesCloner.CloneListNullPropagating(completeSimulation.PerformanceCurves, ownerId);
            var cloneCalculatedAttribute = CalculatedAttributeCloner.CloneList(completeSimulation.CalculatedAttributes, ownerId);
         
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
            var cloneBudgetPriorities = BudgetPriorityCloner.CloneList(completeSimulation.BudgetPriorities, budgetIdMap, ownerId);
            var cloneBaseCommittedProject = BaseCommittedProjectCloner.CloneList(completeSimulation.CommittedProjects, budgetIdMap);
            var user = new SimulationUserDTO
            {
                CanModify = true,
                UserId = ownerId,
                IsOwner = true,
                Username = ownerName,
            };
            var users = new List<SimulationUserDTO>
            {
                
            };
            if (ownerId != Guid.Empty&&! string.IsNullOrEmpty(ownerName))
            {
                users.Add(user);
            }
                        
            var cloneSimulationOutputJsons = SimulationOutputJsonCloner.CloneList(completeSimulation.SimulationOutputJsons);
            
            var simulationAnalysisDetail = SimulationAnalysisDetailCloner.Clone(completeSimulation.SimulationAnalysisDetail);

            var clone = new CompleteSimulationDTO
            {
                NoTreatmentBeforeCommittedProjects = completeSimulation.NoTreatmentBeforeCommittedProjects,
                Name = cloneRequest.ScenarioName,
                NetworkId = cloneRequest.NetworkId,
                //figure out where the properties come from
                AnalysisMethod = cloneAnalysisMethod,
                ReportIndexes = cloneReportIndex,
                PerformanceCurves = clonePerformanceCurves,
                CalculatedAttributes = cloneCalculatedAttribute,
                BudgetPriorities = cloneBudgetPriorities,
                CashFlowRules = cloneCashFlowFule,
                InvestmentPlan = cloneInvestmentPlan,
                RemainingLifeLimits = cloneRemainingLifeLimits,
                Treatments = cloneTreatment,
                TargetConditionGoals = cloneTargetConditionGoal,
                DeficientConditionGoals = cloneDeficientConditionGoal,
                Budgets = cloneBudget,
                CommittedProjects = cloneBaseCommittedProject,
                Id = Guid.NewGuid(),
                Users = users,
                SimulationOutputJsons = cloneSimulationOutputJsons,
                SimulationAnalysisDetail = simulationAnalysisDetail
            };

            return clone;
        }        
    }
}
