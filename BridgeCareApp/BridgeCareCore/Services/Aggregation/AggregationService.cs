using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Helpers;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
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

        /// <summary>AggregationState can be just new AggregationState() object. Purpose is to allow calling class to access the state.</summary>
        public async Task<bool> AggregateNetworkData(Writer writer, Guid networkId, AggregationState state, List<AttributeDTO> attributes, CancellationToken? cancellationToken = null)
        {
            state.NetworkId = networkId;
            var isError = false;
            var isUnmatchedDatum = false;
            state.ErrorMessage = "";

            await Task.Run(() =>
            {
                try
                {
                    _unitOfWork.BeginTransaction();

                    var maintainableAssets = new List<MaintainableAsset>();
                    var attributeData = new List<IAttributeDatum>();
                    var attributeIdsToBeUpdatedWithAssignedData = new List<Guid>();
                    if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    {
                        _unitOfWork.Rollback();
                        return;
                    }
                    state.Status = "Preparing";
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);  // DbUpdateException here -- "The wait operation timed out."

                    // Get/create configurable attributes
                    var configurationAttributes = AttributeDtoDomainMapper.ToDomainList(attributes, _unitOfWork.EncryptionKey);

                    var checkForDuplicateIDs = configurationAttributes.Select(_ => _.Id).ToList();

                    if (checkForDuplicateIDs.Count != checkForDuplicateIDs.Distinct().ToList().Count)
                    {
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error : Duplicate attribute ids for {networkName}.";
                        WriteError(writer, broadcastError);
                        throw new InvalidOperationException();
                    }

                    var checkForDuplicateNames = configurationAttributes.Select(_ => _.Name).ToList();
                    if (checkForDuplicateNames.Count != checkForDuplicateNames.Distinct().ToList().Count)
                    {
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error : Duplicate attribute names for {networkName}";
                        WriteError(writer, broadcastError);
                        throw new InvalidOperationException();
                    }

                    // get all maintainable assets in the network with their assigned data (if any) and locations
                    maintainableAssets = _unitOfWork.MaintainableAssetRepo
                        .GetAllInNetworkWithAssignedDataAndLocations(networkId)
                        .ToList();

                    // Create list of attribute ids we are allowed to update with assigned data.
                    // Could hack it in, but what is the natural way to set it up?
                    var networkAttributeIds = maintainableAssets
                        .Where(_ => _.AssignedData != null && _.AssignedData.Any())
                        .SelectMany(_ => _.AssignedData.Select(__ => __.Attribute.Id).Distinct()).ToList();

                    // In practice, every attribute's data is pulled from the same data source.
                    // Changed to getting all the data sources and loading their sheets into memory, instead of possbily loading and deserializing
                    // the same sheet 100+ times.

                    int iter = 0;
                    try
                    {
                        // Get all dataSources first
                        var dataConnections = new Dictionary<BaseDataSourceDTO, AttributeConnection>();
                        foreach (var attribute in configurationAttributes)
                        {
                            var dataSource = attributes.FirstOrDefault(_ => _.Id == attribute.Id)?.DataSource;
                            if (dataSource == null || dataConnections.Keys.Any(existingDataSource => existingDataSource.Id == dataSource.Id))
                            {
                                continue;
                            }
                            else
                            {
                                dataConnections.Add(dataSource, AttributeConnectionBuilder.Build(attribute, dataSource, _unitOfWork));
                            }
                        }
                        // With all dataSources having been fetched, get the data to populate the larger AttributeData list
                        foreach (var attribute in configurationAttributes)
                        {
                            iter++;
                            if (attribute.ConnectionType != ConnectionType.NONE)
                            {
                                var dataSource = attributes.FirstOrDefault(_ => _.Id == attribute.Id)?.DataSource;
                                var connection = dataConnections.FirstOrDefault(kvp => kvp.Key.Id == dataSource.Id).Value;
                                if (connection != null)
                                {
                                    var specificData = AttributeDataBuilder.GetData(connection);
                                    attributeData.AddRange(specificData);
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error: Fetching data for the attributes for {networkName}::{e.Message}";
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

                    var directory = Directory.GetCurrentDirectory();
                    var path = Path.Combine(directory, "Logs");
                    // Set up the log
                    //var stringBuilder = new StringBuilder();
                    //stringBuilder.AppendLine("Datum Name, Location Id, Datum Id");
                    //StreamWriter streamWriter = new StreamWriter(path + "\\UnmatchedDatum.txt");
                    if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    {
                        _unitOfWork.Rollback();
                        return;
                    }
                    state.Status = "Aggregating";
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
                    // loop over maintainable assets and remove assigned data that has an attribute id
                    // in attributeIdsToBeUpdatedWithAssignedData then assign the new attribute data
                    // that was created
                    foreach (var maintainableAsset in maintainableAssets)
                    {
                        if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                        {
                            _unitOfWork.Rollback();
                            return;
                        }
                        if (i % 500 == 0)
                        {
                            state.Percentage = Math.Round(i / totalAssets * 100, 1);
                        }
                        i++;
                        maintainableAsset.AssignedData.RemoveAll(_ =>
                            attributeIdsToBeUpdatedWithAssignedData.Contains(_.Attribute.Id));
                        // Iterates through entire attributeData list
                        maintainableAsset.AssignAttributeData(attributeData);
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
                            var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                            var broadcastError = $"Error: Creating aggregation rule(s) for the attributes for {networkName}:: {e.Message}";
                            WriteError(writer, broadcastError);
                            throw;
                        }
                    }
                    if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                    {
                        _unitOfWork.Rollback();
                        return;
                    }
                    state.Status = "Saving";
                    _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);

                    //streamWriter.Close();

                    try
                    {
                        _unitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets, attributes);
                    }
                    catch (Exception e)
                    {
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error while filling Assigned Data for {networkName} -  {e.Message}";
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
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error while Updating MaintainableAssets SpatialWeighting for {networkName} -  {e.Message}";
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
                        var networkName = _unitOfWork.NetworkRepo.GetNetworkNameOrId(networkId);
                        var broadcastError = $"Error while adding Aggregated results for {networkName} -  {e.Message}";
                        WriteError(writer, broadcastError);
                        isError = true;
                        state.ErrorMessage = e.Message;
                        throw new Exception(e.StackTrace);
                    }
                    if (!isError)
                    {
                        state.Status = "Aggregated all network data";
                        _unitOfWork.NetworkRepo.UpsertNetworkRollupDetail(networkId, state.Status);
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
            if (isUnmatchedDatum)
            {
                WriteError(writer, "Unmatched Datum locations found::See unmatchedDatum.txt log file for more details.");
            }
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
