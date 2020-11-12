using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentConsequenceEquationEntity
    {
        public Guid TreatmentConsequenceId { get; set; }

        public Guid EquationId { get; set; }

        public virtual TreatmentConsequenceEntity TreatmentConsequence { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
