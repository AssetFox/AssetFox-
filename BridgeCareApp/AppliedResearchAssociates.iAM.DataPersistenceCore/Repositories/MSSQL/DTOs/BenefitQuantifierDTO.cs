using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BenefitQuantifierDTO
    {
        public Guid NetworkId { get; set; }
        public EquationDTO Equation { get; set; }
    }
}
