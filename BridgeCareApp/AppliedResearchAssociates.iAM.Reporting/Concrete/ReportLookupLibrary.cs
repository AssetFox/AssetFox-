using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Reporting
{
    /// <summary>
    /// Provides data to lookup an object type based on a name
    /// </summary>
    /// <remarks>
    /// This object is required to wrap the Dictionary during lookup so that it can be configured during IoC Container
    /// </remarks>
    public class ReportLookupLibrary
    {
        public ReportLookupLibrary(Dictionary<string, Type> init)
        {
            Lookup = init;
        }

        public Dictionary<string, Type> Lookup { get; set; }
    }
}
