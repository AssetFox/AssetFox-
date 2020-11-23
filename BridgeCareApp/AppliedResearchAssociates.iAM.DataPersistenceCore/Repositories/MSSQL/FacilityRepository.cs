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

        public void CreateFacilities(List<Facility> facilities, string networkName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Network.Any(_ => _.Name == networkName))
                    {
                        throw new RowNotInTableException($"No network found having name {networkName}");
                    }

                    var networkEntity = Context.Network.Single(_ => _.Name == networkName);

                    var facilityEntities = facilities.Select(_ => _.ToEntity(networkEntity.Id)).ToList();

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
                        var sectionsPerFacilityId = facilities.Where(_ => _.Sections.Any())
                            .ToDictionary(_ => facilityEntities.Single(__ => __.Name == _.Name).Id,
                                _ => _.Sections.ToList());

                        _sectionRepository.CreateSections(sectionsPerFacilityId);
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
