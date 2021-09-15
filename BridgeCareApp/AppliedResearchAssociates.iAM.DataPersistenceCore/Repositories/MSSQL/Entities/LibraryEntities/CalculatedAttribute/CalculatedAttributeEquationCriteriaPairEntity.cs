using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute
{
    public class CalculatedAttributeEquationCriteriaPairEntity : BaseCalculatedAttributeEquationPairEntity
    {
        public Guid CalculatedAttributeId { get; set; }

        public virtual CalculatedAttributeEntity CalculatedAttribute { get; set; }

        public virtual CriterionLibraryCalculatedAttributePairEntity CriterionLibraryCalculatedAttributeJoin { get; set; }

        public virtual EquationCalculatedAttributePairEntity EquationCalculatedAttributeJoin { get; set; }
    }
}
