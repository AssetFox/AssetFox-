using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class FacilityRepository : IFacilityRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public FacilityRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var existingFacilities = _unitOfWork.Context.Facility.Select(_ => _.Name).ToList();
            var existingSections = _unitOfWork.Context.Section.Select(_ => $"{_.Name}{_.Area}").ToList();

            var facilityEntities = facilities.Where(_ => !existingFacilities.Contains(_.Name)).Select(_ => _.ToEntity())
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Facility.AddRange(facilityEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(facilityEntities);
            }

            _unitOfWork.Context.SaveChanges();

            if (facilities.Any(_ => _.Sections.Any(__ => !existingSections.Contains($"{__.Name}{__.Area}"))))
            {
                var sections = facilities.Where(_ => _.Sections.Any(__ => !existingSections.Contains($"{__.Name}{__.Area}")))
                    .SelectMany(_ => _.Sections.Where(__ => !existingSections.Contains($"{__.Name}{__.Area}"))).ToList();

                _unitOfWork.SectionRepo.CreateSections(sections);
            }
        }
    }
}
