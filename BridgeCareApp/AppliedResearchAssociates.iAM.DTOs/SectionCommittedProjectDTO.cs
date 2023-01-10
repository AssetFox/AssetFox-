using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        public override bool VerifyLocation(string networkKeyAttribute)
        {
            const string IdKey = "ID";
            return LocationKeys.ContainsKey(networkKeyAttribute) && LocationKeys.ContainsKey(IdKey);
        }
    }
}
