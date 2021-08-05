using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve
{
    public class PerformanceCurveEntity : BasePerformanceCurveEntity
    {
        public Guid PerformanceCurveLibraryId { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }

        public virtual CriterionLibraryPerformanceCurveEntity CriterionLibraryPerformanceCurveJoin { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }
    }
}
