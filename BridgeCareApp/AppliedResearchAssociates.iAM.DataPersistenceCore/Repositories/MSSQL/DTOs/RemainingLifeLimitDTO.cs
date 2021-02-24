using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class RemainingLifeLimitDTO : BaseDTO
    {
        public string Attribute { get; set; }
        public double Value { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
