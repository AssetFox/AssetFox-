using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    /// <summary>
    /// This is the data repository for pulling data from the Facility-Section-AttributeValueHistory database schema
    /// </summary>
    /// <remarks>
    /// Not compatiable with databases running the latest schema!  Kept for reference.
    /// </remarks>
    public class PennDOTAssetDataRepository : IAssetData
    {
        UnitOfWork.UnitOfDataPersistenceWork _unitofwork;

        public PennDOTAssetDataRepository(UnitOfWork.UnitOfDataPersistenceWork uow)
        {
            _unitofwork = uow;
            var network = _unitofwork.NetworkRepo.GetPennDotNetwork();
            var facilities = _unitofwork.Context.Facility.Where(_ => _.NetworkId == network.Id).Include(_ => _.Sections);
            //network.Facilities = facilities;


            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            foreach (var key in DataPersistenceConstants.PennDOTKeyFields.Keys)
            {
                var keyValues = new List<KeySegmentDatum>();

                // The special case of BMSID (which is contained in the segment name) has to be handled
                if (key == "BMSID")
                {
                    // This comes from section.Name, not a specific attribute.
                    var sectionList = facilities.SelectMany(_ => _.Sections);
                    foreach (var section in sectionList)    
                    {
                        keyValues.Add(new KeySegmentDatum()
                        {
                            SegmentId = section.Id,
                            KeyValue = new SegmentAttributeDatum(key, section.Name)
                        });
                    }
                }
                else
                {
                    var keyAttribute = _unitofwork.Context.Attribute.FirstOrDefault(_ => _.Name == key);
                    if (keyAttribute == null)
                    {
                        throw new ArgumentException();
                    }

                    // EF Core 3 requires this to be a two step process
                    var sectionsWithKeyAttribute = _unitofwork.Context.NumericAttributeValueHistory.Where(_ => _.AttributeId == keyAttribute.Id).ToList();

                    var sectionsWithNumericKeys = sectionsWithKeyAttribute.GroupBy(_ => _.SectionId);

                    //IEnumerable<SectionEntity> sectionsWithTextKeys = new List<SectionEntity>();
                    if (sectionsWithNumericKeys.Count() < 1)
                    {
                        // Only look in the text values if the numeric values are missing
                        var sectionsWithKeyTextAttributes = _unitofwork.Context.TextAttributeValueHistory.Where(_ => _.AttributeId == keyAttribute.Id).ToList();

                        var sectionsWithTextKeys = sectionsWithKeyTextAttributes.GroupBy(_ => _.SectionId);

                        foreach (var group in sectionsWithTextKeys)
                        {
                            var lastValue = group
                                .OrderByDescending(_ => _.Year)
                                .First()
                                .Value;

                            keyValues.Add(new KeySegmentDatum()
                            {
                                SegmentId = group.Key,
                                KeyValue = new SegmentAttributeDatum(key, lastValue)
                            });
                        }
                    }
                    else
                    {
                        // Use the numeric keys
                        foreach (var group in sectionsWithNumericKeys)
                        {
                            var lastValue = group
                                .OrderByDescending(_ => _.Year)
                                .First()
                                .Value;

                            keyValues.Add(new KeySegmentDatum()
                            {
                                SegmentId = group.Key,
                                KeyValue = new SegmentAttributeDatum(key, lastValue.ToString())
                            });
                        }
                    }
                }

                KeyProperties.Add(key, keyValues);
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
            var targetSegment = lookupSource.FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetSegment == null) return new List<SegmentAttributeDatum>();
            var segment = _unitofwork.Context.Section.Where(_ => _.Id == targetSegment.SegmentId)
                .Include(_ => _.NumericAttributeValueHistories).ThenInclude(_ => _.Attribute)
                .Include(_ => _.TextAttributeValueHistories).ThenInclude(_ => _.Attribute)
                .FirstOrDefault();
            if (segment == null) return new List<SegmentAttributeDatum>();

            // Populate the return value list
            var returnValueList = new List<SegmentAttributeDatum>();

            foreach (var numericAttribute in segment.NumericAttributeValueHistories)
            {
                returnValueList.Add(new SegmentAttributeDatum(numericAttribute.Attribute.Name, numericAttribute.Value.ToString()));
            }
            foreach (var textAttribute in segment.TextAttributeValueHistories)
            {
                returnValueList.Add(new SegmentAttributeDatum(textAttribute.Attribute.Name, textAttribute.Value));
            }
            // Add BMSID
            returnValueList.Add(new SegmentAttributeDatum("BMSID", segment.Name));

            return returnValueList;
        }

        public Dictionary<int, SegmentAttributeDatum> GetAttributeValueHistory(string keyName, string keyValue, string attribute)
        {
            // Check for the existence of the given key
            if (!KeyProperties.ContainsKey(keyName))
            {
                throw new ArgumentException($"{keyName} not a key attribute in PennDOT network");
            }

            // Get the target segment info
            var lookupSource = KeyProperties[keyName];
            var targetSegment = lookupSource.FirstOrDefault(_ => _.KeyValue.Value == keyValue);
            if (targetSegment == null) return new Dictionary<int, SegmentAttributeDatum>();
            var segment = _unitofwork.Context.Section.Where(_ => _.Id == targetSegment.SegmentId)
                .Include(_ => _.NumericAttributeValueHistories).ThenInclude(_ => _.Attribute)
                .Include(_ => _.TextAttributeValueHistories).ThenInclude(_ => _.Attribute)
                .FirstOrDefault();
            if (segment == null) return new Dictionary<int, SegmentAttributeDatum>();

            // Get the target attribute information
            var keyAttribute = _unitofwork.Context.Attribute.FirstOrDefault(_ => _.Name == attribute);
            if (keyAttribute == null) return new Dictionary<int, SegmentAttributeDatum>();

            // Get all entires for the attribute-segment combination from either the NumericAttributeValueHistories or TextAttributeValueHistories
            var numAttributeValues = _unitofwork.Context.NumericAttributeValueHistory.Where(_ => _.SectionId == segment.Id && _.AttributeId == keyAttribute.Id);
            if (numAttributeValues != null)
            {
                var result = new Dictionary<int, SegmentAttributeDatum>();
                foreach (var measurment in numAttributeValues)
                {
                    result.Add(measurment.Year, new SegmentAttributeDatum(keyAttribute.Name, measurment.Value.ToString()));
                }
                return result;
            }

            var textAttributeValues = _unitofwork.Context.TextAttributeValueHistory.Where(_ => _.SectionId == segment.Id && _.AttributeId == keyAttribute.Id);
            if (textAttributeValues != null)
            {
                var result = new Dictionary<int, SegmentAttributeDatum>();
                foreach (var measurement in textAttributeValues)
                {
                    result.Add(measurement.Year, new SegmentAttributeDatum(keyAttribute.Name, measurement.Value));
                }
                return result;
            }

            return new Dictionary<int, SegmentAttributeDatum>();
        }

    }
}
