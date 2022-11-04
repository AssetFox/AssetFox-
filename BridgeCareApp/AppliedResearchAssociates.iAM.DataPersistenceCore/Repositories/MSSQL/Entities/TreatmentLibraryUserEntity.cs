using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentLibraryUserEntity: BaseEntity
    {
        public Guid TreatmentId { get; set; }

        public Guid UserId { get; set; }

        public bool CanModify { get; set; }

        public bool IsOwner { get; set; }

        public int AccessLevel { get; set; }

        public virtual TreatmentLibraryEntity TreatmentLibrary { get; set; }

        public virtual UserEntity User { get; set; }
    }
}
