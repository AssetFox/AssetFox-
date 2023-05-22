using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class MaintainableAssetDataRepository : IAssetData
    {
        UnitOfDataPersistenceWork _unitOfWork;

        public MaintainableAssetDataRepository(UnitOfDataPersistenceWork uow)
        {
            _unitOfWork = uow;
            var network = _unitOfWork.NetworkRepo.GetMainNetwork();
            var keyDatumFieldNames = _unitOfWork.AdminDataRepo.GetKeyFields();
            var keyDatumFields = _unitOfWork.Context.Attribute
                .Where(_ => keyDatumFieldNames.Contains(_.Name))
                .Select(_ => new {_.Id, _.Name, Type = _.DataType})
                .ToList();
            var keyDatumFieldsNetwork = _unitOfWork.Context.Attribute
                .Where(_ => _.Id == network.KeyAttributeId )
                .Select(_ => new { _.Id, _.Name, Type = _.DataType })
                .ToList();
            foreach(var keyDatumField in keyDatumFieldsNetwork)
            {
                if (!keyDatumFields.Contains(keyDatumField))
                {
                    keyDatumFields.Add(keyDatumField);
                }
            }
            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            foreach (var attribute in keyDatumFields)
            {
                var keyFieldValue = new List<KeySegmentDatum>();
                var filteredAggregatedData = _unitOfWork.Context.AggregatedResult
                    .Include(_ => _.MaintainableAsset)
                    .Where(_ => _.MaintainableAsset.NetworkId == network.Id && _.AttributeId == attribute.Id);
                foreach (var datum in filteredAggregatedData)
                {
                    var dataValue = attribute.Type == "NUMBER" ? datum.NumericValue.ToString() : datum.TextValue;
                    keyFieldValue.Add(new KeySegmentDatum { AssetId = datum.MaintainableAssetId, KeyValue = new SegmentAttributeDatum(attribute.Name, dataValue) });
                }
                KeyProperties.Add(attribute.Name, keyFieldValue);
            }
        }

        public Dictionary<string, List<KeySegmentDatum>> KeyProperties { get; private set; }

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
    }
}
