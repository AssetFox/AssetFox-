using System.Collections.Generic;
using System.Reflection.Metadata;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class TreatmentLibraryEntity : LibraryEntity
    {
        public TreatmentLibraryEntity()
        {
            Treatments = new HashSet<SelectableTreatmentEntity>();
            Users = new HashSet<TreatmentLibraryUserEntity>();
        }
        public virtual ICollection<TreatmentLibraryUserEntity> Users { get; set; }
        public virtual ICollection<SelectableTreatmentEntity> Treatments { get; set; }
    }
}
