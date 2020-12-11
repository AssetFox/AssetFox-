using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryTreatmentCostEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentCostId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual TreatmentCostEntity TreatmentCost { get; set; }
    }
}
