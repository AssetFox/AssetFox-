using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveLibraryEntity
    {
        public PerformanceCurveLibraryEntity()
        {
            PerformanceCurves = new HashSet<PerformanceCurveEntity>();
            PerformanceCurveLibrarySimulationJoins = new HashSet<PerformanceCurveLibrarySimulationEntity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PerformanceCurveEntity> PerformanceCurves { get; set; }
        public virtual ICollection<PerformanceCurveLibrarySimulationEntity> PerformanceCurveLibrarySimulationJoins { get; set; }
    }
}
