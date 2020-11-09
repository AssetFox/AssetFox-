using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveEquationEntity
    {
        public Guid PerformanceCurveId { get; set; }
        public Guid EquationId { get; set; }

        public virtual PerformanceCurveEntity PerformanceCurve { get; set; }
        public virtual EquationEntity Equation { get; set; }
    }
}
