using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PennDOTMaintainableAssetDataRepository : IAssetData
    {
        UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public PennDOTMaintainableAssetDataRepository(UnitOfWork.UnitOfDataPersistenceWork uow)
        {
            _unitOfWork = uow;
            var network = _unitOfWork.NetworkRepo.GetPennDotNetwork();
            var assets = _unitOfWork.Context.MaintainableAsset.Where(_ => _.NetworkId == network.Id);
            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            var brkeyDatum = new List<KeySegmentDatum>();
            var bmsidDatum = new List<KeySegmentDatum>();

            foreach (var asset in assets)
            {
                brkeyDatum.Add(new KeySegmentDatum()
                {
                    SegmentId = asset.Id,
                    KeyValue = new SegmentAttributeDatum("BRKEY", asset.FacilityName)
                });
                bmsidDatum.Add(new KeySegmentDatum()
                {
                    SegmentId = asset.Id,
                    KeyValue = new SegmentAttributeDatum("BMSID", asset.SectionName)
                });
            }

            KeyProperties.Add("BRKEY", brkeyDatum);
            KeyProperties.Add("BMSID", bmsidDatum);
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
            var targetSegment = lookupSource.FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetSegment == null) return new List<SegmentAttributeDatum>();
            var segment = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.Id == targetSegment.SegmentId)
                .Include(_ => _.AggregatedResults)
                .ThenInclude(_ => _.Attribute)
                .FirstOrDefault();
            if (segment == null) return new List<SegmentAttributeDatum>();
            var attributeIdList = segment.AggregatedResults.Select(_ => _.AttributeId).Distinct();

            // Populate the return value list
            var returnValueList = new List<SegmentAttributeDatum>();

            foreach (var attributeId in attributeIdList)
            {
                // Get the entry with the most recent value
                var maxEntry = segment.AggregatedResults
                    .Where(_ => _.AttributeId == attributeId)
                    .OrderByDescending(_ => _.Year)
                    .First();
                string attributeValue = (maxEntry.Discriminator[0] == 'N') ? maxEntry.NumericValue.ToString() : maxEntry.TextValue;
                returnValueList.Add(new SegmentAttributeDatum(maxEntry.Attribute.Name, attributeValue));
            }
            returnValueList.Add(new SegmentAttributeDatum("BRKEY", segment.FacilityName));
            returnValueList.Add(new SegmentAttributeDatum("BMSID", segment.SectionName));

            return returnValueList;
        }
        public Dictionary<int, SegmentAttributeDatum> GetAttributeValueHistory(string keyName, string keyValue, string attribute)
        {
            // Check for the existence of the given key
            if (!KeyProperties.ContainsKey(keyName))
            {
                throw new ArgumentException($"{keyName} not a key attribute in PennDOT network");
            }
            var targetSegment = KeyProperties[keyName].FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetSegment == null) return new Dictionary<int, SegmentAttributeDatum>();

            // Get the sought attribute id
            var attributeInfo = _unitOfWork.Context.Attribute.FirstOrDefault(_ => _.Name == attribute);
            if (attributeInfo == null)
            {
                throw new ArgumentException($"{attribute} was not found");
            }

            var result = new Dictionary<int, SegmentAttributeDatum>();
            if (attribute == "BRKEY")
            {
                result.Add(0, new SegmentAttributeDatum("BRKEY", _unitOfWork.Context.MaintainableAsset.First(_ => _.Id == targetSegment.SegmentId).FacilityName));
            }
            else if (attribute == "BMSID")
            {
                result.Add(0, new SegmentAttributeDatum("BMSID", _unitOfWork.Context.MaintainableAsset.First(_ => _.Id == targetSegment.SegmentId).SectionName));
            }
            else
            {
                var attributeValues = _unitOfWork.Context.AggregatedResult
                    .Where(_ => _.MaintainableAssetId == targetSegment.SegmentId && _.AttributeId == attributeInfo.Id);

                foreach (var value in attributeValues)
                {
                    string yearValue = (value.Discriminator[0] == 'N') ? value.NumericValue.ToString() : value.TextValue;
                    result.Add(value.Year, new SegmentAttributeDatum(attributeInfo.Name, yearValue));
                }
            }

            return result;
        }
    }
}
