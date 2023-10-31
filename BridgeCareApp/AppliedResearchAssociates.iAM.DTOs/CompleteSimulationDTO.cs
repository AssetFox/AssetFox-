using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Serialization-friendly aggregate of all respositories of a simulation.
    /// </summary>
    public class CompleteSimulationDTO : SimulationDTO
    {
               
        /// <summary>
        /// The Analysis repository of a simulation.
        /// </summary>
        public AnalysisMethodDTO AnalysisMethod { get; set; }

        /// <summary>
        /// The Investment repository of a simulation
        /// </summary>
        public InvestmentPlanDTO InvestmentPlan { get; set; }

        /// <summary>
        /// The Report repository of a simulation.
        /// </summary>
        public IList<ReportIndexDTO> ReportIndexes { get; set; }

        /// <summary>
        /// The Project repository of a simulation.
        /// </summary>
        public IList<BaseCommittedProjectDTO> CommittedProjects { get; set; }

        /// <summary>
        /// The Performance Curve repository of a simulation.
        /// </summary>
        public IList<PerformanceCurveDTO> PerformanceCurves { get; set; }

        /// <summary>
        /// The Attribute repository of a simulation.
        /// </summary>
        public IList<CalculatedAttributeDTO> CalculatedAttributes { get; set; }

        /// <summary>
        /// The Treatment repository of a simulation.
        /// </summary>
        public IList<TreatmentDTO> Treatments { get; set; }

        /// <summary>
        /// The Target Condition Goal repository of a simulation.
        /// </summary>
        public IList<TargetConditionGoalDTO> TargetConditionGoals { get; set; }

        /// <summary>
        /// The Budget repository of a simulation.
        /// </summary>
        public IList<BudgetDTO> Budgets { get; set; }

        /// <summary>
        /// The Deficient Condition Goal repository of a simulation.
        /// </summary>
        public IList<DeficientConditionGoalDTO> DeficientConditionGoals { get; set; }

        /// <summary>
        /// The Budget Priority repository of a simulation.
        /// </summary>
        public IList<BudgetPriorityDTO> BudgetPriorities { get; set; }

        /// <summary>
        /// The Remaining Life Limit repository of a simulation.
        /// </summary>
        public IList<RemainingLifeLimitDTO> RemainingLifeLimits { get; set; }

        /// <summary>
        /// The Cash Flow Rule repository of a simulation.
        /// </summary>
        public IList<CashFlowRuleDTO> CashFlowRules { get; set; }
    }
}
