﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public abstract class SimulationOutputValueEntity: BaseEntity
    {
        public Guid Id { get; set; }

        public string Discriminator { get; set; }

        public string TextValue { get; set; }

        public double? NumericValue { get; set; }

        public Guid MaintainableAssetId { get; set; } // WjJake

        public Guid AttributeId { get; set; }

        public virtual AttributeEntity Attribute { get; set; }

        public virtual MaintainableAssetEntity MaintainableAsset { get; set; }
    }
}
