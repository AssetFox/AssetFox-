using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeEntity
    {
        public AttributeEntity()
        {
            Benefits = new HashSet<BenefitEntity>();
            AggregatedResults = new HashSet<AggregatedResultEntity>();
            AttributeData = new HashSet<AttributeDatumEntity>();
            AnalysisMethods = new HashSet<AnalysisMethodEntity>();
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
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

        public virtual ICollection<BenefitEntity> Benefits { get; set; }
        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }
        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }
        public virtual ICollection<AnalysisMethodEntity> AnalysisMethods { get; set; }
        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }
    }
}
