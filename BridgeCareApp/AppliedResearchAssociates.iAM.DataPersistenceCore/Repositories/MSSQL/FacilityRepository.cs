using System;
using System.Collections.Generic;
using System.Data;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class FacilityRepository : IFacilityRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ISectionRepository _sectionRepository;
        private readonly IAMContext _context;

        public FacilityRepository(ISectionRepository sectionRepository, IAMContext context)
        {
            _sectionRepository = sectionRepository ?? throw new ArgumentNullException(nameof(sectionRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            if (!_context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var facilityEntities = facilities.Select(_ => _.ToEntity()).ToList();

            if (IsRunningFromXUnit)
            {
                _context.Facility.AddRange(facilityEntities);
            }
            else
            {
                _context.BulkInsert(facilityEntities);
            }

            if (facilities.Any(_ => _.Sections.Any()))
            {
                var sections = facilities.Where(_ => _.Sections.Any())
                    .SelectMany(_ => _.Sections).ToList();

                _sectionRepository.CreateSections(sections);
            }
        }
    }
}
