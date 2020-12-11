using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class ConditionalTreatmentConsequenceEquationEntity
    {
        public Guid ConditionalTreatmentConsequenceId { get; set; }

        public Guid EquationId { get; set; }

        public virtual ConditionalTreatmentConsequenceEntity ConditionalTreatmentConsequence { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
