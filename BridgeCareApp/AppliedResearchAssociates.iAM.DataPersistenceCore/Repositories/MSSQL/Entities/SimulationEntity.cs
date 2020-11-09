﻿using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationEntity
    {
        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }
        public string Name { get; set; }
        public int NumberOfYearsOfTreatmentOutlook { get; set; }

        public virtual NetworkEntity Network { get; set; }
        public virtual AnalysisMethodEntity AnalysisMethod { get; set; }
        public virtual BudgetPriorityLibrarySimulationEntity BudgetPriorityLibrarySimulationJoin { get; set; }
        public virtual CashFlowRuleLibrarySimulationEntity CashFlowRuleLibrarySimulationJoin { get; set; }
        public virtual DeficientConditionGoalLibrarySimulationEntity DeficientConditionGoalLibrarySimulationJoin { get; set; }
        public virtual InvestmentPlanEntity InvestmentPlan { get; set; }
        public virtual PerformanceCurveLibrarySimulationEntity PerformanceCurveLibrarySimulationJoin { get; set; }
        public virtual RemainingLifeLimitLibrarySimulationEntity RemainingLifeLimitLibrarySimulationJoin { get; set; }
        public virtual SimulationOutputEntity SimulationOutput { get; set; }
        public virtual TargetConditionGoalLibrarySimulationEntity TargetConditionGoalLibrarySimulationJoin { get; set; }
        public virtual TreatmentLibrarySimulationEntity TreatmentLibrarySimulationJoin { get; set; }
    }
}
