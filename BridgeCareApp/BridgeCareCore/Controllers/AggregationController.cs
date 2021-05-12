﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : BridgeCareCoreBaseController
    {
        private readonly ILog _logger;

        public AggregationController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [Authorize]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {
            try
            {


                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();

                    var percentage = 0.0;
                    var configurationAttributes = new List<Attribute>();
                    var maintainableAssets = new List<MaintainableAsset>();
                    var benefitQuantifierEquation = new BenefitQuantifierDTO();
                    var networkAttributeIds = new List<Guid>();
                    var attributeData = new List<IAttributeDatum>();
                    var attributeIdsToBeUpdatedWithAssignedData = new List<Guid>();

                    var getResult = Task.Factory.StartNew(() =>
                    {
                        // Get/create configurable attributes
                        configurationAttributes = UnitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList();

                        var checkForDuplicateIDs = configurationAttributes.Select(_ => _.Id).ToList();

                        if(checkForDuplicateIDs.Count != checkForDuplicateIDs.Distinct().ToList().Count)
                        {
                            _logger.Error($"Error : Metadata.json file has duplicate Ids");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: Metadata.json file has duplicate Ids");
                            throw new InvalidOperationException();
                        }
                        var checkForDuplicateNames = configurationAttributes.Select(_ => _.Name).ToList();
                        if (checkForDuplicateNames.Count != checkForDuplicateNames.Distinct().ToList().Count)
                        {
                            _logger.Error($"Error : Metadata.json file has duplicate names");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: Metadata.json file has duplicate Names");
                            throw new InvalidOperationException();
                        }

                        // get all maintainable assets in the network with their assigned data (if any) and locations
                        maintainableAssets = UnitOfWork.MaintainableAssetRepo
                            .GetAllInNetworkWithAssignedDataAndLocations(networkId)
                            .ToList();

                        // get the network benefit quantifier equation
                        benefitQuantifierEquation = UnitOfWork.BenefitQuantifierRepo.GetBenefitQuantifier(maintainableAssets.First().NetworkId);

                        // Create list of attribute ids we are allowed to update with assigned data.
                        networkAttributeIds = maintainableAssets.Where(_ => _.AssignedData != null && _.AssignedData.Any())
                            .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                        // create list of attribute data from configuration attributes (exclude attributes
                        // that don't have command text as there will be no way to select data for them from
                        // the data source)
                        try
                        {
                            attributeData = configurationAttributes.Where(_ => !string.IsNullOrEmpty(_.Command))
                            .Select(AttributeConnectionBuilder.Build)
                            .SelectMany(AttributeDataBuilder.GetData).ToList();
                        }
                        catch(Exception e)
                        {
                            _logger.Error($"While getting data for the attributes (attributes coming from metaData.json file) -  {e.Message}");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: Fetching data for the attributes ::{e.Message}");
                            throw;
                        }
                        // get the attribute ids for assigned data that can be deleted (attribute is present
                        // in the data source and meta data file)
                        attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                            .Intersect(networkAttributeIds).Distinct().ToList();
                    });

                    var count = 0;
                    while (!getResult.IsCompleted)
                    {
                        if (count > 3)
                        {
                            count = 0;
                        }

                        SendCurrentStatusMessage(count, "Preparing", percentage);

                        count++;

                        Thread.Sleep(3000);
                    }

                    var aggregatedResults = new List<IAggregatedResult>();

                    var totalAssets = (double)maintainableAssets.Count;
                    var i = 0.0;

                    var aggregationResult = Task.Factory.StartNew(() =>
                    {
                        // loop over maintainable assets and remove assigned data that has an attribute id
                        // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                        // that was created
                        foreach (var maintainableAsset in maintainableAssets)
                        {
                            if (i % 500 == 0)
                            {
                                percentage = Math.Round((i / totalAssets) * 100, 1);
                            }
                            i++;

                            maintainableAsset.AssignedData.RemoveAll(_ =>
                                attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                            maintainableAsset.AssignAttributeData(attributeData);

                            //maintainableAsset.AssignSpatialWeighting(benefitQuantifierEquation.Equation.Expression);

                            // aggregate numeric data
                            if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "NUMBER"))
                            {
                                aggregatedResults.AddRange(maintainableAsset.AssignedData
                                    .Where(_ => _.Attribute.DataType == "NUMBER")
                                    .Select(_ => _.Attribute).Distinct()
                                    .Select(_ =>
                                        maintainableAsset.GetAggregatedValuesByYear(_,
                                            AggregationRuleFactory.CreateNumericRule(_)))
                                    .ToList());
                            }

                            //aggregate text data
                            if (maintainableAsset.AssignedData.Any(_ => _.Attribute.DataType == "STRING"))
                            {
                                aggregatedResults.AddRange(maintainableAsset.AssignedData
                                    .Where(_ => _.Attribute.DataType == "STRING")
                                    .Select(_ => _.Attribute).Distinct()
                                    .Select(_ => maintainableAsset.GetAggregatedValuesByYear(_,
                                        AggregationRuleFactory.CreateTextRule(_))).ToList());
                            }
                        }
                    });

                    count = 0;
                    while (!aggregationResult.IsCompleted)
                    {
                        if (count > 3)
                        {
                            count = 0;
                        }

                        SendCurrentStatusMessage(count, "Aggregating", percentage);

                        count++;

                        Thread.Sleep(3000);
                    }

                    var crudResult = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            UnitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets);
                        }
                        catch(Exception e)
                        {
                            _logger.Error($"Error while filling Assigned Data -  {e.Message}");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: while filling Assigned Data ::{e.Message}");
                            throw;
                        }
                        try
                        {
                            UnitOfWork.MaintainableAssetRepo.UpdateMaintainableAssetsSpatialWeighting(maintainableAssets);
                        }
                        catch(Exception e)
                        {
                            _logger.Error($"Error while Updating MaintainableAssets SpatialWeighting -  {e.Message}");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: while updating MaintainableAssets SpatialWeighting ::{e.Message}");
                            throw;
                        }

                        try
                        {
                            UnitOfWork.AggregatedResultRepo.AddAggregatedResults(aggregatedResults);
                        }
                        catch(Exception e)
                        {
                            _logger.Error($"Error while adding Aggregated results -  {e.Message}");
                            HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Error: while adding Aggregated results ::{e.Message}");
                            throw;
                        }
                    });

                    count = 0;
                    while (!crudResult.IsCompleted)
                    {
                        if (count > 3)
                        {
                            count = 0;
                        }

                        SendCurrentStatusMessage(count, "Saving", percentage);

                        count++;

                        Thread.Sleep(3000);
                    }

                    UnitOfWork.Commit();

                    HubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus,
                        "Aggregated all network data", percentage);
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus, "Aggregation failed", 0.0);
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Aggregation error::{e.Message}");
                throw;
            }
        }

        private void SendCurrentStatusMessage(int count, string status, double percentage)
        {
            var message = count switch
            {
                0 => $"{status}.",
                1 => $"{status}..",
                2 => $"{status}...",
                _ => status
            };

            HubService.SendRealTimeMessage(HubConstant.BroadcastAssignDataStatus,
                message, percentage);
        }
    }
}
