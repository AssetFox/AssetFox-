using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class ExcelSpreadsheetImportService
    {
        private UnitOfDataPersistenceWork _unitOfWork;

        public ExcelSpreadsheetImportService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }
        public Guid ImportSpreadsheet(
            ExcelPackage excelPackage
            )
        {
            var cells = excelPackage.Workbook.Worksheets[0].Cells;
            var end = cells.End;
            var columns = new List<ExcelDatabaseColumn>();
            for (var columnIndex = 1; columnIndex <= end.Column; columnIndex++)
            {
                var columnCells = new List<IExcelCellDatum>();
                for (var rowIndex = 1; rowIndex <= end.Row; rowIndex++)
                {
                    var cellValue = cells[rowIndex, columnIndex].Value;
                    if (cellValue is double doubleValue)
                    {
                        columnCells.Add(ExcelCellData.Double(doubleValue));
                    }
                    else if (cellValue is int intValue)
                    {
                        columnCells.Add(ExcelCellData.Double(intValue));
                    }
                    else if (cellValue is DateTime dateTimeValue)
                    {
                        columnCells.Add(ExcelCellData.DateTime(dateTimeValue));
                    }
                    else if (cellValue is float floatValue)
                    {
                        columnCells.Add(ExcelCellData.Double(floatValue));
                    } else
                    {
                        columnCells.Add(ExcelCellData.String(cellValue.ToString()));
                    }
                }
                var column = ExcelDatabaseColumns.WithEntries(columnCells);
                columns.Add(column);
            }
            var workseet = ExcelDatabaseWorksheets.WithColumns(columns);
            var dto = ExcelDatabaseWorksheetMapper.ToDTO(workseet);
            var returnValue = _unitOfWork.ExcelWorksheetRepository.AddExcelWorksheet(dto);
            return returnValue;
        }
    }
}
