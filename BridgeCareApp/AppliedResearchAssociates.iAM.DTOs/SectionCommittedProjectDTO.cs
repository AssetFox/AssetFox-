using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        // TODO:  Change this once the data source for section-based networks has been added
        //        to ensure the key field is being added.  Also verify budget versus scenario
        public override bool VerifyLocation() => LocationKeys.Count == 3 ? true : false;

    }
}
