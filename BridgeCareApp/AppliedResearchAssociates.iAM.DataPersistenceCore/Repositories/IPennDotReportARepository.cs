using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPennDotReportARepository
    {
        SortedSet<PennDotReportAEntity> GetPennDotReportAData(List<int> brKeys);
    }
}
