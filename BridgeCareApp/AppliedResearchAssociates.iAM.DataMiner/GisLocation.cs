using System;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class GisLocation : Location
    {
        public GisLocation(Guid id, string wellKnownTextString, string uniqueIdentifier) : base(id, uniqueIdentifier)
        {
            WellKnownTextString = wellKnownTextString;
        }

        public string WellKnownTextString { get; }

        public override bool MatchOn(Location location)
        {
            throw new System.NotImplementedException();
        }
    }
}
