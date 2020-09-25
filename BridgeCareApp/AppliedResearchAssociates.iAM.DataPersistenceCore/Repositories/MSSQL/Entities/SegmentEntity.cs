﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class SegmentEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid NetworkId { get; set; }
        public Guid AttributeDatumId { get; set; }

        [ForeignKey("NetworkId")]
        public virtual NetworkEntity Network { get; set; }

        [ForeignKey("AttributeDatumId")]
        public virtual AttributeDatumEntity AttributeDatum { get; set; }

        public virtual LocationEntity Location { get; set; }
    }
}
