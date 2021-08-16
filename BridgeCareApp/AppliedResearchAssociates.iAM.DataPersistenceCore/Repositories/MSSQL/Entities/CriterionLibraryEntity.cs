﻿using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryEntity : LibraryEntity
    {
        public CriterionLibraryEntity()
        {
            CriterionLibraryAnalysisMethodJoins = new HashSet<CriterionLibraryAnalysisMethodEntity>();
            CriterionLibraryBudgetJoins = new HashSet<CriterionLibraryBudgetEntity>();
            CriterionLibraryScenarioBudgetJoins = new HashSet<CriterionLibraryScenarioBudgetEntity>();
            CriterionLibraryBudgetPriorityJoins = new HashSet<CriterionLibraryBudgetPriorityEntity>();
            CriterionLibraryCashFlowRuleJoins = new HashSet<CriterionLibraryCashFlowRuleEntity>();
            CriterionLibraryDeficientConditionGoalJoins = new HashSet<CriterionLibraryDeficientConditionGoalEntity>();
            CriterionLibraryPerformanceCurveJoins = new HashSet<CriterionLibraryPerformanceCurveEntity>();
            CriterionLibraryScenarioPerformanceCurveJoins = new HashSet<CriterionLibraryScenarioPerformanceCurveEntity>();
            CriterionLibraryRemainingLifeLimitJoins = new HashSet<CriterionLibraryRemainingLifeLimitEntity>();
            CriterionLibrarySelectableTreatmentJoins = new HashSet<CriterionLibrarySelectableTreatmentEntity>();
            CriterionLibraryScenarioSelectableTreatmentJoins = new HashSet<CriterionLibraryScenarioSelectableTreatmentEntity>();
            CriterionLibraryTreatmentConsequenceJoins = new HashSet<CriterionLibraryConditionalTreatmentConsequenceEntity>();
            CriterionLibraryScenarioTreatmentConsequenceJoins = new HashSet<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>();
            CriterionLibraryTreatmentCostJoins = new HashSet<CriterionLibraryTreatmentCostEntity>();
            CriterionLibraryScenarioTreatmentCostJoins = new HashSet<CriterionLibraryScenarioTreatmentCostEntity>();
            CriterionLibraryTreatmentSupersessionJoins = new HashSet<CriterionLibraryTreatmentSupersessionEntity>();
            CriterionLibraryScenarioTreatmentSupersessionJoins = new HashSet<CriterionLibraryScenarioTreatmentSupersessionEntity>();
            AttributeEquationCriterionLibraryJoins = new HashSet<AttributeEquationCriterionLibraryEntity>();
            CriterionLibraryUserJoins = new HashSet<CriterionLibraryUserEntity>();
        }

        public string MergedCriteriaExpression { get; set; }

        public bool IsSingleUse { get; set; }

        public virtual ICollection<CriterionLibraryAnalysisMethodEntity> CriterionLibraryAnalysisMethodJoins { get; set; }

        public virtual ICollection<CriterionLibraryBudgetEntity> CriterionLibraryBudgetJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioBudgetEntity> CriterionLibraryScenarioBudgetJoins { get; set; }

        public virtual ICollection<CriterionLibraryBudgetPriorityEntity> CriterionLibraryBudgetPriorityJoins { get; set; }

        public virtual ICollection<CriterionLibraryCashFlowRuleEntity> CriterionLibraryCashFlowRuleJoins { get; set; }

        public virtual ICollection<CriterionLibraryDeficientConditionGoalEntity> CriterionLibraryDeficientConditionGoalJoins { get; set; }
        public virtual ICollection<CriterionLibraryScenarioDeficientConditionGoalEntity> CriterionLibraryScenarioDeficientConditionGoalJoins { get; set; }

        public virtual ICollection<CriterionLibraryPerformanceCurveEntity> CriterionLibraryPerformanceCurveJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioPerformanceCurveEntity> CriterionLibraryScenarioPerformanceCurveJoins { get; set; }

        public virtual ICollection<CriterionLibraryRemainingLifeLimitEntity> CriterionLibraryRemainingLifeLimitJoins { get; set; }

        public virtual ICollection<CriterionLibraryTargetConditionGoalEntity> CriterionLibraryTargetConditionGoalJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioTargetConditionGoalEntity> CriterionLibraryScenarioTargetConditionGoalJoins { get; set; }

        public virtual ICollection<CriterionLibrarySelectableTreatmentEntity> CriterionLibrarySelectableTreatmentJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioSelectableTreatmentEntity> CriterionLibraryScenarioSelectableTreatmentJoins { get; set; }

        public virtual ICollection<CriterionLibraryConditionalTreatmentConsequenceEntity> CriterionLibraryTreatmentConsequenceJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity> CriterionLibraryScenarioTreatmentConsequenceJoins { get; set; }

        public virtual ICollection<CriterionLibraryTreatmentCostEntity> CriterionLibraryTreatmentCostJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioTreatmentCostEntity> CriterionLibraryScenarioTreatmentCostJoins { get; set; }

        public virtual ICollection<CriterionLibraryTreatmentSupersessionEntity> CriterionLibraryTreatmentSupersessionJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioTreatmentSupersessionEntity> CriterionLibraryScenarioTreatmentSupersessionJoins { get; set; }

        public virtual ICollection<AttributeEquationCriterionLibraryEntity> AttributeEquationCriterionLibraryJoins { get; set; }

        public virtual ICollection<CriterionLibraryUserEntity> CriterionLibraryUserJoins { get; set; }
    }
}
