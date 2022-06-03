using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeDatumRepository : IAttributeDatumRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AttributeDatumRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void AddAssignedData(List<MaintainableAsset> maintainableAssets, List<AttributeDTO> attributeDtos)
        {
            var configurableAttributes = AttributeMapper.ToDomainListButDiscardBad(attributeDtos);

            // insert/update configurable attributes
            _unitOfWork.AttributeRepo.UpsertAttributes(configurableAttributes);

            // get the attribute ids off of the assigned data on the maintainable assets that have
            // not been modified yet
            var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData.Any())
                .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct());

            // get the attribute ids that will be used to delete the assigned data where there is an
            // attribute match in both the meta data file and the data source
            var attributeIdsToBeUpdatedWithAssignedData = configurableAttributes.Select(_ => _.Id).ToList()
                .Intersect(networkAttributeIds).Distinct().ToList();

            if (attributeIdsToBeUpdatedWithAssignedData.Any())
            {
                // use a raw sql query to delete AssignedData
                var query =
                    $"DELETE FROM dbo.AttributeDatum WHERE MaintainableAssetId IN (SELECT Id FROM dbo.MaintainableAsset WHERE NetworkId = '{maintainableAssets.First().NetworkId}') AND AttributeId IN ('{string.Join("','", attributeIdsToBeUpdatedWithAssignedData)}')";
                _unitOfWork.Context.Database.ExecuteSqlRaw(query);
                _unitOfWork.Context.SaveChanges();
            }

            // convert any assigned data to their equivalent entity object types
            var attributeDatumEntities = maintainableAssets
                .SelectMany(_ => _.AssignedData.Select(__ => __.ToEntity(_.Id)));

             var configAttributeIds = configurableAttributes.Select(s => s.Id).ToHashSet();
             var filtertedEnteties = attributeDatumEntities.Where(_ => configAttributeIds.Contains(_.AttributeId)).ToList();

            _unitOfWork.Context.AddAll(filtertedEnteties, _unitOfWork.UserEntity?.Id);

            var attributeDatumLocationEntities = filtertedEnteties.Select(_ => _.AttributeDatumLocation).ToList();

            _unitOfWork.Context.AddAll(attributeDatumLocationEntities, _unitOfWork.UserEntity?.Id);
        }
    }
}
