using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage;
using AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage.CellData;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class ExcelRawDataImportService : IExcelRawDataImportService
    {

        public const string TopSpreadsheetRowIsEmpty = "The top row of the spreadsheet is empty. It is expected to contain column names.";
        public const string DataSourceDoesNotExist = "No DataSource in the database with id";

        private IUnitOfWork _unitOfWork;

        public ExcelRawDataImportService(
            IUnitOfWork unitOfWork
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
            var dataSource = _unitOfWork.DataSourceRepo.GetDataSource(dataSourceId);
            if (dataSource == null)
            {
                return new ExcelRawDataImportResultDTO
                {
                    WarningMessage = $"{DataSourceDoesNotExist} {dataSourceId}"
                };
            }
            var columnIndexesToInclude = new List<int>();

            var cells = worksheet.Cells;
            var end = worksheet.Dimension.End;

            int endRow = 1;
            int endCol = 1;
            for (int i = 1; i <= end.Row; i++)
            {
                //Check if the second column has text, in case the first column has gaps or ends early.
                if (!string.IsNullOrWhiteSpace(cells[i, 1].Text) || !string.IsNullOrWhiteSpace(cells[i, 2].Text))
                    endRow = i;
                else
                    break;
            }
            for (int j = 1; j <= end.Column; j++)
            {
                //Check for each column title, as it should exist.
                if (!string.IsNullOrWhiteSpace(cells[1, j].Text))
                    endCol = j;
                else
                    break;
            }
            for (int i = 1; i <= endCol; i++)
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
            for (var columnIndex = 1; columnIndex <= endCol; columnIndex++)
            {
                if (columnIndexesToInclude.Contains(columnIndex))
                {
                    var columnCells = new List<IExcelCellDatum>();
                    for (var rowIndex = 1; rowIndex <= endRow; rowIndex++)
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
            var dto = ExcelRawDataSpreadsheetSerializationMapper.ToDTO(workseet, dataSourceId, newId);
            var returnId = _unitOfWork.ExcelWorksheetRepository.AddExcelRawData(dto);
            return new ExcelRawDataImportResultDTO
            {
                RawDataId = returnId,
            };
        }
    }
}
