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

        public string Name { get; set; }

        public string Command { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public virtual ICollection<AttributeDatumEntity> AttributeData { get; set; }

        public virtual AggregationResultEntity AggregationResult { get; set; }
    }
}
