using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        public override bool VerifyLocation()
        {            
            const string IdKey = "ID";
            return LocationKeys.ContainsKey(NetworkKeyAttribute) && LocationKeys.ContainsKey(IdKey);
        }

    }
}
