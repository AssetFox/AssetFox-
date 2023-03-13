using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute
{
    public class CalculatedAttributeLibraryEntity : LibraryEntity
    {
        public bool IsDefault { get; set; } = false;

        public CalculatedAttributeLibraryEntity()
        {
            CalculatedAttributes = new HashSet<CalculatedAttributeEntity>();
            Users = new HashSet<CalculatedAttributeLibraryUserEntity>();
        }
        public virtual ICollection<CalculatedAttributeEntity> CalculatedAttributes { get; set; }
        public virtual ICollection<CalculatedAttributeLibraryUserEntity> Users { get; set; }
    }
}
