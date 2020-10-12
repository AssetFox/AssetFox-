using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Models
{
    public class Location
    {
        public Location(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;
        public string UniqueIdentifier { get; set; }
    }
}
