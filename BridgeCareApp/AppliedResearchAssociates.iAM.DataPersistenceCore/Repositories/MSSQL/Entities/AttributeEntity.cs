using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Guid AttributeDatumId { get; set; }

        [ForeignKey("AttributeDatumId")]
        public virtual AttributeDatumEntity AttributeData { get; set; }

    }
}
