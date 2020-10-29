using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeDatumLocationEntity : LocationEntity
    {
        public AttributeDatumLocationEntity(Guid id, string discriminator, string uniqueIdentifier) : base(id, discriminator, uniqueIdentifier) { }

        public Guid AttributeDatumId { get; set; }

        [ForeignKey("AttributeDatumId")]
        public virtual AttributeDatumEntity AttributeDatum { get; set; }
    }
}
