using System;
using System.Collections.Generic;
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
            var columnNames = new List<string>();
            for (int i=1; i<=rowLength; i++)
            {
                var cellValue = cells[rowIndex, i].Value?.ToString();
                if (cellValue == null)
                {
                    break;
                }
                columnNames.Add(cellValue);
            }
            throw new NotImplementedException();
        }
    }
}
