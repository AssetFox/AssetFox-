using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        public override TOutput Accept<TOutput, THelper>(Abstract.IBaseCommittedProjectDtoVisitor<THelper, TOutput> visitor, THelper helper)
        {
            return visitor.Visit(this, helper);
        } 

        public override bool VerifyLocation(string networkKeyAttribute)
        {
            const string IdKey = "ID";
            return LocationKeys.ContainsKey(networkKeyAttribute) && LocationKeys.ContainsKey(IdKey);
        }
    }
}
