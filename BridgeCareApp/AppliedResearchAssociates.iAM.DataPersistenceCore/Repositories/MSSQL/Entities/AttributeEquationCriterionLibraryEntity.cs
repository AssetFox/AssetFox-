using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class AttributeEquationCriterionLibraryEntity : BaseEntity
    {
        public Guid AttributeId { get; set; }

        public Guid EquationId { get; set; }

        public Guid? CriterionLibraryId { get; set; }

        public virtual AttributeEntity Attribute { get; set; }

        public virtual EquationEntity Equation { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
    }
}
