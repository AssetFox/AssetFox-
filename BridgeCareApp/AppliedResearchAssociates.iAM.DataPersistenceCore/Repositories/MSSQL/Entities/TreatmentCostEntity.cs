using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentCostEntity
    {
        public Guid Id { get; set; }

        public Guid TreatmentId { get; set; }

        public virtual SelectableTreatmentEntity SelectableTreatment { get; set; }

        public virtual CriterionLibraryTreatmentCostEntity CriterionLibraryTreatmentCostJoin { get; set; }

        public virtual TreatmentCostEquationEntity TreatmentCostEquationJoin { get; set; }
    }
}
