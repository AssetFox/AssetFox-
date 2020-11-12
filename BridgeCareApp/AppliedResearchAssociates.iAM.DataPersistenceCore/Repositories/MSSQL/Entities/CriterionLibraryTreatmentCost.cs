using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public partial class CriterionLibraryTreatmentCost
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid TreatmentCostId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual TreatmentCostEntity TreatmentCost { get; set; }
    }
}
