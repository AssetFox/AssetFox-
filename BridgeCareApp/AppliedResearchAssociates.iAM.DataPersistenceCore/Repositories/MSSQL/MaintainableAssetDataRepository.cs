using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetDataRepository : IAssetData
    {
        UnitOfDataPersistenceWork _unitOfWork;

        public MaintainableAssetDataRepository(UnitOfDataPersistenceWork uow)
        {
            _unitOfWork = uow;
            var reportTypeParam = uow.AdminSettingsRepo.GetInventoryReports();
            var network = _unitOfWork.NetworkRepo.GetMainNetwork();
            var keyDatumFieldNames = _unitOfWork.AdminSettingsRepo.GetKeyFields();
            var rawNetwork = _unitOfWork.NetworkRepo.GetRawNetwork();
            var rawKeyDatumFieldNames = _unitOfWork.AdminSettingsRepo.GetRawKeyFields();
            
            var keyDatumFields = _unitOfWork.Context.Attribute
                .Where(_ => keyDatumFieldNames.Contains(_.Name))
                .Select(_ => new {_.Id, _.Name, Type = _.DataType})
                .ToList();
            var keyDatumFieldsNetwork = _unitOfWork.Context.Attribute
                .Where(_ => _.Id == network.KeyAttributeId )
                .Select(_ => new { _.Id, _.Name, Type = _.DataType })
                .ToList();
            // Ensure the network's key datum field is in the list of key properties
            foreach (var keyDatumField in keyDatumFieldsNetwork)
            {
                if (!keyDatumFields.Contains(keyDatumField))
                {
                    keyDatumFields.Add(keyDatumField);
                }
            }

            // Populate key properties
            // TODO: Replace key properties with main data table
            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            foreach (var attribute in keyDatumFields)
            {
                var keyFieldValue = new List<KeySegmentDatum>();
                var filteredAggregatedKeyData = _unitOfWork.Context.AggregatedResult
                    .Include(_ => _.MaintainableAsset)
                    .Where(_ => _.MaintainableAsset.NetworkId == network.Id && _.AttributeId == attribute.Id);
                foreach (var datum in filteredAggregatedKeyData)
                {
                    var dataValue = attribute.Type == "NUMBER" ? datum.NumericValue.ToString() : datum.TextValue;
                    keyFieldValue.Add(new KeySegmentDatum { AssetId = datum.MaintainableAssetId, KeyValue = new SegmentAttributeDatum(attribute.Name, dataValue) });
                }

                if (reportTypeParam[0].Contains("(P)"))
                {
                    KeyProperties.Add(attribute.Name, keyFieldValue);
                }
            }

            // Populate main key data table
            MainNetworkKeyTable = new List<MaintainableAssetQueryDTO>();
            var keyDatumFieldIds = keyDatumFields.Select(_ => _.Id).ToList();
            var filteredAggregatedData = _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Include(_ => _.Attribute)
                .Where(_ => _.MaintainableAsset.NetworkId == network.Id && keyDatumFieldIds.Contains(_.AttributeId))
                .AsEnumerable()
                .GroupBy(_ => _.MaintainableAssetId);
            foreach (var asset in filteredAggregatedData)
            {
                var queryData = new MaintainableAssetQueryDTO() { AssetId = asset.Key };
                queryData.AssetProperties = new Dictionary<AttributeDTO, string>();
                foreach (var attribute in asset)
                {
                    var convertedAttribute = attribute.Attribute.ToDto(null);
                    var dataValue = convertedAttribute.Type == "NUMBER" ? attribute.NumericValue.ToString() : attribute.TextValue;
                    queryData.AssetProperties.Add(convertedAttribute, dataValue);
                }
                MainNetworkKeyTable.Add(queryData);
            }

            // Populate raw key data table
            RawNetworkKeyTable = new List<MaintainableAssetQueryDTO>();

            var rawKeyDatumFields = _unitOfWork.Context.Attribute
                .Where(_ => rawKeyDatumFieldNames.Contains(_.Name))
                .Select(_ => new { _.Id, _.Name, Type = _.DataType })
                .ToList();

            var rawKeyDatumFieldsNetwork = _unitOfWork.Context.Attribute
                .Where(_ => _.Id == rawNetwork.KeyAttributeId)
                .Select(_ => new { _.Id, _.Name, Type = _.DataType })
                .ToList();

            // Ensure the network's key datum field is in the list of key properties
            foreach (var rawKeyDatumField in rawKeyDatumFieldsNetwork)
            {
                if (!rawKeyDatumFields.Contains(rawKeyDatumField))
                {
                    rawKeyDatumFields.Add(rawKeyDatumField);
                }
            }
            var rawKeyDatumFieldIds = rawKeyDatumFields.Select(_ => _.Id).ToList();
            var filteredRawAggregatedData = _unitOfWork.Context.AggregatedResult
                .Include(_ => _.MaintainableAsset)
                .Include(_ => _.Attribute)
                .Where(_ => _.MaintainableAsset.NetworkId == rawNetwork.Id && rawKeyDatumFieldIds.Contains(_.AttributeId))
                .AsEnumerable()
                .GroupBy (_ => _.MaintainableAssetId);
            foreach (var asset in filteredRawAggregatedData)
            {
                var queryData = new MaintainableAssetQueryDTO() { AssetId = asset.Key };
                queryData.AssetProperties = new Dictionary<AttributeDTO, string>();
                foreach (var attribute in asset)
                {
                    var convertedAttribute = attribute.Attribute.ToDto(null);
                    var dataValue = convertedAttribute.Type == "NUMBER" ? attribute.NumericValue.ToString() : attribute.TextValue;
                    queryData.AssetProperties.Add(convertedAttribute, dataValue);
                }
                RawNetworkKeyTable.Add(queryData);
            }

            foreach (var attribute in rawKeyDatumFields)
            {
                var rawKeyFieldValue = new List<KeySegmentDatum>();
                var filteredRawAggregatedKeyData = _unitOfWork.Context.AggregatedResult
                    .Include(_ => _.MaintainableAsset)
                    .Where(_ => _.MaintainableAsset.NetworkId == rawNetwork.Id && _.AttributeId == attribute.Id);

                foreach (var datum in filteredRawAggregatedKeyData)
                {
                    var datumValue = attribute.Type == "NUMBER" ? datum.NumericValue.ToString() : datum.TextValue;
                    rawKeyFieldValue.Add(new KeySegmentDatum { AssetId = datum.MaintainableAssetId, KeyValue = new SegmentAttributeDatum(attribute.Name, datumValue)});
                }
                var raw = reportTypeParam[0].ElementAt(28);
                
               if (reportTypeParam[0].Contains("(R)"))
                {
                    KeyProperties.Add(attribute.Name, rawKeyFieldValue);
                }

            }
        }

        public Dictionary<string, List<KeySegmentDatum>> KeyProperties { get; private set; }

        public List<MaintainableAssetQueryDTO> MainNetworkKeyTable { get; private set; }

        public List<MaintainableAssetQueryDTO> RawNetworkKeyTable { get; private set; }

        public List<SegmentAttributeDatum> GetAssetAttributes(string keyName, string keyValue)
        {
            // Check for the existence of the given key
            if (!KeyProperties.ContainsKey(keyName))
            {
                throw new ArgumentException($"{keyName} not a key attribute in PennDOT network");
            }

            // Get the target segment info
            var lookupSource = KeyProperties[keyName];
            var debugMe = lookupSource.Select(_ => _.KeyValue.Value).ToList();
            var targetAsset = lookupSource.FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetAsset == null) return new List<SegmentAttributeDatum>();
            var asset = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.Id == targetAsset.AssetId)
                .Include(_ => _.AggregatedResults)
                .ThenInclude(_ => _.Attribute)
                .FirstOrDefault();
            if (asset == null) return new List<SegmentAttributeDatum>();
            var attributeIdList = asset.AggregatedResults.Select(_ => _.AttributeId).Distinct();

            // Populate the return value list
            var returnValueList = new List<SegmentAttributeDatum>();

            foreach (var attributeId in attributeIdList)
            {
                // Get the entry with the most recent value
                var maxEntry = asset.AggregatedResults
                    .Where(_ => _.AttributeId == attributeId)
                    .OrderByDescending(_ => _.Year)
                    .First();
                string attributeValue = (maxEntry.Discriminator[0] == 'N') ? maxEntry.NumericValue.ToString() : maxEntry.TextValue;
                returnValueList.Add(new SegmentAttributeDatum(maxEntry.Attribute.Name, attributeValue));
            }
            //returnValueList.Add(new SegmentAttributeDatum("BRKEY", asset.FacilityName));
            //returnValueList.Add(new SegmentAttributeDatum("BMSID", asset.SectionName));

            // Add in each key property if it does not exist
            foreach (var keyProperty in KeyProperties)
            {
                if (!returnValueList.Any(_ => _.Name == keyProperty.Key))
                {
                    // This does not exist in the set yet
                    var specificKeyValue = KeyProperties[keyProperty.Key].FirstOrDefault(_ => _.AssetId == asset.Id);
                    if (specificKeyValue != null) returnValueList.Add(new SegmentAttributeDatum(keyProperty.Key, specificKeyValue.KeyValue.TextValue));
                }
            }

            return returnValueList;
        }

        public List<List<string>> GetKeyPropertiesTable(List<string> keyFieldNames)
        {
            var network = _unitOfWork.NetworkRepo.GetMainNetwork();
            var result = new List<List<string>>();
            var keyDatumFields = _unitOfWork.Context.Attribute
                .Where(_ => keyFieldNames.Contains(_.Name))
                .Select(_ => new { _.Id, _.Name, Type = _.DataType })
                .ToList();
            var assets = _unitOfWork.Context.MaintainableAsset.Where(_ => _.NetworkId == network.Id).ToList();
            var aggregatedData = _unitOfWork.Context.AggregatedResult
                    .Include(_ => _.MaintainableAsset)
                    .Include(_ => _.Attribute)
                    .Where(_ => keyFieldNames.Contains(_.Attribute.Name) && _.MaintainableAsset.NetworkId == network.Id)
                    .ToList();
            foreach (var asset in assets)
            {
                var assetTuple = new List<string>();
                foreach (var attributeName in keyFieldNames)
                {
                    var attribute = keyDatumFields.Where(_ => _.Name == attributeName).FirstOrDefault();
                    if (attribute.Type == "NUMBER")
                    {
                        assetTuple.Add(aggregatedData.FirstOrDefault(_ => _.MaintainableAssetId == asset.Id && _.AttributeId == attribute.Id).NumericValue.ToString());
                    }
                    else
                    {
                        assetTuple.Add(aggregatedData.FirstOrDefault(_ => _.MaintainableAssetId == asset.Id && _.AttributeId == attribute.Id).TextValue);
                    }
                }
                result.Add(assetTuple);
            }

            return result;
        }

        public Dictionary<int, SegmentAttributeDatum> GetAttributeValueHistory(string keyName, string keyValue, string attribute)
        {
            // Check for the existence of the given key
            if (!KeyProperties.ContainsKey(keyName))
            {
                throw new ArgumentException($"{keyName} not a key attribute in network");
            }
            var targetAsset = KeyProperties[keyName].FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetAsset == null) return new Dictionary<int, SegmentAttributeDatum>();

            // Get the sought attribute id
            var attributeInfo = _unitOfWork.Context.Attribute.FirstOrDefault(_ => _.Name == attribute);
            if (attributeInfo == null)
            {
                if (KeyProperties.ContainsKey(attribute)) throw new ArgumentException($"{attribute} is a key and has no history");
                throw new ArgumentException($"{attribute} was not found");
            }

            var result = new Dictionary<int, SegmentAttributeDatum>();

            var attributeValues = _unitOfWork.Context.AggregatedResult
                .Where(_ => _.MaintainableAssetId == targetAsset.AssetId && _.AttributeId == attributeInfo.Id);

            foreach (var value in attributeValues)
            {
                string yearValue = (value.Discriminator[0] == 'N') ? value.NumericValue.ToString() : value.TextValue;
                result.Add(value.Year, new SegmentAttributeDatum(attributeInfo.Name, yearValue));
            }

            return result;
        }

        public List<MaintainableAssetQueryDTO> QueryKeyAttributes(Dictionary<AttributeDTO, string> queryParameters,
            NetworkTypes networkType, List<MaintainableAssetQueryDTO> previousQuery = null)
        {
            var reportTypeParam = _unitOfWork.AdminSettingsRepo.GetInventoryReports();
            // Populate the previous query with the entire network if it does not already exist
            if (previousQuery == null)
            {
                if(networkType == NetworkTypes.Raw)
                {
                    previousQuery = RawNetworkKeyTable;
                }
                else if(networkType == NetworkTypes.Main)
                {
                    previousQuery = MainNetworkKeyTable;
                }
            }

            // Ensure that each attribute is in the previousQuery
            var listOfAttributes = previousQuery.SelectMany(_ => _.AssetProperties.Keys).Select(_ => _.Id).Distinct();
            var missingAttributes = queryParameters.Keys.Where(_ => !listOfAttributes.Contains(_.Id));
            if (missingAttributes.Count() > 0)
            {
                var message = $"Unable to find attribute {missingAttributes.First().Name} in key properties";
                if (missingAttributes.Count() > 1) message = message + $" or {missingAttributes.Count()} other attributes";
                throw new RowNotInTableException(message);
            }

            // Remove assets without any of the queried IDs
            var queryAttributeIds = queryParameters.Keys.Select(_ => _.Id);
            foreach (var attribute in queryAttributeIds)
            {
                previousQuery = previousQuery.Where(q => q.AssetProperties.Keys.Any(a => a.Id == attribute)).ToList();
            }

            // Reduce the list of maintainable IDs by the query parameters one by one
            foreach (var parameter in queryParameters)
            {
                var valueTable = new Dictionary<Guid, string>();
                foreach (var asset in previousQuery)
                {
                    try
                    {
                        var value = asset.AssetProperties.SingleOrDefault(_ => _.Key.Id == parameter.Key.Id).Value;
                        valueTable.Add(asset.AssetId, value);
                    }
                    catch
                    {
                        throw new RowNotInTableException($"Found multiple values for attribute {parameter.Key.Name} in attribute {asset.AssetId.ToString()}");
                    }
                }
                var newIdList = valueTable.Where(_ => _.Value == parameter.Value).Select(_ => _.Key).ToList();
                previousQuery = previousQuery.Where(_ => newIdList.Contains(_.AssetId)).ToList();
            }

            return previousQuery;
        }
    }
}
