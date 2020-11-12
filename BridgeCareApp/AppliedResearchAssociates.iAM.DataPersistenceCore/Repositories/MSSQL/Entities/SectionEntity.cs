using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SectionEntity
    {
        public SectionEntity()
        {
            CommittedProjects = new HashSet<CommittedProjectEntity>();
        }

        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public string AreaUnit { get; set; }

        public virtual FacilityEntity Facility { get; set; }
        public virtual ICollection<CommittedProjectEntity> CommittedProjects { get; set; }
    }
}
