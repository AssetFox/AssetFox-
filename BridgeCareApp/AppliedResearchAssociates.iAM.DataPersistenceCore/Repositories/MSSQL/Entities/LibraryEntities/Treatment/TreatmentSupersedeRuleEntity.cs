using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class TreatmentSupersedeRuleEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public Guid PreventTreatmentId { get; set; }

        public virtual CriterionLibraryTreatmentSupersedeRuleEntity CriterionLibraryTreatmentSupersedeRuleJoin { get; set; }
    }
}
