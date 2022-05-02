using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CRUDMethods<S, L>
        where S : BaseDTO
        where L : BaseLibraryDTO
    {
        public Action<Guid, List<S>> UpsertScenario { get; set; }
        public Func<Guid, List<S>> RetrieveScenario { get; set; }
        public Action<Guid, List<S>> DeleteScenario { get; set; }
        public Action<L> UpsertLibrary { get; set; }
        public Func<List<L>> RetrieveLibrary { get; set; }
        public Action<Guid> DeleteLibrary { get; set; }
    }

    public class NoLibraryCRUDMethods<S> where S : BaseDTO
    {
        public Action<Guid, S> UpsertScenario { get; set; }
        public Func<Guid, S> RetrieveScenario { get; set; }
        public Action<Guid> DeleteScenario { get; set; }
    }
}
