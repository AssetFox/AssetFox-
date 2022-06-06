﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    /*
Every record/row is a bridge
Every column/field is a specific attribute (ID, Deck Area, Structure Type, Rating, etc.)
The user will provide the name of the ID field as part of the import (in our case, BRKEY)
You can attempt to match the attribute names in the first row of the excel sheet to the attribute names in the Attribute table.
>> If there are repeats in the column names, reject the upload
>> If the user specified key column is null, reject the upload with a message telling the user which row has a null key field
>> If the key field values are not unique, reject the upload with a message telling the user which rows have duplicates
>> If you find a unique match for each column, great!  start importing data
>> If there are column names that do not have a matching Attribute.Name in the Attributes table, reject the upload and let the user know what fields need to be added
The network is defined by these attributes - in other words, you cannot have a mismatch between assets and attributes
Do not try and create attributes here - juset verify they exist.  We can tackle making the attributes later given our timeframe
    */
    public class AttributeImportService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string NoColumnFoundForId = "No column found for Id";
        public const string NoAttributeWasFoundWithName = "no attribute was found with name";
        public const string WasFoundInRow = "was found in row";
        public const string NonemptyKeyIsRequired = "A non-empty key column name is required.";

        public AttributeImportService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public AttributesImportResultDTO ImportExcelAttributes(string keyColumnName, ExcelPackage excelPackage)
        {
            if (string.IsNullOrWhiteSpace(keyColumnName))
            {
                return new AttributesImportResultDTO
                {
                    WarningMessage = NonemptyKeyIsRequired,
                };
            }
            var workbook = excelPackage.Workbook;
            var worksheet = workbook.Worksheets[0];
            var cells = worksheet.Cells;
            var rowIndex = 1;
            var end = cells.End;
            var endColumn = end.Column;
            var endRow = end.Row;
            var rowLength = cells.End.Column;
            var columnNameDictionary = new Dictionary<int, string>();
            var columnNameList = new List<string>();
            var keyColumnIndex = FindKeyColumnIndex(keyColumnName, cells, rowIndex, rowLength, columnNameDictionary, columnNameList);
            if (keyColumnIndex == -1)
            {
                var warningMessage = BuildKeyNotFoundWarningMessage(keyColumnName, columnNameDictionary);
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
                if (columnIndex != keyColumnIndex)
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
                var assetName = worksheet.Cells[assetRowIndex, 1]?.Value.ToString();
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
                if (!string.IsNullOrWhiteSpace(assetName)) {
                    var newAsset = new MaintainableAsset(Guid.NewGuid(), networkId, null, "spatialWeighting");
                    maintainableAssets.Add(newAsset);
                }
            }
            var networkName = "Created by import";
            var newNetwork = new Network(maintainableAssets, networkId, networkName);
            _unitOfWork.NetworkRepo.CreateNetwork(newNetwork);
            // If we have reached this point, we should add a new Network to our database based on the spreadsheet values
            throw new NotImplementedException();
        }

        private static int FindKeyColumnIndex(string keyColumnName, ExcelRange cells, int rowIndex, int rowLength, Dictionary<int, string> columnNameDictionary, List<string> columnNameList)
        {
            var keyColumnIndex = -1;
            for (var columnIndex = 2; columnIndex <= rowLength; columnIndex++)
            {
                var cellValue = cells[rowIndex, columnIndex].Value?.ToString();
                if (cellValue == null)
                {
                    break;
                }
                columnNameDictionary[columnIndex] = cellValue;
                columnNameList.Append(cellValue);
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
