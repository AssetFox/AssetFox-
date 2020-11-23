using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISectionRepository
    {
        void CreateSections(Dictionary<Guid, List<Section>> sectionsPerFacilityId);
    }
}
