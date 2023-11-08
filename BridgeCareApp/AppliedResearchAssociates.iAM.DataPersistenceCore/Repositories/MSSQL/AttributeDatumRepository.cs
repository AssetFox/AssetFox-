using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.Mappers;
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
            var configurableAttributes = AttributeDtoDomainMapper.ToDomainList(attributeDtos, _unitOfWork.EncryptionKey);

            // insert/update configurable attributes
            _unitOfWork.AttributeRepo.UpsertAttributesNonAtomic(configurableAttributes);

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
            var filteredEntities = attributeDatumEntities.Where(_ => configAttributeIds.Contains(_.AttributeId)).ToList();

            _unitOfWork.Context.AddAll(filteredEntities, _unitOfWork.UserEntity?.Id);

            var attributeDatumLocationEntities = filteredEntities.Select(_ => _.AttributeDatumLocation).ToList();

            _unitOfWork.Context.AddAll(attributeDatumLocationEntities, _unitOfWork.UserEntity?.Id);
        }

        public List<AttributeDatumDTO> GetAllInNetwork(IEnumerable<Guid> networkMaintainableAssetIds, List<Guid> requiredAttributeIds)
        {
            var _ = _unitOfWork.Context.Attribute.ToList();  //Even though attributes are not used below, they have to be loaded into memory to avoid a crash.
            var attributeDatumDTOs = new List<AttributeDatumDTO>();
            var attributeDatumSet = _unitOfWork.Context.AttributeDatum;
            var attributeDatums = requiredAttributeIds?.Count > 0 ? attributeDatumSet.Where(_ => requiredAttributeIds.Contains(_.AttributeId)) : attributeDatumSet.Select(_ => _);

            foreach (var assetId in networkMaintainableAssetIds)
            {
                var attributeDatumsForAsset = attributeDatums.Where(_ => _.MaintainableAssetId == assetId).ToList();
                foreach (var attributeDatumForAsset in attributeDatumsForAsset)
                {
                    var attributeDatumDTO = new AttributeDatumDTO
                    {
                        MaintainableAssetId = assetId,
                        Id = attributeDatumForAsset.Id,
                        Attribute = attributeDatumForAsset.Attribute.Name,
                        NumericValue = attributeDatumForAsset.NumericValue,
                        TextValue = attributeDatumForAsset.TextValue
                    };
                    attributeDatumDTOs.Add(attributeDatumDTO);
                }
            }

            return attributeDatumDTOs;
        }
    }
}
