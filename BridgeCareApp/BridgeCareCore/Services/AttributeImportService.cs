using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class AttributeImportService
    {

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
            for (var i=1; i<=rowLength; i++)
            {
                var cellValue = cells[rowIndex, i].Value?.ToString();
                if (cellValue == null)
                {
                    break;
                }
                columnNameDictionary[i] = cellValue;
                columnNameList.Append(cellValue);
                if (cellValue == keyColumnName)
                {
                    keyColumnIndex = i;
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
            throw new NotImplementedException();
        }

        private static string BuildKeyNotFoundWarningMessage(string keyColumnName, Dictionary<int, string> columnNameDictionary)
        {
            string warningMessage;
            if (columnNameDictionary.Keys.Any())
            {
                var warningMessageBuilder = new StringBuilder();
                warningMessageBuilder.AppendLine($"No column found for Id {keyColumnName}");
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
