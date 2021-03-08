using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveLibraryEntity : LibraryEntity
    {
        public PerformanceCurveLibraryEntity()
        {
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
            PerformanceCurveLibrarySimulationJoins = new HashSet<PerformanceCurveLibrarySimulationEntity>();
        }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }

        public virtual ICollection<PerformanceCurveLibrarySimulationEntity> PerformanceCurveLibrarySimulationJoins { get; set; }
    }
}
