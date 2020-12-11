using System;
using System.Collections.Generic;
using System.Data;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class FacilityRepository : MSSQLRepository, IFacilityRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ISectionRepository _sectionRepository;

        public FacilityRepository(ISectionRepository sectionRepository, IAMContext context) : base(context) => _sectionRepository = sectionRepository ?? throw new ArgumentNullException(nameof(sectionRepository));

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Network.Any(_ => _.Id == networkId))
                    {
                        throw new RowNotInTableException($"No network found having id {networkId}");
                    }

                    var facilityEntities = facilities.Select(_ => _.ToEntity()).ToList();

                    if (IsRunningFromXUnit)
                    {
                        Context.Facility.AddRange(facilityEntities);
                    }
                    else
                    {
                        Context.BulkInsert(facilityEntities);
                    }

                    Context.SaveChanges();

                    if (facilities.Any(_ => _.Sections.Any()))
                    {
                        var sections = facilities.Where(_ => _.Sections.Any())
                            .SelectMany(_ => _.Sections).ToList();

                        _sectionRepository.CreateSections(sections);
                    }

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
