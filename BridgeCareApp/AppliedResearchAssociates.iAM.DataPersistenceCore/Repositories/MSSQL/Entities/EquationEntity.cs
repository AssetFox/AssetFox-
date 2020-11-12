using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class EquationEntity
    {
        public Guid Id { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }
        public virtual TreatmentConsequenceEquationEntity TreatmentConsequenceEquationJoin { get; set; }
        public virtual TreatmentCostEquationEntity TreatmentCostEquationJoin { get; set; }
    }
}
