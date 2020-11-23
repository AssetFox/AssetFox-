using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SectionRepository : MSSQLRepository, ISectionRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public SectionRepository(IAMContext context) : base(context) { }

        public void CreateSections(Dictionary<Guid, List<Section>> sectionsPerFacilityId)
        {
            var sectionEntities = sectionsPerFacilityId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.Section.AddRange(sectionEntities);
            }
            else
            {
                Context.BulkInsert(sectionEntities);
            }

            Context.SaveChanges();
        }
    }
}
