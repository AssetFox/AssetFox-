using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Helpers;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.Serializers;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;

namespace BridgeCareCore.Services
{
    public class AttributeImportService : IAttributeImportService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string NoColumnFoundForId = "No column found for Id";
        public const string NoAttributeWasFoundWithName = "no attribute was found with name";
        public const string WasFoundInRow = "was found in row";
        public const string NonemptyKeyIsRequired = "A non-empty key column name is required.";
        public const string InspectionDateColumn = "InspectionDate column";
        public const string FailedToCreateAValidAttributeDatum = "Failed to create a valid AttributeDatum";
        public const string NumberIsOutOfValidRange = "is out of the valid range";
        public const string IsNotLessThanOrEqualToTheMaximumValue = "is not less than or equal to the maximum value";
        public const string IsNotGreaterThanOrEqualToTheMinimumValue = "is not greater than or equal to the minimum value";

        public AttributeImportService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public AttributesImportResultDTO ImportExcelAttributes(
            string keyColumnName,
            string inspectionDateColumnName,
            string spatialWeightingValue,
            ExcelRawDataSpreadsheet worksheet)
        {
            if (string.IsNullOrWhiteSpace(keyColumnName))
            {
                return new AttributesImportResultDTO
                {
                    WarningMessage = NonemptyKeyIsRequired,
                };
            }
            var rowIndex = 1;
            var endColumn = worksheet.Columns.Count;
            var endRow = worksheet.Columns.Max(c => c.Entries.Count);
            var columnNameDictionary = new Dictionary<int, string>();
            var columnNameList = new List<string>();
            var keyColumnIndex = FindKeyColumnIndex(keyColumnName, worksheet, rowIndex, endColumn, columnNameDictionary, columnNameList);
            if (keyColumnIndex == -1)
            {
                var warningMessage = BuildKeyNotFoundWarningMessage(keyColumnName, columnNameDictionary);
                return new AttributesImportResultDTO
                {
                    WarningMessage = warningMessage,
                };
            }
            Func<KeyValuePair<int, string>, bool> dateTimeColumnPredicate =
                kvp => kvp.Value.Equals(inspectionDateColumnName, StringComparison.OrdinalIgnoreCase);
            var inspectionDateColumnPair = columnNameDictionary.FirstOrDefault(dateTimeColumnPredicate);
            var inspectionDateColumnIndex = inspectionDateColumnPair.Key;
            if (inspectionDateColumnIndex <= 0)
            {
                var allColumnNames = string.Join(", ", columnNameList);
                var warningMessage = $"{InspectionDateColumn} {inspectionDateColumnName} not found. The column names we did find were {allColumnNames}";
                return new AttributesImportResultDTO
                {
                    WarningMessage = warningMessage,
                };
            }
            var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();
            var allAttributeNames = allAttributes.Select(a => a.Name).OrderBy(s => s).ToList();
            var allAttributeNamesString = string.Join(", ", allAttributeNames);
            var columnIndexAttributeDictionary = new Dictionary<int, AttributeDTO>();
            foreach (var columnIndex in columnNameDictionary.Keys)
            {
                if (columnIndex != keyColumnIndex && columnIndex != inspectionDateColumnIndex)
                {
                    var attributeName = columnNameDictionary[columnIndex];
                    var attribute = allAttributes.FirstOrDefault(a => a.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
                    if (attribute == null)
                    {
                        var warningMessage = $"The title of column {columnIndex} is {attributeName}. However, {NoAttributeWasFoundWithName} {attributeName}. The following is a list of all attribute names: {allAttributeNamesString}";
                        return new AttributesImportResultDTO
                        {
                            WarningMessage = warningMessage,
                        };
                    }
                    columnIndexAttributeDictionary[columnIndex] = attribute;
                }
            }
            var networkId = Guid.NewGuid();

            var maintainableAssets = new List<MaintainableAsset>();
            var foundAssetNames = new Dictionary<string, int>();
            for (var assetRowIndex = 2; assetRowIndex <= endRow; assetRowIndex++)
            {
                var assetName = GetCellValueOrNull(worksheet, keyColumnIndex, assetRowIndex)?.ToString();
                if (!string.IsNullOrWhiteSpace(assetName))
                {
                    var lowercaseAssetName = assetName.ToLowerInvariant();
                    if (foundAssetNames.ContainsKey(lowercaseAssetName))
                    {
                        var warningMessage = $"The asset name {assetName} {WasFoundInRow} {foundAssetNames[lowercaseAssetName]} and row {assetRowIndex}. This is not allowed.";
                        return new AttributesImportResultDTO
                        {
                            WarningMessage = warningMessage,
                        };
                    }
                    foundAssetNames[lowercaseAssetName] = assetRowIndex;
                    var location = new SectionLocation(Guid.NewGuid(), assetName);
                    var maintainableAssetId = Guid.NewGuid();
                    var newAsset = new MaintainableAsset(maintainableAssetId, networkId, location, spatialWeightingValue); 
                    var inspectionDateObject = GetCellValueOrNull(worksheet, assetRowIndex, inspectionDateColumnIndex);
                    DateTime inspectionDate = DateTime.MinValue;
                    if (inspectionDateObject is DateTime inspectionDateObjectDate)
                    {
                        inspectionDate = inspectionDateObjectDate;
                    }
                    var attributeDataForAsset = new List<IAttributeDatum>();
                    foreach (var attributeColumnIndex in columnIndexAttributeDictionary.Keys)
                    {
                        var attribute = columnIndexAttributeDictionary[attributeColumnIndex];
                        var attributeValue = GetCellValueOrNull(worksheet, attributeColumnIndex, assetRowIndex);
                        IAttributeDatum attributeDatum = null;
                        try
                        {
                            attributeDatum = CreateAttributeDatum(attribute, attributeValue, maintainableAssetId, location, inspectionDate);
                        }
                        catch
                        {
                            var warningMessage = $@"{FailedToCreateAValidAttributeDatum} at row {assetRowIndex} column {attributeColumnIndex}. The spreadsheet value was ""{attributeValue}.""";
                            return new AttributesImportResultDTO
                            {
                                WarningMessage = warningMessage,
                            };
                        }
                        if (attribute.Type == DataPersistenceConstants.AttributeNumericDataType)
                        {
                            if (attributeDatum is AttributeDatum<double> doubleAttributeDatum)
                            {
                                var min = attribute.Minimum;
                                if (min != null)
                                {
                                    var minValue = min.Value;
                                    if (!(minValue <= doubleAttributeDatum.Value))
                                    {
                                        var warningMessage = $"Value for attribute {attribute.Name} at row {assetRowIndex}, colum {attributeColumnIndex} is {doubleAttributeDatum.Value}. This {IsNotGreaterThanOrEqualToTheMinimumValue} {attribute.Minimum}.";
                                        return new AttributesImportResultDTO
                                        {
                                            WarningMessage = warningMessage,
                                        };
                                    }
                                }
                                var max = attribute.Maximum;
                                if (max != null)
                                {
                                    var maxValue = max.Value;
                                    if (!(maxValue >= doubleAttributeDatum.Value))
                                    {
                                        var warningMessage = $"Value for attribute {attribute.Name} at row {assetRowIndex}, colum {attributeColumnIndex} is {doubleAttributeDatum.Value}. This {IsNotLessThanOrEqualToTheMaximumValue} {attribute.Maximum}.";
                                        return new AttributesImportResultDTO
                                        {
                                            WarningMessage = warningMessage,
                                        };
                                    }
                                }
                            }
                        }
                        attributeDataForAsset.Add(attributeDatum);
                    }
                    newAsset.AssignAttributeData(attributeDataForAsset, maintainableAssetId);
                    maintainableAssets.Add(newAsset);
                }
            }
            var networkName = "Created by import";
            var newNetwork = new Network(maintainableAssets, networkId, networkName);
            _unitOfWork.NetworkRepo.CreateNetwork(newNetwork);
            _unitOfWork.AttributeDatumRepo.AddAssignedData(maintainableAssets, allAttributes);
            return new AttributesImportResultDTO
            {
                NetworkId = newNetwork.Id,
            };
        }

        private IAttributeDatum CreateAttributeDatum(AttributeDTO attribute, object attributeValue, Guid maintainableAssetId, Location location, DateTime inspectionDate)
        {
            var domainAttribute = AttributeMapper.ToDomain(attribute, _unitOfWork.EncryptionKey);
            var attributeId = Guid.NewGuid();
            var attributeType = domainAttribute.DataType;
            IAttributeDatum returnValue = null;
            switch (attributeType)
            {
            case DataPersistenceConstants.AttributeTextDataType:
                returnValue = new AttributeDatum<string>(attributeId, domainAttribute, attributeValue.ToString(), location, inspectionDate);
                break;
            case DataPersistenceConstants.AttributeNumericDataType:
                double? nullableDoubleValue = null;
                if (attributeValue == null || attributeValue is string attributeValueString && string.IsNullOrWhiteSpace(attributeValueString))
                {
                    var parseDefaultValue = DoubleParseHelper.TryParseNullableDouble(attribute.DefaultValue);
                    if (parseDefaultValue != null)
                    {
                        nullableDoubleValue = parseDefaultValue;
                    }
                }
                else
                {
                    nullableDoubleValue = DoubleParseHelper.TryParseNullableDouble(attributeValue);
                }
                if (nullableDoubleValue.HasValue)
                {
                    returnValue = new AttributeDatum<double>(attributeId, domainAttribute, nullableDoubleValue.Value, location, inspectionDate);
                }
                break;
            }
            return returnValue;
        }

        private static object GetCellValueOrNull(ExcelRawDataSpreadsheet worksheet, int oneBasedColumnIndex, int oneBasedRowIndex)
        {
            if (oneBasedColumnIndex > worksheet.Columns.Count || oneBasedColumnIndex < 1)
            {
                return null;
            }
            var column = worksheet.Columns[oneBasedColumnIndex - 1];
            if (oneBasedRowIndex > column.Entries.Count || oneBasedRowIndex < 1)
            {
                return null;
            }
            var entry = column.Entries[oneBasedRowIndex - 1];
            var returnValue = entry.Accept(new ExcelCellDatumValueGetter(), Unit.Default);
            return returnValue;
        }

        private static int FindKeyColumnIndex(string keyColumnName, ExcelRawDataSpreadsheet worksheet, int rowIndex, int rowLength, Dictionary<int, string> columnNameDictionary, List<string> columnNameList)
        {
            var keyColumnIndex = -1;
            for (var columnIndex = 1; columnIndex <= rowLength; columnIndex++)
            {
                var cellValue = GetCellValueOrNull(worksheet, columnIndex, 1)?.ToString();
                if (cellValue == null)
                {
                    break;
                }
                columnNameDictionary[columnIndex] = cellValue;
                columnNameList.Add(cellValue);
                if (keyColumnName.Equals(cellValue, StringComparison.OrdinalIgnoreCase))
                {
                    keyColumnIndex = columnIndex;
                }
            }

            return keyColumnIndex;
        }

        private static string BuildKeyNotFoundWarningMessage(string keyColumnName, Dictionary<int, string> columnNameDictionary)
        {
            string warningMessage;
            if (columnNameDictionary.Keys.Any())
            {
                var warningMessageBuilder = new StringBuilder();
                warningMessageBuilder.AppendLine($"{NoColumnFoundForId} {keyColumnName}");
                var maxColumnKey = columnNameDictionary.Keys.Max();
                warningMessageBuilder.Append($"The column headers we did find were ");
                for (var i = 1; i <= maxColumnKey; i++)
                {
                    if (columnNameDictionary.ContainsKey(i))
                    {
                        if (i == maxColumnKey)
                        {
                            warningMessageBuilder.Append("and ");
                        }
                        warningMessageBuilder.Append(columnNameDictionary[i]);
                        if (i < maxColumnKey)
                        {
                            warningMessageBuilder.Append($", ");
                        }
                        else
                        {
                            warningMessageBuilder.Append('.');
                        }
                    }
                }
                warningMessage = warningMessageBuilder.ToString();
            }
            else
            {
                warningMessage = $"None of the columns had headers. The headers are expected to be in the top row of the spreadsheet.";
            }

            return warningMessage;
        }
    }
}
