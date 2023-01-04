using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        // TODO:  Also verify budget versus scenario - how to test this??
        public override bool VerifyLocation()
        {            
            const string idKey = "ID";
            return LocationKeys.ContainsKey(NetworkKeyAttribute) && LocationKeys.ContainsKey(idKey);
        }

    }
}
