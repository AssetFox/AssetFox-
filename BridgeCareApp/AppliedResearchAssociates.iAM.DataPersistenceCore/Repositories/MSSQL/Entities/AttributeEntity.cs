using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;

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
            ScenarioRemainingLifeLimits = new HashSet<ScenarioRemainingLifeLimitEntity>();
            TreatmentConsequences = new HashSet<ConditionalTreatmentConsequenceEntity>();
            ScenarioTreatmentConsequences = new HashSet<ScenarioConditionalTreatmentConsequenceEntity>();
            NumericAttributeValueHistories = new HashSet<NumericAttributeValueHistoryEntity>();
            TextAttributeValueHistories = new HashSet<TextAttributeValueHistoryEntity>();
            AttributeEquationCriterionLibraryJoins = new HashSet<AttributeEquationCriterionLibraryEntity>();
            AssetDetailValuesIntId = new HashSet<AssetDetailValueEntityIntId>();
            AssetSummaryDetailValuesIntId = new HashSet<AssetSummaryDetailValueEntityIntId>();
            DeficientConditionGoalDetails = new HashSet<DeficientConditionGoalDetailEntity>();
            TargetConditionGoalDetails = new HashSet<TargetConditionGoalDetailEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }

        public string AggregationRuleType { get; set; }

        public string Command { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public DataSourceEntity DataSource { get; set; }

        public Guid? DataSourceId { get; set; }

        public string DefaultValue { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        public bool IsCalculated { get; set; }

        public bool IsAscending { get; set; }

        public virtual ICollection<AttributeEquationCriterionLibraryEntity> AttributeEquationCriterionLibraryJoins { get; set; }

        public virtual ICollection<BenefitEntity> Benefits { get; set; }

        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        public virtual ICollection<AnalysisMethodEntity> AnalysisMethods { get; set; }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }

        public virtual ICollection<CalculatedAttributeEntity> CalculatedAttributes { get; set; }

        public virtual ICollection<ScenarioPerformanceCurveEntity> ScenarioPerformanceCurves { get; set; }

        public virtual ICollection<ScenarioCalculatedAttributeEntity> ScenarioCalculatedAttributes { get; set; }

        public virtual ICollection<RemainingLifeLimitEntity> RemainingLifeLimits { get; set; }

        public virtual ICollection<ScenarioRemainingLifeLimitEntity> ScenarioRemainingLifeLimits { get; set; }

        public virtual ICollection<ConditionalTreatmentConsequenceEntity> TreatmentConsequences { get; set; }

        public virtual ICollection<ScenarioConditionalTreatmentConsequenceEntity> ScenarioTreatmentConsequences { get; set; }

        public virtual ICollection<DeficientConditionGoalEntity> DeficientConditionGoals { get; set; }

        public virtual ICollection<ScenarioDeficientConditionGoalEntity> ScenarioDeficientConditionGoals { get; set; }

        public virtual ICollection<NumericAttributeValueHistoryEntity> NumericAttributeValueHistories { get; set; }

        public virtual ICollection<TextAttributeValueHistoryEntity> TextAttributeValueHistories { get; set; }

        public virtual ICollection<AssetDetailValueEntityIntId> AssetDetailValuesIntId { get; set; }

        public virtual ICollection<AssetSummaryDetailValueEntityIntId> AssetSummaryDetailValuesIntId { get; set; }

        public virtual ICollection<DeficientConditionGoalDetailEntity> DeficientConditionGoalDetails { get; set; }

        public virtual ICollection<TargetConditionGoalDetailEntity> TargetConditionGoalDetails { get; set; }
    }
}
