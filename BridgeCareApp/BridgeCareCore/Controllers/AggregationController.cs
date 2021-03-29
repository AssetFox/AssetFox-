using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AggregationController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfDataPersistenceWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [Authorize]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {
            var percentage = 0.0;
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Starting data assignment", percentage);

                    // Get/create configurable attributes
                    var configurationAttributes = _unitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList();

                    // get all maintainable assets in the network with their assigned data (if any) and locations
                    var maintainableAssets = _unitOfWork.MaintainableAssetRepo.GetAllInNetworkWithAssignedDataAndLocations(networkId)
                        .ToList();

                    // Create list of attribute ids we are allowed to update with assigned data.
                    var networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                        .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                    // create list of attribute data from configuration attributes (exclude attributes
                    // that don't have command text as there will be no way to select data for them from
                    // the data source)
                    var attributeData = configurationAttributes.Where(_ => !string.IsNullOrEmpty(_.Command))
                        .Select(AttributeConnectionBuilder.Build)
                        .SelectMany(AttributeDataBuilder.GetData).ToList();

                    // get the attribute ids for assigned data that can be deleted (attribute is present
                    // in the data source and meta data file)
                    var attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                        .Intersect(networkAttributeIds).Distinct().ToList();

                    var totalAssests = (double)maintainableAssets.Count;
                    var i = 0.0;
                    // loop over maintainable assets and remove assigned data that has an attribute id
                    // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                    // that was created
                    foreach (var maintainableAsset in maintainableAssets)
                    {
                        if (i % 500 == 0)
                        {
                            percentage = Math.Round((i / totalAssests) * 100, 1);
                            _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Assigning attribute data", percentage);
                        }
                        i++;
                        maintainableAsset.AssignedData.RemoveAll(_ => attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                        maintainableAsset.AssignAttributeData(attributeData);
                    }

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus,
                        "Finished assigning attribute data. Saving it to the datasource...", percentage);

                    // update the maintainable assets assigned data in the data source
                    var updatedRecordsCount = _unitOfWork.AttributeDatumRepo.UpdateAssignedData(maintainableAssets);

                    AggregateData(networkId, maintainableAssets);

                    _unitOfWork.Commit();

                    _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus,
                        "Attribute data has been aggregated to maintenance assets.");
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Aggregation error::{e.Message}");
                throw;
            }
        }

        private void AggregateData(Guid networkId, List<MaintainableAsset> maintainableAssets)
        {
            var percentage = 0.0;

            _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Starting data aggregation", percentage);

            var aggregatedResults = new List<IAggregatedResult>();

            var totalAssests = (double)maintainableAssets.Count;
            var i = 0.0;
            // loop over the maintainable assets and aggregate the assigned data as numeric or
            // text based on assigned data attribute data type
            foreach (var maintainableAsset in maintainableAssets)
            {
                if (i % 500 == 0)
                {
                    percentage = Math.Round((i / totalAssests) * 100, 1);
                    _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Aggregating data", percentage);
                }

                i++;
                // aggregate numeric data
                if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMBER"))
                {
                    aggregatedResults.AddRange(maintainableAsset.AssignedData
                        .Where(_ => _.Attribute.DataType == "NUMBER")
                        .Select(_ => _.Attribute)
                        .Select(_ =>
                            maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateNumericRule(_)))
                        .ToList());
                }

                //aggregate text data
                if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "STRING"))
                {
                    aggregatedResults.AddRange(maintainableAsset.AssignedData
                        .Where(_ => _.Attribute.DataType == "STRING")
                        .Select(_ => _.Attribute)
                        .Select(_ =>
                            maintainableAsset.GetAggregatedValuesByYear(_, AggregationRuleFactory.CreateTextRule(_)))
                        .ToList());
                }
            }

            // TODO: assign spatial weighting using MaintainableAsset.AssignSpatialWeighting
            // maintainableAssets.Select(_ => _.AssignSpatialWeighting());

            _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus,
                "Finished aggregating attribute data. Saving it to the datasource...", percentage);

            // create aggregated data records in the data source
            var createdRecordsCount = _unitOfWork.AggregatedResultRepo.CreateAggregatedResults(aggregatedResults);

            _hubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Successfully aggregated the data", percentage);
        }
    }
}
