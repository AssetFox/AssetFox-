using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeDatumEntity
    {
        public Guid Id { get; set; }
        public string Discriminator { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? NumericValue { get; set; }
        public string TextValue { get; set; }
        public Guid AttributeId { get; set; }
        public Guid MaintainableAssetId { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
        public virtual AttributeDatumLocationEntity AttributeDatumLocation { get; set; }
    }
}
