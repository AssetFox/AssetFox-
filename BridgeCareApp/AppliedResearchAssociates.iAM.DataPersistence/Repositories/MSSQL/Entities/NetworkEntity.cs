using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistence.Models;

namespace AppliedResearchAssociates.iAM.DataPersistence.Repositories.MSSQL.Entities
{
    public class NetworkEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<SegmentEntity> Segments { get; set; }
    }
}
