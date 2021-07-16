using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public FacilityRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateFacilities(List<Facility> facilities, Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var existingFacilities = _unitOfWork.Context.Facility.Select(_ => _.Name).ToList();

            var facilityEntities = facilities.Where(_ => !existingFacilities.Contains(_.Name)).Select(_ => _.ToEntity())
                .ToList();

            _unitOfWork.Context.AddAll(facilityEntities, _unitOfWork.UserEntity?.Id);

            if (!facilities.Any(_ => _.Sections.Any()))
            {
                return;
            }

            var sections = facilities.Where(_ => _.Sections.Any())
                .SelectMany(_ => _.Sections).ToList();

            _unitOfWork.SectionRepo.CreateSections(sections);
        }
    }
}
