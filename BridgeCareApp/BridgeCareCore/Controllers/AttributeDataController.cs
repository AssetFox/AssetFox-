using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeDataController : ControllerBase
    {
        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IMaintainableAssetRepository _maintainableAssetRepo;
        private readonly IAttributeDatumRepository _attributeDatumRepo;
        private readonly ILogger<NetworkController> _logger;

        public AttributeDataController(IAttributeMetaDataRepository attributeMetaDataRepo,
            IMaintainableAssetRepository maintainableAssetRepo,
            IAttributeDatumRepository attributeDatumRepo,
            ILogger<NetworkController> logger)
        {
            _attributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _maintainableAssetRepo = maintainableAssetRepo ?? throw new ArgumentNullException(nameof(maintainableAssetRepo));
            _attributeDatumRepo = attributeDatumRepo ?? throw new ArgumentNullException(nameof(attributeDatumRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("AssignNetworkData")]
        public IActionResult AssignNetworkData([FromBody] Guid networkId)
        {
            try
            {
                // Get/create configurable attributes
                var configurationAttributes = _attributeMetaDataRepo.GetAllAttributes().ToList();

                // get all maintainable assets in the network with their assigned data (if any) and locations
                var maintainableAssets = _maintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId)
                    .ToList();

                // Create list of attribute ids we are allowed to update with assigned data.
                var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                    .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                // create list of attribute data from configuration attributes (exclude attributes that don't have command text as there
                // will be no way to select data for them from the data source)
                var attributeData = configurationAttributes.Where(_ => !string.IsNullOrEmpty(_.Command))
                    .Select(AttributeConnectionBuilder.Build)
                    .SelectMany(AttributeDataBuilder.GetData).ToList();

                // get the attribute ids for assigned data that can be deleted (attribute is present
                // in the data source and meta data file)
                var attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                    .Intersect(networkAttributeIds).Distinct().ToList();

                // loop over maintainable assets and remove assigned data that has an attribute id
                // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                // that was created
                foreach (var maintainableAsset in maintainableAssets)
                {
                    maintainableAsset.AssignedData.RemoveAll(_ => attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                    maintainableAsset.AssignAttributeData(attributeData);
                }

                // update the maintainable assets assigned data in the data source
                var updatedRecordsCount = _attributeDatumRepo.UpdateAssignedData(maintainableAssets);

                _logger.LogInformation("Attribute data have been assigned to maintenance assets.");
                return Ok($"Updated {updatedRecordsCount} records.");
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
