using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class TreatmentSupersessionEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public virtual CriterionLibraryTreatmentSupersessionEntity CriterionLibraryTreatmentSupersessionJoin { get; set; }

        public virtual CriterionLibraryScenarioTreatmentSupersessionEntity CriterionLibraryScenarioTreatmentSupersessionJoin { get; set; }
    }
}
