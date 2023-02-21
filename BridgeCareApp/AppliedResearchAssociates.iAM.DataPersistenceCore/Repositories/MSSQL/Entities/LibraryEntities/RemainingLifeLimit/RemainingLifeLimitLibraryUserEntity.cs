using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit
{
    public class RemainingLifeLimitLibraryUserEntity : LibraryUserBaseEntity
    {
        public virtual RemainingLifeLimitLibraryEntity RemainingLifeLimitLibrary { get; set; }
    }
}
