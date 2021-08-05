using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities
{
    public class PerformanceCurveEquationEntity : BaseEquationJoinEntity
    {
        public Guid PerformanceCurveId { get; set; }

        public virtual PerformanceCurveEntity PerformanceCurve { get; set; }
    }
}
