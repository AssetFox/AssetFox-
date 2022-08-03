using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentRejectionDetailEntity: BaseEntity
    {
        public Guid AssetDetailId { get; set; }
        public virtual AssetDetailEntity AssetDetail { get; set; }
        public string TreatmentName { get; }

        public TreatmentRejectionReason TreatmentRejectionReason { get; }
    }
}
