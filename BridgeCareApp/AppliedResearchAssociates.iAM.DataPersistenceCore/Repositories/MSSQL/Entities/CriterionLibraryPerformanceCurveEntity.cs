using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class CriterionLibraryPerformanceCurveEntity
    {
        public Guid CriterionLibraryId { get; set; }
        public Guid PerformanceCurveId { get; set; }

        public virtual CriterionLibraryEntity CriterionLibrary { get; set; }
        public virtual PerformanceCurveEntity PerformanceCurve { get; set; }
    }
}
