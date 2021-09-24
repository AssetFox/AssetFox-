using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISectionRepository
    {
        void CreateSections(List<Section> sections);
    }
}
