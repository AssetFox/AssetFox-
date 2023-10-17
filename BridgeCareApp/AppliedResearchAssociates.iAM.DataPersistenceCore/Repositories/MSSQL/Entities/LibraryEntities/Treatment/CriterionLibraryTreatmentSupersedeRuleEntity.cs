using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class CriterionLibraryTreatmentSupersedeRuleEntity : BaseEntity
    {
        public Guid CriterionLibraryId { get; set; }

        public Guid TreatmentSupersedeRuleId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }

        public virtual TreatmentSupersedeRuleEntity TreatmentSupersedeRule { get; set; }
    }
}
