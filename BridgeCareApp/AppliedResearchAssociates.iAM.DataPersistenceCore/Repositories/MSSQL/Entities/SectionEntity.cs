using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SectionEntity : BaseEntity
    {
        public SectionEntity()
        {
            CommittedProjects = new HashSet<CommittedProjectEntity>();
            NumericAttributeValueHistories = new HashSet<NumericAttributeValueHistoryEntity>();
            TextAttributeValueHistories = new HashSet<TextAttributeValueHistoryEntity>();
        }

        public Guid Id { get; set; }

        public Guid FacilityId { get; set; }

        public string Name { get; set; }

        public double Area { get; set; }

        public string AreaUnit { get; set; }

        public virtual FacilityEntity Facility { get; set; }

        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }

        public virtual ICollection<NumericAttributeValueHistoryEntity> NumericAttributeValueHistories { get; set; }

        public virtual ICollection<TextAttributeValueHistoryEntity> TextAttributeValueHistories { get; set; }
    }
}
