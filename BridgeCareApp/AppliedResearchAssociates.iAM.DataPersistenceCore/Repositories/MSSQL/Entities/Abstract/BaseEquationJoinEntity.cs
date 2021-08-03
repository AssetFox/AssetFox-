using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract
{
    public class BaseEquationJoinEntity : BaseEntity
    {
        public Guid EquationId { get; set; }

        public virtual EquationEntity Equation { get; set; }
    }
}
