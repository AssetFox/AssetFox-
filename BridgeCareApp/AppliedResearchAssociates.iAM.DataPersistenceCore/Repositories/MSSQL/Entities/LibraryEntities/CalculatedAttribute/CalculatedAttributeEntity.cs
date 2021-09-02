using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute
{
    public class CalculatedAttributeEntity : BaseCalculatedAttributeEntity
    {
        public CalculatedAttributeEntity() => Equations = new HashSet<CalculatedAttributeEquationCriteriaPairEntity>();

        public Guid CalculatedAttributeLibraryId { get; set; }

        public virtual CalculatedAttributeLibraryEntity CalculatedAttributeLibrary { get; set; }

        public ICollection<CalculatedAttributeEquationCriteriaPairEntity> Equations { get; set; }
    }
}
