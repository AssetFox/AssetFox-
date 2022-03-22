using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CRUDMethods<S, L>
        where S : class
        where L : class
    {
        public Action<Guid, List<S>> UpsertScenario { get; set; }
        public Func<Guid, List<S>> RetrieveScenario { get; set; }
        public Action<Guid, List<S>> DeleteScenario { get; set; }
        public Action<L> UpsertLibrary { get; set; }
        public Func<List<L>> RetrieveLibrary { get; set; }
        public Action<Guid> DeleteLibrary { get; set; }
    }
}
