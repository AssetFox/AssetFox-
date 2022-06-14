﻿using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;


namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.WorksheetContentExtractors
{
    public static class ExcelWorksheetContentExtractor
    {
        public static ExcelDatabaseWorksheet ExtractContent(ExcelWorksheet worksheet)
        {
            var end = worksheet.Cells.End;
            var returnValue = new ExcelDatabaseWorksheet();
            for (int columnIndex = 1; columnIndex <= end.Column; columnIndex++)
            {
                var column = new ExcelDatabaseColumn();
                for (int rowIndex = 1; rowIndex <= end.Row; rowIndex++)
                {
                    var cell = worksheet.Cells[rowIndex, columnIndex];
                    var cellValue = cell.Value;
                    var content = ExcelCellContentExtractor.ExtractContent(cellValue);
                    column.Entries.Add(content);
                }
                returnValue.Columns.Add(column);
            }
            return returnValue;
        }
    }
}
