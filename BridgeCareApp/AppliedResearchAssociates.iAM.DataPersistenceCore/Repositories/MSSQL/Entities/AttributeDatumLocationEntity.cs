using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeDatumLocationEntity : LocationEntity
    {
        public AttributeDatumLocationEntity(Guid id, string discriminator, string locationIdentifier) : base(id, discriminator, locationIdentifier) { }

        public Guid AttributeDatumId { get; set; }

        [ForeignKey("AttributeDatumId")]
        public virtual AttributeDatumEntity AttributeDatum { get; set; }
    }
}
