using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DataType { get; set; }

        [Required]
        public string AggregationRuleType { get; set; }

        [Required]
        public string Command { get; set; }

        [Required]
        public ConnectionType ConnectionType { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        public virtual ICollection<AggregatedResultEntity> AggregatedResults { get; set; }
    }
}
