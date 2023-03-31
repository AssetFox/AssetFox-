using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentRejectionDetailEntity
    {
        public Guid Id { get; set; }

        public Guid AssetDetailId { get; set; }

        public virtual AssetDetailEntity AssetDetail { get; set; }

        public double PotentialConditionChange { get; set; }

        public string TreatmentName { get; set; }

        public int TreatmentRejectionReason { get; set; }
    }
}
