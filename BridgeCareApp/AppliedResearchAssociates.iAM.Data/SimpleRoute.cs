namespace AppliedResearchAssociates.iAM.Data.Attributes
{
    public class SimpleRoute : Route
    {
        public SimpleRoute(string name) : base(name)
        {
        }

        public override bool MatchOn(Route route)
        {
            throw new System.NotImplementedException();
        }
    }
}
