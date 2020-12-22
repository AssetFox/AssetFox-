using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AggregatedResultEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public string Discriminator { get; set; }

        public int Year { get; set; }

        public string TextValue { get; set; }

        public double? NumericValue { get; set; }

        public Guid MaintainableAssetId { get; set; }

        public Guid AttributeId { get; set; }

        public virtual AttributeEntity Attribute { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
    }
}
