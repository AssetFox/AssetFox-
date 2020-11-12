using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities;
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
        private readonly IHighlightWorkDoneCells _highlightWorkDoneCells;
        private Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>> valueForMinC;
        private readonly List<int> SimulationYears = new List<int>();
        private readonly List<double> previousYearInitialMinC = new List<double>();
        public BridgeDataForSummaryReport(IExcelHelper excelHelper, IHighlightWorkDoneCells highlightWorkDoneCells)
        {
            _excelHelper = excelHelper ?? throw new ArgumentNullException(nameof(excelHelper));
            _highlightWorkDoneCells = highlightWorkDoneCells ?? throw new ArgumentNullException(nameof(highlightWorkDoneCells));
        }

        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData,
            SortedSet<PennDotReportAEntity> pennDotReportAData)
        {
            // Add data to excel.
            var headers = GetHeaders();

            reportOutputData.Years.ForEach(_ => SimulationYears.Add(_.Year));

            var currentCell = AddHeadersCells(worksheet, headers, SimulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            AddBridgeDataModelsCells(worksheet, reportOutputData, currentCell);
            AddDynamicDataCells(worksheet, reportOutputData, pennDotReportAData, currentCell);

            worksheet.Cells.AutoFitColumns();
            var spacerBeforeFirstYear = SpacerColumnNumbers[0] - 11;
            worksheet.Column(spacerBeforeFirstYear).Width = 3;
            foreach (var spacerNumber in SpacerColumnNumbers)
            {
                worksheet.Column(spacerNumber).Width = 3;
            }
            var lastColumn = worksheet.Dimension.Columns + 1;
            worksheet.Column(lastColumn).Width = 3;

            var workSummaryModel = new WorkSummaryModel
            {
            };

            return workSummaryModel;
        }

        #region Private Methods
        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput outputResults,
            SortedSet<PennDotReportAEntity> pennDotReportAData,
            CurrentCell currentCell)
        {
            var initialRow = 4;
            var row = 4; // Data starts here
            var startingRow = row;
            var column = currentCell.Column;
            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();

            // making dictionary to remove if else, which was used to enter value for MinC
            valueForMinC = new Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>>();
            valueForMinC.Add(MinCValue.defaultValue, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterDefaultMinCValue));
            valueForMinC.Add(MinCValue.valueEqualsCulv, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterValueEqualsCulv));
            valueForMinC.Add(MinCValue.minOfDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSub));
            valueForMinC.Add(MinCValue.minOfCulvDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSubCulv));

            var workDoneData = new List<int>();
            var previousYearSectionMinC = new List<double>();
            if (outputResults.Years.Count > 0)
            {
                workDoneData = new List<int>(new int[outputResults.Years[0].Sections.Count]);
                previousYearSectionMinC = new List<double>(new double[outputResults.Years[0].Sections.Count]);
            } 
            var poorOnOffColumnStart = outputResults.Years.Count + column + 2;
            var index = 1; // to track the initial section from rest of the years
            foreach (var yearlySectionData in outputResults.Years)
            {
                row = initialRow;

                // Add work done cells
                //yearlySectionData.Sections.Sort(
                //    (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
                //    );

                var sectionsAndReportAData = yearlySectionData.Sections.Zip(pennDotReportAData, (n, w) => new { section = n, reportAData = w });
                TreatmentCause previousYearCause = TreatmentCause.Undefined;
                var i = 0;
                foreach (var data in sectionsAndReportAData)
                {
                    // Work done in a year
                    var range = worksheet.Cells[row, column];
                    setColor(data.reportAData.Parallel_Struct, data.section.TreatmentName, previousYearCause, data.section.TreatmentCause,
                        yearlySectionData.Year, index, data.section.TreatmentName, worksheet, row, column);

                    if (abbreviatedTreatmentNames.ContainsKey(data.section.TreatmentName))
                    {
                        range.Value = string.IsNullOrEmpty(abbreviatedTreatmentNames[data.section.TreatmentName]) ? "--" : abbreviatedTreatmentNames[data.section.TreatmentName];
                    }
                    else
                    {
                        range.Value = "--";
                    }
                    if (!range.Value.Equals("--"))
                    {
                        workDoneData[i]++;
                    }

                    // poor on off Rate
                    var prevYrMinc = 0.0;
                    if (index == 1)
                    {
                        prevYrMinc = previousYearInitialMinC[i];
                    }
                    else
                    {
                        prevYrMinc = previousYearSectionMinC[i];
                    }

                    var thisYrMinc = data.section.ValuePerNumericAttribute["MINCOND"];

                    worksheet.Cells[row, poorOnOffColumnStart].Value = prevYrMinc < 5 ? (thisYrMinc >= 5 ? "Off" : "--") :
                        (thisYrMinc < 5 ? "On" : "--");
                    previousYearCause = data.section.TreatmentCause;
                    previousYearSectionMinC[i] = thisYrMinc;
                    i++;
                    row++;
                }
                index++;

                poorOnOffColumnStart++;
                column++;
            }

            // work done information
            row = 4; // setting row back to start
            foreach (var wdInfo in workDoneData)
            {
                worksheet.Cells[row++, column].Value = wdInfo > 1 ? "Yes" : "--";
            }
            column = column + outputResults.Years.Count + 2; // this will take us to the empty column after "poor on off"
            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            row = 4; // setting row back to start
            var initialColumn = column;
            foreach (var intialsection in outputResults.InitialSectionSummaries)
            {
                column = initialColumn; // This is to reset the column
                column = AddSimulationYearData(worksheet, row, column, intialsection, null);
                row++;
            }
            currentCell.Column = column++;
            currentCell.Row = initialRow;
            foreach (var sectionData in outputResults.Years)
            {
                row = currentCell.Row; // setting row back to start
                currentCell.Column = column;
                foreach (var section in sectionData.Sections)
                {
                    column = currentCell.Column;
                    column = AddSimulationYearData(worksheet, row, column, null, section);

                    worksheet.Cells[row, ++column].Value = section.TreatmentCause; // Project Pick
                    // [TODO] this value is just a placeholder
                    worksheet.Cells[row, ++column].Value = section.ValuePerTextAttribute["STRUCTURE_TYPE"]; // Budget
                    worksheet.Cells[row, ++column].Value = section.TreatmentName; // Column name is "Project"
                    if (section.TreatmentCause == TreatmentCause.SelectedTreatment)
                    {
                        _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(0, 255, 0));
                        _excelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
                    }
                    worksheet.Cells[row, ++column].Value = 0; // [TODO] it is just a placeholder for cost
                    _excelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
                    worksheet.Cells[row, ++column].Value = ""; // District Remarks
                    column = column+1;
                    row++;
                }
            }
        }
        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column,
            SectionSummaryDetail initialSection, SectionDetail section)
        {
            var selectedSection = initialSection ?? section;
            var minCActionCallDecider = MinCValue.minOfCulvDeckSubSuper;
            var familyId = int.Parse(selectedSection.ValuePerTextAttribute["FAMILY_ID"]);
            var familyIdLessThanEleven = familyId < 11;
            if (familyId > 10)
            {
                worksheet.Cells[row, ++column].Value = "N"; // deck cond
                worksheet.Cells[row, ++column].Value = "N"; // super cond
                worksheet.Cells[row, ++column].Value = "N"; // sub cond

                worksheet.Cells[row, column + 2].Value = "N"; // deck dur
                worksheet.Cells[row, column + 3].Value = "N"; // super dur
                worksheet.Cells[row, column + 4].Value = "N"; // sub dur
                minCActionCallDecider = MinCValue.valueEqualsCulv;
            }
            else
            {
                worksheet.Cells[row, ++column].Value = selectedSection.ValuePerNumericAttribute["PREV_DECKSEED"];
                worksheet.Cells[row, ++column].Value = selectedSection.ValuePerNumericAttribute["PREV_SUPSEED"];
                worksheet.Cells[row, ++column].Value = selectedSection.ValuePerNumericAttribute["PREV_SUBSEED"];

                worksheet.Cells[row, column + 2].Value = selectedSection.ValuePerNumericAttribute["DECK_DURATION_N"];
                worksheet.Cells[row, column + 3].Value = selectedSection.ValuePerNumericAttribute["SUP_DURATION_N"];
                worksheet.Cells[row, column + 4].Value = selectedSection.ValuePerNumericAttribute["SUB_DURATION_N"];
            }
            if (familyIdLessThanEleven)
            {
                worksheet.Cells[row, ++column].Value = "N"; // culv cond
                worksheet.Cells[row, column + 4].Value = "N"; // culv seeded

                if (minCActionCallDecider == MinCValue.valueEqualsCulv)
                {
                    minCActionCallDecider = MinCValue.defaultValue;
                }
                else
                {
                    minCActionCallDecider = MinCValue.minOfDeckSubSuper;
                }
            }
            else
            {
                worksheet.Cells[row, ++column].Value = selectedSection.ValuePerNumericAttribute["PREV_CULVSEED"];

                worksheet.Cells[row, column + 4].Value = selectedSection.ValuePerNumericAttribute["CULV_DURATION_N"];
            }
            column += 4; // this will take us to "Min cond" column

            // It returns the column number where MinC value is written
            column = valueForMinC[minCActionCallDecider](worksheet, row, column, selectedSection.ValuePerNumericAttribute);
            if (selectedSection.ValuePerNumericAttribute["P3"] > 0 && selectedSection.ValuePerNumericAttribute["MINCOND"] < 5)
            {
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.Yellow);
                _excelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
            }
            worksheet.Cells[row, ++column].Value = selectedSection.ValuePerNumericAttribute["MINCOND"] < 5 ? "Y" : "N"; //poor

            return column;
        }
        private void AddBridgeDataModelsCells(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;
            //reportOutputData.InitialSectionSummaries.Sort(
            //        (a, b) => int.Parse(a.FacilityName).CompareTo(int.Parse(b.FacilityName))
            //        );
            foreach (var sectionSummary in reportOutputData.InitialSectionSummaries)
            {
                rowNo++;
                if (rowNo % 2 == 0)
                {
                    _excelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, worksheet.Dimension.Columns], Color.LightGray);
                }
                columnNo = 1;
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.SectionName;
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.FacilityName;
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DISTRICT"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BRIDGE_TYPE"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_AREA"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LENGTH"];
                columnNo++; // temporary, because we can commented out 1 excel rows

                //worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.PlanningPartner;
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FAMILY_ID"];
                worksheet.Cells[rowNo, columnNo++].Value = int.Parse(sectionSummary.ValuePerTextAttribute["NHS_IND"]) > 0 ? "Y" : "N";
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["STRUCTURE_TYPE"];
                worksheet.Cells[rowNo, columnNo++].Value = (int)sectionSummary.ValuePerNumericAttribute["YEAR_BUILT"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["AGE"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["ADTTOTAL"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISK_SCORE"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["P3"] > 0 ? "Y" : "N";
                previousYearInitialMinC.Add(sectionSummary.ValuePerNumericAttribute["MINCOND"]);
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }
        private void setColor(int parallelBridge, string treatment, TreatmentCause previousYearCause,
           TreatmentCause treatmentCause, int year, int index, string project, ExcelWorksheet worksheet, int row, int column)
        {
            _highlightWorkDoneCells.CheckConditions(parallelBridge, treatment, previousYearCause, treatmentCause, year, index, project, worksheet, row, column);
        }
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
        private int EnterDefaultMinCValue(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            worksheet.Cells[row, ++column].Value = "N";
            // It is a dummy value
            numericAttribute["MINCOND"] = 100;
            return column;
        }
        private int EnterValueEqualsCulv(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            numericAttribute["MINCOND"] = numericAttribute["CULV"];
            worksheet.Cells[row, ++column].Value = numericAttribute["MINCOND"];
            if (numericAttribute["MINCOND"] <= 3.5)
            {
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                _excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private int EnterMinDeckSuperSub(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            var minValue = Math.Min(numericAttribute["DECK"], Math.Min(numericAttribute["SUP"], numericAttribute["SUB"]));
            worksheet.Cells[row, ++column].Value = minValue;
            numericAttribute["MINCOND"] = minValue;
            if (numericAttribute["MINCOND"] <= 3.5)
            {
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                _excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private int EnterMinDeckSuperSubCulv(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            worksheet.Cells[row, ++column].Value = numericAttribute["MINCOND"];
            if (numericAttribute["MINCOND"] <= 3.5)
            {
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(112, 48, 160));
                _excelHelper.SetTextColor(worksheet.Cells[row, column], Color.White);
            }
            return column;
        }
        private enum MinCValue
        {
            minOfCulvDeckSubSuper,
            minOfDeckSubSuper,
            valueEqualsCulv,
            defaultValue
        }
        #endregion
    }
}
