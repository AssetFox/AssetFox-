using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class FacilityRepository : IFacilityRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public FacilityRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var facilityEntities = facilities.Select(_ => _.ToEntity()).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.Facility.AddRange(facilityEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(facilityEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (facilities.Any(_ => _.Sections.Any()))
            {
                var sections = facilities.Where(_ => _.Sections.Any())
                    .SelectMany(_ => _.Sections).ToList();

                _unitOfDataPersistenceWork.SectionRepo.CreateSections(sections);
            }
        }
    }
}
