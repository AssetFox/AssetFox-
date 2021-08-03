using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TreatmentLibraryEntity : LibraryEntity
    {
        public TreatmentLibraryEntity()
        {
            Treatments = new HashSet<SelectableTreatmentEntity>();
        }

        public virtual ICollection<SelectableTreatmentEntity> Treatments { get; set; }
    }
}
