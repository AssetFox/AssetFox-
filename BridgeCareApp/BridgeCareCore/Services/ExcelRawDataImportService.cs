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
    public class ExcelRawDataImportService : IExcelRawDataImportService
    {

        public const string TopSpreadsheetRowIsEmpty = "The top row of the spreadsheet is empty. It is expected to contain column names.";

        private UnitOfDataPersistenceWork _unitOfWork;

        public ExcelRawDataImportService(
            UnitOfDataPersistenceWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>This import is not particularly generic. It skips over columns whose top cell is empty,
        /// effectively deleting them from the imported spreadsheet.</summary>
        public ExcelRawDataImportResultDTO ImportRawData(
            Guid dataSourceId,
            ExcelWorksheet worksheet,
            bool includeColumnsWithoutTitles = false
            )
        {
            var columnIndexesToInclude = new List<int>();

            var cells = worksheet.Cells;
            var end = cells.End;
            for (int i = 1; i <= end.Column; i++)
            {
                var titleContent = cells[1, i].Value;
                var shouldIncludeColumn = includeColumnsWithoutTitles || titleContent != null && !string.IsNullOrWhiteSpace(titleContent.ToString());
                if (shouldIncludeColumn)
                {
                    columnIndexesToInclude.Add(i);
                }
            }
            if (!columnIndexesToInclude.Any())
            {
                return new ExcelRawDataImportResultDTO
                {
                    WarningMessage = TopSpreadsheetRowIsEmpty,
                };
            }
            var columns = new List<ExcelRawDataColumn>();
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
                    var column = ExcelRawDataColumns.WithEntries(columnCells);
                    columns.Add(column);
                }
            }
            var workseet = ExcelRawDataSpreadsheets.WithColumns(columns);
            var newId = Guid.NewGuid();
            var dto = ExcelDatabaseWorksheetMapper.ToDTO(workseet, dataSourceId, newId);
            var returnId = _unitOfWork.ExcelWorksheetRepository.AddExcelRawData(dto);
            return new ExcelRawDataImportResultDTO
            {
                RawDataId = returnId,
            };
        }
    }
}
