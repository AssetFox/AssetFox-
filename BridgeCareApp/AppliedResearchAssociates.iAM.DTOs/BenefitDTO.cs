using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class BenefitDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public double Limit { get; set; }
    }
}
