using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData
{
    public class PamsDataForSummaryReport: IPamsDataForSummaryReport
    {
        private List<int> _spacerColumnNumbers;
        private readonly List<int> _simulationYears = new List<int>();

        public PamsDataForSummaryReport()
        {
            
        }

        private List<string> GetHeaders()
        {
            return new List<string>
            {
                "Asset Management Section",
                "District",
                "County",
                "Co No",
                "Route",
                "Segment",

                "Length",
                "Width",
                "Pavement Depth",
                "Direction",

                "Lanes",
                "FamilyID",
                "MPO/ RPO",

                "Surface",
                "BPN",

                "Year Built",
                "Year Last ResurfaceYear Last  Structural overlay",

                "ADT",
                "Truck %",
                "ESALS",
                "Risk Score",
            };
        }

        private List<string> GetYearDataSubHeaders()
        {
            return new List<string>
            {
                "OPI",
                "IRI Rutting",
                "Faulting",
                "Cracking",
                "Project Source",
                "Budget",
                "Recommended Treatment",
                "Cost",
                "District Remarks"
            };
        }


        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            // Add data to excel.
            reportOutputData.Years.ForEach(_ => _simulationYears.Add(_.Year));
            var currentCell = BuildHeaderAndSubHeaders(worksheet, _simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1]) {
                autoFilterCells.AutoFilter = true;
            }

            //AddPamsDataModelsCells(worksheet, reportOutputData, currentCell);
            //AddDynamicDataCells(worksheet, reportOutputData, currentCell);

            worksheet.Cells.AutoFitColumns();
            var spacerBeforeFirstYear = _spacerColumnNumbers[0] - 11;
            worksheet.Column(spacerBeforeFirstYear).Width = 3;
            foreach (var spacerNumber in _spacerColumnNumbers)
            {
                worksheet.Column(spacerNumber).Width = 3;
            }
            var lastColumn = worksheet.Dimension.Columns + 1;
            worksheet.Column(lastColumn).Width = 3;

            return;
        }

        private CurrentCell BuildHeaderAndSubHeaders(ExcelWorksheet worksheet, List<int> simulationYears)
        {
            
            //Get Headers
            var headers = GetHeaders();

            //add header columns
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++) {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }

            //ExcelHelper.MergeCells(worksheet, 1, headers.Count, 1, headers.Count + 5);

            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count + 1 };
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work to be Done in ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears) {
                ExcelHelper.MergeCells(worksheet, row, ++column, row, ++column);
                worksheet.Cells[row, column - 1].Value = HeaderConstText + year;
                worksheet.Cells[row + 2, column - 1].Value = PAMSConstants.Work;
                worksheet.Cells[row + 2, column].Value = PAMSConstants.Cost;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column - 1, row + 2, column]);
                ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], Color.FromArgb(244, 176, 132));
            }

            worksheet.Cells[row, ++column].Value = "Work Done";
            worksheet.Cells[row, ++column].Value = "Work Done more than once";
            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1, row, column], Color.FromArgb(244, 176, 132));

            worksheet.Cells[row, ++column].Value = "Total";
            //worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            //var poorOnOffRateColumn = column;
            //foreach (var year in simulationYears)
            //{
            //    worksheet.Cells[row + 2, column].Value = year;
            //    ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
            //    column++;
            //}

            worksheet.Row(row).Height = 40;            
            currentCell.Column = column;

            // Add Years Data headers
            var yearDataSubHeaders = GetYearDataSubHeaders();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddYearDataSubHeaders(worksheet, column, row, yearDataSubHeaders, yearDataSubHeaders.Count - 5);
            ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);

            // Empty column
            currentCell.Column = ++column;

            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            var yearHeaderColumn = currentCell.Column;
            yearDataSubHeaders.RemoveAll(_ => _.Equals("SD") || _.Equals("Posted"));
            _spacerColumnNumbers = new List<int>();

            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddYearDataSubHeaders(worksheet, column, row, yearDataSubHeaders, yearDataSubHeaders.Count);
                ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
                if (simulationYear % 2 != 0)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.Gray);
                }
                else
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], Color.LightGray);
                }

                worksheet.Column(currentCell.Column).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Column(currentCell.Column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
                _spacerColumnNumbers.Add(currentCell.Column);

                currentCell.Column = ++column;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, initialColumn, row + 1, worksheet.Dimension.Columns]);
            currentCell.Row = currentCell.Row + 2;
        }

        private int AddYearDataSubHeaders(ExcelWorksheet worksheet, int column, int row, List<string> yearDataSubHeaders, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = yearDataSubHeaders[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }

        
    }
}
