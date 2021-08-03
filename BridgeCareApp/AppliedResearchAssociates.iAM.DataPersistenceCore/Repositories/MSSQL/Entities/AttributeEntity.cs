using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeEntity : BaseEntity
    {
        public AttributeEntity()
        {
            Benefits = new HashSet<BenefitEntity>();
            AggregatedResults = new HashSet<AggregatedResultEntity>();
            AttributeData = new HashSet<AttributeDatumEntity>();
            AnalysisMethods = new HashSet<AnalysisMethodEntity>();
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
            ScenarioPerformanceCurves = new HashSet<ScenarioPerformanceCurveEntity>();
            RemainingLifeLimits = new HashSet<RemainingLifeLimitEntity>();
            TreatmentConsequences = new HashSet<ConditionalTreatmentConsequenceEntity>();
            NumericAttributeValueHistories = new HashSet<NumericAttributeValueHistoryEntity>();
            TextAttributeValueHistories = new HashSet<TextAttributeValueHistoryEntity>();
            AttributeEquationCriterionLibraryJoins = new HashSet<AttributeEquationCriterionLibraryEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }

        public string AggregationRuleType { get; set; }

        public string Command { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public string DefaultValue { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public bool IsCalculated { get; set; }

        public bool IsAscending { get; set; }

        public virtual ICollection<AttributeEquationCriterionLibraryEntity> AttributeEquationCriterionLibraryJoins
        {
            get; set;
        }

        public virtual ICollection<BenefitEntity> Benefits { get; set; }

        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        public virtual ICollection<AnalysisMethodEntity> AnalysisMethods { get; set; }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }

        public virtual ICollection<ScenarioPerformanceCurveEntity> ScenarioPerformanceCurves { get; set; }

        public virtual ICollection<RemainingLifeLimitEntity> RemainingLifeLimits { get; set; }

        public virtual ICollection<ConditionalTreatmentConsequenceEntity> TreatmentConsequences { get; set; }

        public virtual ICollection<DeficientConditionGoalEntity> DeficientConditionGoals { get; set; }

        public virtual ICollection<NumericAttributeValueHistoryEntity> NumericAttributeValueHistories { get; set; }

        public virtual ICollection<TextAttributeValueHistoryEntity> TextAttributeValueHistories { get; set; }
    }
}
