﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AssetDetailEntity: AssetSummaryDetailEntity
    {
        public Guid SimulationYearDetailId { get; set; }

        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }

        public string AppliedTreatment { get; set; }

        public int TreatmentCause { get; set; }

        public ICollection<AssetDetailValueEntity> AssetDetailValues { get; set; } = new HashSet<AssetDetailValueEntity>();

        public ICollection<TreatmentConsiderationDetailEntity> TreatmentConsiderationDetails { get; } = new HashSet<TreatmentConsiderationDetailEntity>();

        public bool TreatmentFundingIgnoresSpendingLimit { get; set; }

        public ICollection<TreatmentOptionDetailEntity> TreatmentOptionDetails { get; } = new HashSet<TreatmentOptionDetailEntity>();

        public ICollection<TreatmentRejectionDetailEntity> TreatmentRejectionDetails { get; } = new HashSet<TreatmentRejectionDetailEntity>();

        public ICollection<TreatmentSchedulingCollisionDetailEntity> TreatmentSchedulingCollisionDetails { get; } = new HashSet<TreatmentSchedulingCollisionDetailEntity>();

        public int TreatmentStatus { get; set; }

    }
}
