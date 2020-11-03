using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.BridgeData
{
    public class BridgeDataForSummaryReport : IBridgeDataForSummaryReport
    {
        private List<int> SpacerColumnNumbers;
        private readonly IExcelHelper _excelHelper;
        public BridgeDataForSummaryReport(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
        }

        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            // Add data to excel.
            var headers = GetHeaders();
            var simulationYears = new List<int>();
            reportOutputData.Years.ForEach(_ => simulationYears.Add(_.Year));
            var currentCell = AddHeadersCells(worksheet, headers, simulationYears);
            return new WorkSummaryModel();
        }

        #region Private Methods 
        private List<string> GetHeaders()
        {
            return new List<string>
            {
                "BridgeID",
                "BRKey",
                "District",
                "Bridge (B/C)",
                "Deck Area",
                "Structure Length",
                "Planning Partner",
                "Bridge Family",
                "NHS",
                "BPN",
                "Struct Type",
                //"Functional Class",
                "Year Built",
                "Age",
                "ADTT",
                //"ADT Over 10,000",
                "Risk Score",
                "P3"
            };
        }
        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            _excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);

            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work Done in ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = HeaderConstText + year;
                worksheet.Cells[row + 2, column].Value = year;
                _excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(244, 176, 132));
            }
            worksheet.Cells[row, ++column].Value = "Work Done more than once";
            worksheet.Cells[row, ++column].Value = "Total";
            worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            var poorOnOffRateColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, column].Value = year;
                _excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                column++;
            }

            // Merge 2 rows for headers till column before Poor On/Off Rate
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn < poorOnOffRateColumn; cellColumn++)
            {
                _excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }
            // Merge columns for Poor On/Off Rate
            _excelHelper.MergeCells(worksheet, row, poorOnOffRateColumn, row + 1, column - 1);
            currentCell.Column = column;

            // Add Years Data headers
            var simulationHeaderTexts = GetSimulationHeaderTexts();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count - 5);
            _excelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);

            // Empty column
            currentCell.Column = ++column;

            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            var yearHeaderColumn = currentCell.Column;
            simulationHeaderTexts.RemoveAll(_ => _.Equals("SD") || _.Equals("Posted"));
            SpacerColumnNumbers = new List<int>();

            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count);
                _excelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
                if (simulationYear % 2 != 0)
                {
                    _excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.Gray);
                }
                else
                {
                    _excelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.LightGray);
                }

                worksheet.Column(currentCell.Column).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Column(currentCell.Column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
                SpacerColumnNumbers.Add(currentCell.Column);

                currentCell.Column = ++column;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[row, initialColumn, row + 1, worksheet.Dimension.Columns]);
            currentCell.Row = currentCell.Row + 2;
        }
        private int AddSimulationHeaderTexts(ExcelWorksheet worksheet, int column, int row, List<string> simulationHeaderTexts, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = simulationHeaderTexts[index];
                _excelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }
        private List<string> GetSimulationHeaderTexts()
        {
            return new List<string>
            {
                "Deck Cond",
                "Super Cond",
                "Sub Cond",
                "Culv Cond",
                "Deck Dur",
                "Super Dur",
                "Sub Dur",
                "Culv Dur",
                "Min Cond",
                //"SD",
                "Poor",
                //"Posted",
                "Project Pick",
                "Budget",
                "Project",
                "Cost",
                "District Remarks"
            };
        }
        #endregion
    }
}
