using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NetworkEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<SegmentEntity> SegmentEntities { get; set; }
    }
}
