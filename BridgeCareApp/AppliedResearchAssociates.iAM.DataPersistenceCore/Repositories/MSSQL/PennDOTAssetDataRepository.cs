﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class PennDOTAssetDataRepository : IAssetData
    {
        UnitOfWork.UnitOfDataPersistenceWork _unitofwork;

        public PennDOTAssetDataRepository(UnitOfWork.UnitOfDataPersistenceWork uow)
        {
            _unitofwork = uow;
            var network = _unitofwork.NetworkRepo.GetPennDotNetwork();
            var sectionList = network.Facilities.SelectMany(_ => _.Sections);

            KeyProperties = new Dictionary<string, List<KeySegmentDatum>>();
            foreach (var key in DataPersistenceConstants.PennDOTKeyFields.Keys)
            {
                var keyAttribute = _unitofwork.Context.Attribute.FirstOrDefault(_ => _.Name == key);
                var keyValues = new List<KeySegmentDatum>();
                if (keyAttribute == null)
                {
                    throw new ArgumentException();
                }

                var sectionsWithNumericKeys = sectionList
                    .Where(_ => _.NumericAttributeValueHistories.Any(_ => _.AttributeId == keyAttribute.Id));
                IEnumerable<SectionEntity> sectionsWithTextKeys = new List<SectionEntity>();
                if (sectionsWithNumericKeys.Count() < 1)
                {
                    // Only look in the text values if the numeric values are missing
                    sectionsWithTextKeys = sectionList
                        .Where(_ => _.TextAttributeValueHistories.Any(_ => _.AttributeId == keyAttribute.Id));

                    foreach (var section in sectionsWithTextKeys)
                    {
                        var lastValue = section.TextAttributeValueHistories
                            .Where(_ => _.AttributeId == keyAttribute.Id)
                            .OrderByDescending(_ => _.Year)
                            .First()
                            .Value;

                        keyValues.Add(new KeySegmentDatum()
                        {
                            SegmentId = section.Id,
                            KeyValue = new SegmentAttributeDatum(key, lastValue)
                        });
                    }
                }
                else
                {
                    // Use the numeric keys
                    foreach (var section in sectionsWithNumericKeys)
                    {
                        var lastValue = section.NumericAttributeValueHistories
                            .Where(_ => _.AttributeId == keyAttribute.Id)
                            .OrderByDescending(_ => _.Year)
                            .First()
                            .Value;

                        keyValues.Add(new KeySegmentDatum()
                        {
                            SegmentId = section.Id,
                            KeyValue = new SegmentAttributeDatum(key, lastValue.ToString())
                        });
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
            var segment = _unitofwork.Context.Section.FirstOrDefault(_ => _.Id == targetSegment.SegmentId);
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

            return returnValueList;
        }
    }
}
