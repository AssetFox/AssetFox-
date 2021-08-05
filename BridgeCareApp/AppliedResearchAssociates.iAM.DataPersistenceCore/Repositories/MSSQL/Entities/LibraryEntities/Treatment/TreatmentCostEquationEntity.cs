using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities
{
    public class TreatmentCostEquationEntity : BaseEquationJoinEntity
    {
        public Guid TreatmentCostId { get; set; }
        public virtual TreatmentCostEntity TreatmentCost { get; set; }
    }
}
