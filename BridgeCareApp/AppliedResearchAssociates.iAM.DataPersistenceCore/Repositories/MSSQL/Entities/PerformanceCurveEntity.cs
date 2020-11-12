using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class PerformanceCurveEntity
    {
        public Guid Id { get; set; }

        public Guid PerformanceCurveLibraryId { get; set; }

        public Guid AttributeId { get; set; }

        public string Name { get; set; }

        public bool Shift { get; set; }

        public virtual PerformanceCurveLibraryEntity PerformanceCurveLibrary { get; set; }

        public virtual CriterionLibraryPerformanceCurveEntity CriterionLibraryPerformanceCurveJoin { get; set; }

        public virtual PerformanceCurveEquationEntity PerformanceCurveEquationJoin { get; set; }

        public virtual AttributeEntity Attribute { get; set; }
    }
}
