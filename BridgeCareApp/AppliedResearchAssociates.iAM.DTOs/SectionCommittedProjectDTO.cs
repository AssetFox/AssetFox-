using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class SectionCommittedProjectDTO : BaseCommittedProjectDTO
    {
        public override TOutput Accept<TOutput>(Abstract.IBaseCommittedProjectDtoVisitor<TOutput> visitor)
        {
            return visitor.Visit(this);
        } 

        public override bool VerifyLocation(string networkKeyAttribute)
        {
            const string IdKey = "ID";
            return LocationKeys.ContainsKey(networkKeyAttribute) && LocationKeys.ContainsKey(IdKey);
        }
    }
}
