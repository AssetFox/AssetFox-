using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BenefitQuantifierEntity : BaseEntity
    {
        public Guid NetworkId { get; set; }

        public Guid EquationId { get; set; }

        public virtual NetworkEntity Network { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
