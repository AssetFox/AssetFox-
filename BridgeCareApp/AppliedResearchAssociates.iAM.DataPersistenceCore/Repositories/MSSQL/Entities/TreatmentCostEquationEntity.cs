using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentCostEquationEntity
    {
        public Guid TreatmentCostId { get; set; }

        public Guid EquationId { get; set; }

        public virtual TreatmentCostEntity TreatmentCost { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
