using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentOptionDetailEntity: BaseEntity
    {
        public Guid AssetDetailId { get; set; }
        public virtual AssetDetailEntity AssetDetail { get; set; }
        public double Benefit { get; }

        public double Cost { get; }

        public double? RemainingLife { get; }

        public string TreatmentName { get; }
    }
}
