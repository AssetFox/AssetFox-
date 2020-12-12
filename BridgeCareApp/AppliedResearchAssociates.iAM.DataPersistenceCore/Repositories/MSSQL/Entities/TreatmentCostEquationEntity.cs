using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentCostEquationEntity : BaseEntity
    {
        public Guid TreatmentCostId { get; set; }

        public Guid EquationId { get; set; }

        public virtual TreatmentCostEntity TreatmentCost { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
