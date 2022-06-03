using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class AttributeImportService
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string NoColumnFoundForId = "No column found for Id";

        public AttributeImportService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public AttributesImportResultDTO ImportExcelAttributes(string keyColumnName, ExcelPackage excelPackage)
        {
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
            var keyColumnIndex = -1;
            for (var columnIndex=1; columnIndex<=rowLength; columnIndex++)
            {
                var cellValue = cells[rowIndex, columnIndex].Value?.ToString();
                if (cellValue == null)
                {
                    break;
                }
                columnNameDictionary[columnIndex] = cellValue;
                columnNameList.Append(cellValue);
                if (cellValue == keyColumnName)
                {
                    keyColumnIndex = columnIndex;
                }
            }
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
            var allAttributeNamesAsList = string.Join(", ", allAttributeNames);
            var columnIndexAttributeDictionary = new Dictionary<int, AttributeDTO>();
            foreach (var columnIndex in columnNameDictionary.Keys)
            {
                if (columnIndex!=keyColumnIndex)
                {
                    var attributeName = columnNameDictionary[columnIndex];
                    var attribute = allAttributes.FirstOrDefault(a => a.Name == attributeName);
                    if (attribute == null)
                    {
                        var warningMessage = $"The title of colum {columnIndex} is {attributeName}. However, no attribute was found with name {attributeName}. The following is a list of all attribute names: {allAttributeNames}";
                        return new AttributesImportResultDTO
                        {
                            WarningMessage = warningMessage,
                        };
                    }
                    columnIndexAttributeDictionary[columnIndex] = attribute;
                }
            }
            throw new NotImplementedException();
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
