namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class SectionLocation : Location
    {
        public SectionLocation(string uniqueIdentifier) : base(uniqueIdentifier)
        {
        }

        public override bool MatchOn(Location location)
        {
            return location is SectionLocation sectionLocation ? sectionLocation.UniqueIdentifier == UniqueIdentifier : false;
        }
    }
}
