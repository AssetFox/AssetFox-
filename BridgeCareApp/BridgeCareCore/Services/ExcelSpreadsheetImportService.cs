using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
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

        /// <summary>This import is not particularly generic. It skips over columns whose top cell is empty,
        /// effectively deleting them from the imported spreadsheet.</summary>
        public ExcelSpreadsheetImportResultDTO ImportSpreadsheet(
            Guid dataSourceId, ExcelWorksheet worksheet, bool includeColumnsWithoutTitles = false
            )
        {
            var columnIndexesToInclude = new List<int>();
            
            var cells = worksheet.Cells;
            var end = cells.End;
            for (int i = 1; i<= end.Column; i++)
            {
                var titleContent = cells[1, i].Value;
                var shouldIncludeColumn = includeColumnsWithoutTitles || titleContent != null && !string.IsNullOrWhiteSpace(titleContent.ToString());
                if (shouldIncludeColumn) {
                    columnIndexesToInclude.Add(i);
                }
            }
            var columns = new List<ExcelDatabaseColumn>();
            for (var columnIndex = 1; columnIndex <= end.Column; columnIndex++)
            {
                if (columnIndexesToInclude.Contains(columnIndex))
                {
                    var columnCells = new List<IExcelCellDatum>();
                    for (var rowIndex = 1; rowIndex <= end.Row; rowIndex++)
                    {
                        var cellValue = cells[rowIndex, columnIndex].Value;
                        var newCell = ExcelCellData.ForObject(cellValue);
                        columnCells.Add(newCell);
                    }
                    while (columnCells.Any() && columnCells.Last() is EmptyExcelCellDatum)
                    {
                        columnCells.RemoveAt(columnCells.Count - 1);
                    }
                    var column = ExcelDatabaseColumns.WithEntries(columnCells);
                    columns.Add(column);
                }
            }
            var workseet = ExcelDatabaseWorksheets.WithColumns(columns);
            var newId = Guid.NewGuid();
            var dto = ExcelDatabaseWorksheetMapper.ToDTO(workseet, dataSourceId, newId);
            var returnId = _unitOfWork.ExcelWorksheetRepository.AddExcelWorksheet(dto);
            return new ExcelSpreadsheetImportResultDTO
            {
                SpreadsheetId = returnId,
            };
        }
    }
}
