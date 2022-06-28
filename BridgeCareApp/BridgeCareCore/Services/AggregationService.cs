﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Writer = System.Threading.Channels.ChannelWriter<AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.NetworkRollupDetailDTO>;

namespace BridgeCareCore.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private int _count;
        private string _status = string.Empty;
        private double _percentage;
        private Guid _networkId = Guid.Empty;
        public AggregationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AggregateNetworkData(Writer writer, Guid networkId, AggregationState state)
        {
            var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();

            state.NetworkId = networkId;
            var isError = false;
            var errorMessage = "";

            await Task.Factory.StartNew(() =>
            {
                _unitOfWork.BeginTransaction();

                var maintainableAssets = new List<MaintainableAsset>();
                var attributeData = new List<IAttributeDatum>();
                var attributeIdsToBeUpdatedWithAssignedData = new List<Guid>();

                _status = "Preparing";
                var getResult = Task.Factory.StartNew(() =>
                {
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);

                        // Get/create configurable attributes
                    var configurationAttributes = AttributeMapper.ToDomainListButDiscardBad(allAttributes);

                    var checkForDuplicateIDs = configurationAttributes.Select(_ => _.Id).ToList();

                    if (checkForDuplicateIDs.Count != checkForDuplicateIDs.Distinct().ToList().Count)
                    {
                        var broadcastError = $"Error : Metadata.json file has duplicate Ids"; // Wjwjwj error message here is outdated
                        DoublyBroadcastError(broadcastError);
                        throw new InvalidOperationException();
                    }

                    var checkForDuplicateNames = configurationAttributes.Select(_ => _.Name).ToList();
                    if (checkForDuplicateNames.Count != checkForDuplicateNames.Distinct().ToList().Count)
                    {
                        var broadcastError = $"Error : Metadata.json file has duplicate names";
                        DoublyBroadcastError(broadcastError);
                        throw new InvalidOperationException();
                    }

                        // get all maintainable assets in the network with their assigned data (if any) and locations
                    maintainableAssets = _unitOfWork.MaintainableAssetRepo
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
                        var broadcastError = $"Error: Fetching data for the attributes ::{e.Message}";
                        DoublyBroadcastError(broadcastError);
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
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
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
                        try
                        {
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
                        catch (Exception e)
                        {
                            var broadcastError = $"Error: Creating aggregation rule(s) for the attributes :: {e.Message}";
                            DoublyBroadcastError(broadcastError);
                            throw;
                        }
                    }
                });

                CheckCurrentLongRunningTask(aggregationResult);

                _status = "Saving";
                var crudResult = Task.Factory.StartNew(() =>
                {
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);

                    try
                    {
                        _unitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets, allAttributes);
                    }
                    catch (Exception e)
                    {
                        var broadcastError = $"Error while filling Assigned Data -  {e.Message}";
                        DoublyBroadcastError(broadcastError);
                        isError = true;
                        errorMessage = e.Message;
                        throw new Exception(e.StackTrace);
                    }
                    try
                    {
                        _unitOfWork.MaintainableAssetRepo.UpdateMaintainableAssetsSpatialWeighting(maintainableAssets);
                    }
                    catch (Exception e)
                    {
                        var broadcastError = $"Error while Updating MaintainableAssets SpatialWeighting -  {e.Message}";
                        DoublyBroadcastError(broadcastError);
                        isError = true;
                        errorMessage = e.Message;
                        throw new Exception(e.StackTrace);
                    }

                    try
                    {
                        _unitOfWork.AggregatedResultRepo.AddAggregatedResults(aggregatedResults);
                    }
                    catch (Exception e)
                    {
                        var broadcastError = $"Error while adding Aggregated results -  {e.Message}";
                        DoublyBroadcastError(broadcastError);
                        isError = true;
                        errorMessage = e.Message;
                        throw new Exception(e.StackTrace);
                    }
                });


                CheckCurrentLongRunningTask(crudResult);

                if (!isError)
                {
                    _status = "Aggregated all network data";
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(_networkId, _status);
                    _unitOfWork.Commit();

                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = _networkId, Status = _status }, _percentage);
                }
                else
                {
                    _status = $"Error in data aggregation {errorMessage}";
                    _unitOfWork.Rollback();
                    _percentage = 0;
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastAssignDataStatus,
                    new NetworkRollupDetailDTO { NetworkId = _networkId, Status = _status }, _percentage);
                }
            });
            return !isError;
        }
    }
}
