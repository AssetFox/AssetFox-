using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities
{
    public class PerformanceCurveEntity : BasePerformanceCurveEntity
    {
        public Guid PerformanceCurveLibraryId { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }

        public virtual CriterionLibraryPerformanceCurveEntity CriterionLibraryPerformanceCurveJoin { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }
    }
}
