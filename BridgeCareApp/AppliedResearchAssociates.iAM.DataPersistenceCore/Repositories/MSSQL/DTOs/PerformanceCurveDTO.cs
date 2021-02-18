using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class PerformanceCurveDTO
    {
        public Guid Id { get; set; }
        public string Attribute { get; set; }
        public string Name { get; set; }
        public bool Shift { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
        public EquationDTO Equation { get; set; }
    }
}
