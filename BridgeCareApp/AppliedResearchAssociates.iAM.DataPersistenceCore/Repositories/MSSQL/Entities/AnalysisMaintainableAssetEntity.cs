using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    /// <summary>
    ///     This is the entity representing the "analysis version" of a maintainable asset, as they
    ///     used to be called "sections".
    /// </summary>
    public class AnalysisMaintainableAssetEntity : BaseEntity
    {
        public AnalysisMaintainableAssetEntity()
        {
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            NumericAttributeValueHistories = new HashSet<NumericAttributeValueHistoryEntity>();
            TextAttributeValueHistories = new HashSet<TextAttributeValueHistoryEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual EquationEntity SpatialWeighting { get; set; }

        public Guid NetworkId { get; set; }

        public virtual NetworkEntity Network { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<NumericAttributeValueHistoryEntity> NumericAttributeValueHistories { get; set; }

        public virtual ICollection<TextAttributeValueHistoryEntity> TextAttributeValueHistories { get; set; }
    }
}
