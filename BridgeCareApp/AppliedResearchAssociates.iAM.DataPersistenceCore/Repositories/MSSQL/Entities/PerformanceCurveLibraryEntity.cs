using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveLibraryEntity : LibraryEntity
    {
        public PerformanceCurveLibraryEntity()
        {
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
        }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }
    }
}
