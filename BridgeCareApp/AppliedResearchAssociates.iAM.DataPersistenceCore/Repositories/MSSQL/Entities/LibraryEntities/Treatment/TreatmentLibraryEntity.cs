using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment
{
    public class TreatmentLibraryEntity : LibraryEntity
    {
        public TreatmentLibraryEntity()
        {
            Treatments = new HashSet<SelectableTreatmentEntity>();
        }

        public virtual ICollection<SelectableTreatmentEntity> Treatments { get; set; }
        public virtual ICollection<TreatmentLibraryUserEntity> TreatmentLibraryUserJoins { get; set; }
    }
}
