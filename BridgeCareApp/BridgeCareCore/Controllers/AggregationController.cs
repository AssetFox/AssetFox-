using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Logging;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregationController : BridgeCareCoreBaseController
    {
        private int _count;
        private string _status = string.Empty;
        private double _percentage;
        private Guid _networkId = Guid.Empty;
        private readonly ILog _log;

        public AggregationController(ILog log, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _log = log ?? throw new ArgumentNullException(nameof(log));

        [HttpPost]
        [Route("AggregateNetworkData/{networkId}")]
        [Authorize]
        public async Task<IActionResult> AggregateNetworkData(Guid networkId)
        {
            try
            {
                _networkId = networkId;
                var isError = false;
                var errorMessage = "";

                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();

                    var maintainableAssets = new List<MaintainableAsset>();
                    var attributeData = new List<IAttributeDatum>();
                    var attributeIdsToBeUpdatedWithAssignedData = new List<Guid>();

                    _status = "Preparing";
                    var getResult = Task.Factory.StartNew(() =>
                    {
                        UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);

                        // Get/create configurable attributes
                        var configurationAttributes = UnitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList();

                        var checkForDuplicateIDs = configurationAttributes.Select(_ => _.Id).ToList();

                        if (checkForDuplicateIDs.Count != checkForDuplicateIDs.Distinct().ToList().Count)
                        {
                            _log.Error($"Error : Metadata.json file has duplicate Ids");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: Metadata.json file has duplicate Ids");
                            throw new InvalidOperationException();
                        }

                        var checkForDuplicateNames = configurationAttributes.Select(_ => _.Name).ToList();
                        if (checkForDuplicateNames.Count != checkForDuplicateNames.Distinct().ToList().Count)
                        {
                            _log.Error($"Error : Metadata.json file has duplicate names");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: Metadata.json file has duplicate Names");
                            throw new InvalidOperationException();
                        }

                        // get all maintainable assets in the network with their assigned data (if any) and locations
                        maintainableAssets = UnitOfWork.MaintainableAssetRepo
                            .GetAllInNetworkWithAssignedDataAndLocations(networkId)
                            .ToList();

                        // Create list of attribute ids we are allowed to update with assigned data.
                        var networkAttributeIds = maintainableAssets
                            .Where(_ => _.AssignedData != null && _.AssignedData.Any())
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
                        catch (Exception e)
                        {
                            _log.Error($"While getting data for the attributes (attributes coming from metaData.json file) -  {e.Message}");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: Fetching data for the attributes ::{e.Message}");
                            isError = true;
                            errorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }

                        // get the attribute ids for assigned data that can be deleted (attribute is present
                        // in the data source and meta data file)
                        attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                            .Intersect(networkAttributeIds).Distinct().ToList();
                    });

                    CheckCurrentLongRunningTask(getResult);

                    var aggregatedResults = new List<IAggregatedResult>();

                    var totalAssets = (double)maintainableAssets.Count;
                    var i = 0.0;

                    _status = "Aggregating";
                    var aggregationResult = Task.Factory.StartNew(() =>
                    {
                        UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
                        // loop over maintainable assets and remove assigned data that has an attribute id
                        // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                        // that was created
                        foreach (var maintainableAsset in maintainableAssets)
                        {
                            if (i % 500 == 0)
                            {
                                _percentage = Math.Round((i / totalAssets) * 100, 1);
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

                    CheckCurrentLongRunningTask(aggregationResult);

                    _status = "Saving";
                    var crudResult = Task.Factory.StartNew(() =>
                    {
                        UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);

                        try
                        {
                            UnitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets);
                        }
                        catch(Exception e)
                        {
                            _log.Error($"Error while filling Assigned Data -  {e.Message}");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: while filling Assigned Data ::{e.Message}");
                            isError = true;
                            errorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }
                        try
                        {
                            UnitOfWork.MaintainableAssetRepo.UpdateMaintainableAssetsSpatialWeighting(maintainableAssets);
                        }
                        catch(Exception e)
                        {
                            _log.Error($"Error while Updating MaintainableAssets SpatialWeighting -  {e.Message}");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: while updating MaintainableAssets SpatialWeighting ::{e.Message}");
                            isError = true;
                            errorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }

                        try
                        {
                            UnitOfWork.AggregatedResultRepo.AddAggregatedResults(aggregatedResults);
                        }
                        catch(Exception e)
                        {
                            _log.Error($"Error while adding Aggregated results -  {e.Message}");
                            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Error: while adding Aggregated results ::{e.Message}");
                            isError = true;
                            errorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }
                    });


                    CheckCurrentLongRunningTask(crudResult);

                    if (!isError)
                    {
                        _status = "Aggregated all network data";
                        UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
                        UnitOfWork.Commit();

                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                        new NetworkRollupDetailDTO { NetworkId = _networkId, Status = _status }, _percentage);
                    }
                    else
                    {
                        _status = $"Error in data aggregation {errorMessage}";
                        UnitOfWork.Rollback();
                        _percentage = 0;
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                        new NetworkRollupDetailDTO { NetworkId = _networkId, Status = _status }, _percentage);
                    }
                });
                if (isError)
                {
                    return StatusCode(500, errorMessage);
                }
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                _status = "Aggregation failed";
                UnitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO {NetworkId = _networkId, Status = _status}, 0.0);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Aggregation error::{e.Message}");
                throw;
            }
        }

        private void CheckCurrentLongRunningTask(Task currentLongRunningTask)
        {
            _count = 0;
            var cts = new CancellationTokenSource();

            var currentStatusMessageTask = CreateCurrentStatusMessageTask(cts.Token);

            while (!currentLongRunningTask.IsCompleted)
            {
                if (currentStatusMessageTask.IsCompleted)
                {
                    currentStatusMessageTask = CreateCurrentStatusMessageTask(cts.Token);
                }
            }

            if (!currentStatusMessageTask.IsCompleted)
            {
                cts.Cancel();
            }
        }

        private Task CreateCurrentStatusMessageTask(CancellationToken token) =>
            Task.Run(async () =>
            {
                await Task.Delay(3000, token);
                if (_count > 3)
                {
                    _count = 0;
                }
                SendCurrentStatusMessage();
                _count++;
            }, token);

        private void SendCurrentStatusMessage()
        {
            var message = _count switch
            {
                0 => $"{_status}.",
                1 => $"{_status}..",
                2 => $"{_status}...",
                _ => _status
            };

            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                new NetworkRollupDetailDTO {NetworkId = _networkId, Status = message}, _percentage);
        }
    }
}
