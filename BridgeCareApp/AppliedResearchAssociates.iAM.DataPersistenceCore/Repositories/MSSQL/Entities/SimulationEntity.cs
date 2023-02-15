using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SimulationEntity : BaseEntity
    {
        public SimulationEntity()
        {
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            SimulationUserJoins = new HashSet<SimulationUserEntity>();
            SimulationLogs = new HashSet<SimulationLogEntity>();
        }

        public Guid Id { get; set; }

        public Guid NetworkId { get; set; }

        public string Name { get; set; }

        public bool NoTreatmentBeforeCommittedProjects { get; set; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; }

        public virtual NetworkEntity Network { get; set; }

        public virtual AnalysisMethodEntity AnalysisMethod { get; set; }

        public virtual InvestmentPlanEntity InvestmentPlan { get; set; }

        public virtual ICollection<SimulationOutputEntity> SimulationOutputs { get; set; }

        public virtual ICollection<SimulationOutputJsonEntity> SimulationOutputJsons { get; set; }

        public virtual SimulationAnalysisDetailEntity SimulationAnalysisDetail { get; set; }

        public virtual SimulationReportDetailEntity SimulationReportDetail { get; set; }

        public virtual ICollection<SimulationLogEntity> SimulationLogs { get; set; }

        public virtual ICollection<ReportIndexEntity> SimulationReports { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<SimulationUserEntity> SimulationUserJoins { get; set; }

        public virtual ICollection<ScenarioPerformanceCurveEntity> PerformanceCurves { get; set; }

        public virtual ICollection<ScenarioCalculatedAttributeEntity> CalculatedAttributes { get; set; }

        public virtual ICollection<ScenarioSelectableTreatmentEntity> SelectableTreatments { get; set; }

        public virtual ICollection<ScenarioTargetConditionGoalEntity> ScenarioTargetConditionalGoals { get; set; }

        public virtual ICollection<ScenarioBudgetEntity> Budgets { get; set; }

        public virtual ICollection<ScenarioDeficientConditionGoalEntity> ScenarioDeficientConditionGoals { get; set; }

        public virtual ICollection<ScenarioBudgetPriorityEntity> BudgetPriorities { get; set; }

        public virtual ICollection<ScenarioRemainingLifeLimitEntity> RemainingLifeLimits { get; set; }

        public virtual ICollection<ScenarioCashFlowRuleEntity> CashFlowRules { get; set; }
    }
}
