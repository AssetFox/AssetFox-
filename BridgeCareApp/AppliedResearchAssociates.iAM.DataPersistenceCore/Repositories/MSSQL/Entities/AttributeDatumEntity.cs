using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class AttributeDatumEntity<T>
    {
        [Key]
        Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public Guid AttributeId { get; set; }
        public DateTime TimeStamp { get; set; }
        public T Value { get; set; }

        [ForeignKey("LocationId")]
        public virtual LocationEntity Location { get; set; }

        [ForeignKey("AttributeId")]
        public virtual AttributeEntity Attribute { get; set; }
    }
}
