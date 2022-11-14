using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve
{
    public class PerformanceCurveLibraryEntity : LibraryEntity
    {
        //public PerformanceCurveLibraryEntity() => PerformanceCurves = new HashSet<PerformanceCurveEntity>();
        public PerformanceCurveLibraryEntity()
        {
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
            Users = new HashSet<PerformanceCurveLibraryUserEntity>();
        }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }

        public virtual ICollection<PerformanceCurveLibraryUserEntity> Users { get; set; }
    }
}
