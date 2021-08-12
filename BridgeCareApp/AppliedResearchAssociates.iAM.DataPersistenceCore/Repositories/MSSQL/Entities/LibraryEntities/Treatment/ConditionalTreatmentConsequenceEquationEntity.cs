using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class ConditionalTreatmentConsequenceEquationEntity : BaseEntity
    {
        public Guid ConditionalTreatmentConsequenceId { get; set; }

        public Guid EquationId { get; set; }

        public virtual ConditionalTreatmentConsequenceEntity ConditionalTreatmentConsequence { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
