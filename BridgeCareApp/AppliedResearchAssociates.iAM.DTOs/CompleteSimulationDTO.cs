using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CompleteSimulationDTO : SimulationDTO
    {
        public AnalysisMethodDTO AnalysisMethod { get; set; }

        public InvestmentPlanDTO InvestmentPlan { get; set; }

        public IList<ReportIndexDTO> ReportIndexes { get; set; }

        public IList<BaseCommittedProjectDTO> CommittedProjects { get; set; }

        public IList<ScenarioPerformanceCurvesImportResultDTO> PerformanceCurves { get; set; }

        public IList<CalculatedAttributeDTO> CalculatedAttributes { get; set; }

        public IList<TreatmentDTO> Treatments { get; set; }

        public IList<TargetConditionGoalDTO> TargetConditionGoals { get; set; }

        public IList<BudgetDTO> Budgets { get; set; }

        public IList<DeficientConditionGoalDTO> DeficientConditionGoals { get; set; }

        public IList<BudgetPriorityDTO> BudgetPriorities { get; set; }

        public IList<RemainingLifeLimitDTO> RemainingLifeLimits { get; set; }

        public IList<CashFlowRuleDTO> CashFlowRules { get; set; }
    }
}
