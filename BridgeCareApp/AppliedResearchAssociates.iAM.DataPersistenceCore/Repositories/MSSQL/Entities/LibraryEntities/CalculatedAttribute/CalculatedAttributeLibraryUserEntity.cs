using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute
{
    public class CalculatedAttributeLibraryUserEntity : BaseEntity
    {
        public Guid CalculatedAttributeLibraryId { get; set; }
        public Guid UserId { get; set; }
        public int AccessLevel { get; set; }

        public virtual CalculatedAttributeLibraryEntity CalculatedAttributeLibrary { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
