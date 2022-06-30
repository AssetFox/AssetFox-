using System;
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
using BridgeCareCore.Models;
using Writer = System.Threading.Channels.ChannelWriter<BridgeCareCore.Services.Aggregation.AggregationStatusMemo>;

namespace BridgeCareCore.Services.Aggregation
{
    public class AggregationService : IAggregationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AggregationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AggregateNetworkData(Writer writer, Guid networkId, AggregationState state, UserInfo userInfo)
        {
            var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();

            state.NetworkId = networkId;
            var isError = false;
            state.ErrorMessage = "";

            await Task.Run(() =>
            {
                try
                {
                    _unitOfWork.BeginTransaction();

                    var maintainableAssets = new List<MaintainableAsset>();
                    var attributeData = new List<IAttributeDatum>();
                    var attributeIdsToBeUpdatedWithAssignedData = new List<Guid>();

                    state.Status = "Preparing";
                    // WjTodo -- uncomment this and get it not to throw    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);  // DbUpdateException here -- "The wait operation timed out."

                    // Get/create configurable attributes
                    var configurationAttributes = AttributeMapper.ToDomainListButDiscardBad(allAttributes);

                    var checkForDuplicateIDs = configurationAttributes.Select(_ => _.Id).ToList();

                    if (checkForDuplicateIDs.Count != checkForDuplicateIDs.Distinct().ToList().Count)
                    {
                        var broadcastError = $"Error : Metadata.json file has duplicate Ids"; // Wjwjwj error message here is outdated
                        WriteError(writer, broadcastError);
                        throw new InvalidOperationException();
                    }

                    var checkForDuplicateNames = configurationAttributes.Select(_ => _.Name).ToList();
                    if (checkForDuplicateNames.Count != checkForDuplicateNames.Distinct().ToList().Count)
                    {
                        var broadcastError = $"Error : Metadata.json file has duplicate names";
                        WriteError(writer, broadcastError);
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
                        WriteError(writer, broadcastError);
                        isError = true;
                        state.ErrorMessage = e.Message;
                        throw new Exception(e.StackTrace);
                    }

                    // get the attribute ids for assigned data that can be deleted (attribute is present
                    // in the data source and meta data file)
                    attributeIdsToBeUpdatedWithAssignedData = configurationAttributes.Select(_ => _.Id)
                        .Intersect(networkAttributeIds).Distinct().ToList();

                    var aggregatedResults = new List<IAggregatedResult>();

                    var totalAssets = (double)maintainableAssets.Count;
                    var i = 0.0;

                    state.Status = "Aggregating";
                    // WjTodo -- uncomment this and get it not to throw     _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
                    // loop over maintainable assets and remove assigned data that has an attribute id
                    // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                    // that was created
                    foreach (var maintainableAsset in maintainableAssets)
                    {
                        if (i % 500 == 0)
                        {
                            state.Percentage = Math.Round(i / totalAssets * 100, 1);
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
                            WriteError(writer, broadcastError);
                            throw;
                        }

                        state.Status = "Saving";
                        // WjTodo -- uncomment this and get it not to throw        _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);

                        try
                        {
                            _unitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets, allAttributes);
                        }
                        catch (Exception e)
                        {
                            var broadcastError = $"Error while filling Assigned Data -  {e.Message}";
                            WriteError(writer, broadcastError);
                            isError = true;
                            state.ErrorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }
                        try
                        {
                            _unitOfWork.MaintainableAssetRepo.UpdateMaintainableAssetsSpatialWeighting(maintainableAssets);
                        }
                        catch (Exception e)
                        {
                            var broadcastError = $"Error while Updating MaintainableAssets SpatialWeighting -  {e.Message}";
                            WriteError(writer, broadcastError);
                            isError = true;
                            state.ErrorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }

                        try
                        {
                            _unitOfWork.AggregatedResultRepo.AddAggregatedResults(aggregatedResults);
                        }
                        catch (Exception e)
                        {
                            var broadcastError = $"Error while adding Aggregated results -  {e.Message}";
                            WriteError(writer, broadcastError);
                            isError = true;
                            state.ErrorMessage = e.Message;
                            throw new Exception(e.StackTrace);
                        }


                    }
                    if (!isError)
                    {
                        state.Status = "Aggregated all network data";
                        // WjTodo -- uncomment this and get it not to throw        _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
                        _unitOfWork.Commit();

                        WriteState(writer, state);
                    }
                    else
                    {
                        state.Status = $"Error in data aggregation {state.ErrorMessage}";
                        _unitOfWork.Rollback();
                        state.Percentage = 0;
                        WriteState(writer, state);
                    }
                }
                catch
                {
                    _unitOfWork.Rollback();
                    throw;
                }

            });

            return !isError;
        }

        private static void WriteError(Writer writer, string error)
        {
            var status = new AggregationStatusMemo
            {
                ErrorMessage = error,
            };
            writer.TryWrite(status);
        }

        private static void WriteState(Writer writer, AggregationState state)
        {
            var dto = new NetworkRollupDetailDTO
            {
                NetworkId = state.NetworkId,
                Status = state.Status,

            };
            var memo = new AggregationStatusMemo
            {
                ErrorMessage = null,
                Percentage = state.Percentage,
                rollupDetailDto = dto,
            };
            writer.TryWrite(memo);

        }
    }
}
