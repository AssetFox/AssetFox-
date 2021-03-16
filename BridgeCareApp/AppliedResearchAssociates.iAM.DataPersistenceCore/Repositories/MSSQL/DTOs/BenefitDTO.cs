using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class BenefitDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public double Limit { get; set; }
    }
}
