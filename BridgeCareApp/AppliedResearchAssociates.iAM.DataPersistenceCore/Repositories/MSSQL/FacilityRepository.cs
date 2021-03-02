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

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public FacilityRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var facilityEntities = facilities.Select(_ => _.ToEntity()).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Facility.AddRange(facilityEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(facilityEntities);
            }

            _unitOfWork.Context.SaveChanges();

            if (facilities.Any(_ => _.Sections.Any()))
            {
                var sections = facilities.Where(_ => _.Sections.Any())
                    .SelectMany(_ => _.Sections).ToList();

                _unitOfWork.SectionRepo.CreateSections(sections);
            }
        }
    }
}
