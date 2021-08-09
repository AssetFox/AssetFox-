using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTreatmentCostEntity : BaseCriterionLibraryJoinEntity
    {
        public Guid TreatmentCostId { get; set; }

        public virtual TreatmentCostEntity TreatmentCost { get; set; }
    }
}
