using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy
{
    public class PennDotReportARepository : MSSQLLegacyRepository, IPennDotReportARepository
    {
        public PennDotReportARepository(LegacyDbContext context) : base(context)
        {
        }

        public SortedSet<PennDotReportAEntity> GetPennDotReportAData(List<int> brKeys)
        {
            var results = Context.pennDotReportAResults.Where(_ => brKeys.Contains(_.BRKEY));
            var sortedResults = new SortedSet<PennDotReportAEntity>(new SortKeys());
            foreach (var item in results)
            {
                sortedResults.Add(item);
            }
            return sortedResults;
        }

        private class SortKeys : IComparer<PennDotReportAEntity>
        {
            public int Compare([AllowNull] PennDotReportAEntity x, [AllowNull] PennDotReportAEntity y)
            {
                return x.BRKEY.CompareTo(y.BRKEY);
            }
        }
    }
}
