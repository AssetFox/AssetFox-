using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class FacilityEntity
    {
        public FacilityEntity()
        {
            Sections = new HashSet<SectionEntity>();
        }

        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }
        public string Name { get; set; }

        public virtual NetworkEntity Network { get; set; }
        public ICollection<SectionEntity> Sections { get; set; }
    }
}
