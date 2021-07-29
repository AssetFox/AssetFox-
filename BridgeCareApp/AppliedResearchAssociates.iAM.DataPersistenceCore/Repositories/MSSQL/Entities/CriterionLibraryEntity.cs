using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryEntity : LibraryEntity
    {
        public CriterionLibraryEntity()
        {
            CriterionLibraryAnalysisMethodJoins = new HashSet<CriterionLibraryAnalysisMethodEntity>();
            CriterionLibraryBudgetJoins = new HashSet<CriterionLibraryBudgetEntity>();
            CriterionLibraryBudgetPriorityJoins = new HashSet<CriterionLibraryBudgetPriorityEntity>();
            CriterionLibraryCashFlowRuleJoins = new HashSet<CriterionLibraryCashFlowRuleEntity>();
            CriterionLibraryDeficientConditionGoalJoins = new HashSet<CriterionLibraryDeficientConditionGoalEntity>();
            CriterionLibraryPerformanceCurveJoins = new HashSet<CriterionLibraryPerformanceCurveEntity>();
            CriterionLibraryScenarioPerformanceCurveJoins = new HashSet<CriterionLibraryScenarioPerformanceCurveEntity>();
            CriterionLibraryRemainingLifeLimitJoins = new HashSet<CriterionLibraryRemainingLifeLimitEntity>();
            CriterionLibrarySelectableTreatmentJoins = new HashSet<CriterionLibrarySelectableTreatmentEntity>();
            CriterionLibraryTreatmentConsequenceJoins = new HashSet<CriterionLibraryConditionalTreatmentConsequenceEntity>();
            CriterionLibraryTreatmentCostJoins = new HashSet<CriterionLibraryTreatmentCostEntity>();
            CriterionLibraryTreatmentSupersessionJoins = new HashSet<CriterionLibraryTreatmentSupersessionEntity>();
            AttributeEquationCriterionLibraryJoins = new HashSet<AttributeEquationCriterionLibraryEntity>();
            CriterionLibraryUserJoins = new HashSet<CriterionLibraryUserEntity>();
        }

        public string MergedCriteriaExpression { get; set; }

        public bool IsSingleUse { get; set; }

        public virtual ICollection<CriterionLibraryAnalysisMethodEntity> CriterionLibraryAnalysisMethodJoins { get; set; }

        public virtual ICollection<CriterionLibraryBudgetEntity> CriterionLibraryBudgetJoins { get; set; }

        public virtual ICollection<CriterionLibraryBudgetPriorityEntity> CriterionLibraryBudgetPriorityJoins { get; set; }

        public virtual ICollection<CriterionLibraryCashFlowRuleEntity> CriterionLibraryCashFlowRuleJoins { get; set; }

        public virtual ICollection<CriterionLibraryDeficientConditionGoalEntity> CriterionLibraryDeficientConditionGoalJoins { get; set; }

        public virtual ICollection<CriterionLibraryPerformanceCurveEntity> CriterionLibraryPerformanceCurveJoins { get; set; }

        public virtual ICollection<CriterionLibraryScenarioPerformanceCurveEntity> CriterionLibraryScenarioPerformanceCurveJoins { get; set; }

        public virtual ICollection<CriterionLibraryRemainingLifeLimitEntity> CriterionLibraryRemainingLifeLimitJoins { get; set; }

        public virtual ICollection<CriterionLibraryTargetConditionGoalEntity> CriterionLibraryTargetConditionGoalJoins { get; set; }

        public virtual ICollection<CriterionLibrarySelectableTreatmentEntity> CriterionLibrarySelectableTreatmentJoins { get; set; }

        public virtual ICollection<CriterionLibraryConditionalTreatmentConsequenceEntity> CriterionLibraryTreatmentConsequenceJoins { get; set; }

        public virtual ICollection<CriterionLibraryTreatmentCostEntity> CriterionLibraryTreatmentCostJoins { get; set; }

        public virtual ICollection<CriterionLibraryTreatmentSupersessionEntity> CriterionLibraryTreatmentSupersessionJoins { get; set; }

        public virtual ICollection<AttributeEquationCriterionLibraryEntity> AttributeEquationCriterionLibraryJoins { get; set; }

        public virtual ICollection<CriterionLibraryUserEntity> CriterionLibraryUserJoins { get; set; }
    }
}
