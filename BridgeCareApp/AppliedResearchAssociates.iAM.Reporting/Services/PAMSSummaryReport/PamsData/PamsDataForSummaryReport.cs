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
                "Year Last Resurface",
                "Year Last  Structural overlay",

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

            AddPamsDataModelsCells(worksheet, reportOutputData, currentCell);
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
                ExcelHelper.MergeCells(worksheet, headerRow, column + 1, headerRow + 1, column + 1);
            }

            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work to be Done in ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                ExcelHelper.MergeCells(worksheet, row, ++column, row, ++column);
                worksheet.Cells[row, column - 1].Value = HeaderConstText + year;
                worksheet.Cells[row + 1, column - 1].Value = PAMSConstants.Work;
                worksheet.Cells[row + 1, column].Value = PAMSConstants.Cost;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column - 1, row + 1, column]);
                ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], Color.FromArgb(244, 176, 132));
            }

            worksheet.Cells[row, ++column].Value = "Work Done"; 
            worksheet.Cells[row, ++column].Value = "Work Done more than once";            
            worksheet.Cells[row, ++column].Value = "Total";

            ExcelHelper.ApplyStyle(worksheet.Cells[row, column-2, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[row, column-2, row, column], Color.FromArgb(244, 176, 132));

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

        private void AddPamsDataModelsCells(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row; var columnNo = currentCell.Column;
            foreach (var sectionSummary in reportOutputData.InitialSectionSummaries)
            {
                rowNo++; columnNo = 2;
                //var splitIds = sectionSummary.FacilityName.Split('-');
                //var sectionId = ""; var facilityId = splitIds[0];
                //if (splitIds.Length == 2) { sectionId = splitIds[1]; }

                //worksheet.Cells[rowNo, columnNo++].Value = sectionId;
                //worksheet.Cells[rowNo, columnNo++].Value = facilityId;
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DISTRICT"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["COUNTY"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["CNTY"];

                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["SR"];
                worksheet.Cells[rowNo, columnNo++].Value = ""; //sectionSummary.ValuePerNumericAttribute["SEGMENT"];

                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SEGMENT_LENGTH"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["WIDTH"];
                worksheet.Cells[rowNo, columnNo++].Value = ""; // sectionSummary.ValuePerNumericAttribute["DEPTH"];

                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DIRECTION"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LANES"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FAMILY"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["MPO_RPO"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SURFACEID"] + "-" + sectionSummary.ValuePerTextAttribute["SURFACE_NAME"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BUSIPLAN"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["YR_BUILT"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["YEAR_LAST_OVERLAY"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LAST_STRUCTURAL_OVERLAY"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["AADT"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["TRK_PERCENT"];
                worksheet.Cells[rowNo, columnNo++].Value = ""; // sectionSummary.ValuePerNumericAttribute["ESLAS"];
                worksheet.Cells[rowNo, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISKSCORE"];

                if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray); }
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        //private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput outputResults, CurrentCell currentCell)
        //{
        //    var initialRow = 4;
        //    var row = 4; // Data starts here
        //    var startingRow = row;
        //    var column = currentCell.Column;
        //    var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();

        //    // making dictionary to remove if else, which was used to enter value for MinC
        //    _valueForMinC = new Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>>();
        //    _valueForMinC.Add(MinCValue.defaultValue, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterDefaultMinCValue));
        //    _valueForMinC.Add(MinCValue.valueEqualsCulv, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterValueEqualsCulv));
        //    _valueForMinC.Add(MinCValue.minOfDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSub));
        //    _valueForMinC.Add(MinCValue.minOfCulvDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSubCulv));

        //    var workDoneData = new List<int>();
        //    var previousYearSectionMinC = new List<double>();
        //    if (outputResults.Years.Count > 0)
        //    {
        //        workDoneData = new List<int>(new int[outputResults.Years[0].Sections.Count]);
        //        previousYearSectionMinC = new List<double>(new double[outputResults.Years[0].Sections.Count]);
        //    }
        //    var poorOnOffColumnStart = (outputResults.Years.Count * 2) + column + 3;
        //    var index = 1; // to track the initial section from rest of the years

        //    var isInitialYear = true;
        //    foreach (var yearlySectionData in outputResults.Years)
        //    {
        //        _poorOnOffCount.Add(yearlySectionData.Year, (on: 0, off: 0));

        //        row = initialRow;

        //        // Add work done cells
        //        TreatmentCause previousYearCause = TreatmentCause.Undefined;
        //        var previousYearTreatment = BAMSConstants.NoTreatment;
        //        var i = 0;
        //        foreach (var section in yearlySectionData.Sections)
        //        {
        //            TrackDataForParametersTAB(section.ValuePerNumericAttribute, section.ValuePerTextAttribute);

        //            if (!_bpnPoorOnPerYear.ContainsKey(yearlySectionData.Year))
        //            {
        //                _bpnPoorOnPerYear.Add(yearlySectionData.Year, new Dictionary<string, int>());
        //            }

        //            if (!_nhsPoorOnPerYear.ContainsKey(yearlySectionData.Year))
        //            {
        //                _nhsPoorOnPerYear.Add(yearlySectionData.Year, 0);
        //            }

        //            if (!_nonNhsPoorOnPerYear.ContainsKey(yearlySectionData.Year))
        //            {
        //                _nonNhsPoorOnPerYear.Add(yearlySectionData.Year, 0);
        //            }

        //            bool isNHS = int.TryParse(section.ValuePerTextAttribute["NHS_IND"], out var numericValue) && numericValue > 0;

        //            int nhsOrNonPoorOnCount = isNHS ? _nhsPoorOnPerYear[yearlySectionData.Year] : _nonNhsPoorOnPerYear[yearlySectionData.Year];

        //            Dictionary<string, int> bpnPoorOnDictionary = _bpnPoorOnPerYear[yearlySectionData.Year];
        //            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
        //            int bpnPoorOnCount;
        //            // Create/Update BPN info for this Section/Year
        //            if (!bpnPoorOnDictionary.ContainsKey(busPlanNetwork))
        //            {
        //                bpnPoorOnCount = 0;
        //                bpnPoorOnDictionary.Add(busPlanNetwork, bpnPoorOnCount);
        //            }
        //            else
        //            {
        //                bpnPoorOnCount = bpnPoorOnDictionary[busPlanNetwork];
        //            }

        //            var thisYrMinc = section.ValuePerNumericAttribute["MINCOND"];
        //            // poor on off Rate
        //            var prevYrMinc = 0.0;
        //            if (index == 1)
        //            {
        //                prevYrMinc = _previousYearInitialMinC[i];
        //            }
        //            else
        //            {
        //                prevYrMinc = previousYearSectionMinC[i];
        //            }
        //            SectionDetail prevYearSection = null;
        //            if (section.TreatmentCause == TreatmentCause.CommittedProject && !isInitialYear)
        //            {
        //                prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
        //                    .Sections.FirstOrDefault(_ => _.SectionName == section.SectionName);
        //                previousYearCause = prevYearSection.TreatmentCause;
        //                previousYearTreatment = prevYearSection.AppliedTreatment;
        //            }
        //            setColor((int)section.ValuePerNumericAttribute["PARALLEL"], section.AppliedTreatment, previousYearTreatment, previousYearCause, section.TreatmentCause,
        //                yearlySectionData.Year, index, worksheet, row, column);

        //            // Work done in a year
        //            var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
        //            var range = worksheet.Cells[row, column];
        //            if (abbreviatedTreatmentNames.ContainsKey(section.AppliedTreatment))
        //            {
        //                range.Value = abbreviatedTreatmentNames[section.AppliedTreatment];
        //                worksheet.Cells[row, column + 1].Value = cost;


        //                if (!isInitialYear && section.TreatmentCause == TreatmentCause.CashFlowProject)
        //                {
        //                    if (prevYearSection == null)
        //                    {
        //                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
        //                    .Sections.FirstOrDefault(_ => _.SectionName == section.SectionName);
        //                    }
        //                    if (prevYearSection.AppliedTreatment == section.AppliedTreatment)
        //                    {
        //                        range.Value = "--";
        //                        worksheet.Cells[row, column + 1].Value = cost;
        //                    }
        //                }
        //                worksheet.Cells[row, column + 1].Value = cost;
        //                ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);

        //            }
        //            else
        //            {
        //                range.Value = section.AppliedTreatment.ToLower() == BAMSConstants.NoTreatment ? "--" :
        //                    section.AppliedTreatment.ToLower();

        //                worksheet.Cells[row, column + 1].Value = cost;
        //                ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);
        //            }
        //            if (!range.Value.Equals("--"))
        //            {
        //                workDoneData[i]++;
        //            }

        //            worksheet.Cells[row, poorOnOffColumnStart].Value = prevYrMinc < 5 ? (thisYrMinc >= 5 ? "Off" : "--") :
        //                (thisYrMinc < 5 ? "On" : "--");

        //            var onOffCount = _poorOnOffCount[yearlySectionData.Year];
        //            if (worksheet.Cells[row, poorOnOffColumnStart].Value.ToString() == "On")
        //            {
        //                onOffCount.on += 1;
        //                bpnPoorOnCount += 1;
        //                nhsOrNonPoorOnCount += 1;
        //            }
        //            else if (worksheet.Cells[row, poorOnOffColumnStart].Value.ToString() == "Off")
        //            {
        //                onOffCount.off += 1;
        //            }
        //            _poorOnOffCount[yearlySectionData.Year] = onOffCount;
        //            bpnPoorOnDictionary[busPlanNetwork] = bpnPoorOnCount;
        //            if (isNHS)
        //            {
        //                _nhsPoorOnPerYear[yearlySectionData.Year] = nhsOrNonPoorOnCount;
        //            }
        //            else
        //            {
        //                _nonNhsPoorOnPerYear[yearlySectionData.Year] = nhsOrNonPoorOnCount;
        //            }

        //            previousYearSectionMinC[i] = thisYrMinc;
        //            i++;
        //            if (row % 2 == 0)
        //            {
        //                if (section.TreatmentCause != TreatmentCause.CashFlowProject ||
        //                    section.TreatmentCause == TreatmentCause.CommittedProject)
        //                {
        //                    ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.LightGray);
        //                }
        //                ExcelHelper.ApplyColor(worksheet.Cells[row, poorOnOffColumnStart], Color.LightGray);
        //            }
        //            ExcelHelper.ApplyLeftBorder(worksheet.Cells[row, column]);
        //            ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column + 1]);
        //            row++;
        //        }
        //        index++;

        //        poorOnOffColumnStart++;
        //        column += 2;
        //        isInitialYear = false;
        //    }

        //    // work done information
        //    row = 4; // setting row back to start
        //    column++;
        //    var totalWorkMoreThanOnce = 0;
        //    foreach (var wdInfo in workDoneData)
        //    {
        //        // Work Done
        //        worksheet.Cells[row, column - 1].Value = wdInfo >= 1 ? "Yes" : "--";
        //        // Work done more than once
        //        worksheet.Cells[row, column].Value = wdInfo > 1 ? "Yes" : "--";
        //        ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1, row, column]);
        //        if (row % 2 == 0)
        //        {
        //            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1, row, column + 1], Color.LightGray);
        //        }
        //        row++;
        //        totalWorkMoreThanOnce += wdInfo > 1 ? 1 : 0;
        //    }

        //    // "Total" column
        //    worksheet.Cells[3, column + 1].Value = totalWorkMoreThanOnce;
        //    ExcelHelper.ApplyStyle(worksheet.Cells[3, column + 1]);

        //    column = column + outputResults.Years.Count + 2; // this will take us to the empty column after "poor on off"
        //    worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

        //    row = 4; // setting row back to start
        //    var initialColumn = column;
        //    foreach (var intialsection in outputResults.InitialSectionSummaries)
        //    {
        //        TrackInitialYearDataForParametersTAB(intialsection);
        //        column = initialColumn; // This is to reset the column
        //        column = AddSimulationYearData(worksheet, row, column, intialsection, null);
        //        row++;
        //    }
        //    currentCell.Column = column++;
        //    currentCell.Row = initialRow;
        //    isInitialYear = true;
        //    foreach (var sectionData in outputResults.Years)
        //    {
        //        row = currentCell.Row; // setting row back to start
        //        currentCell.Column = column;
        //        foreach (var section in sectionData.Sections)
        //        {
        //            column = currentCell.Column;
        //            column = AddSimulationYearData(worksheet, row, column, null, section);
        //            var initialColumnForShade = column;

        //            SectionDetail prevYearSection = null;
        //            if (!isInitialYear)
        //            {
        //                prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == sectionData.Year - 1)
        //                    .Sections.FirstOrDefault(_ => _.SectionName == section.SectionName);
        //            }

        //            if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
        //            {
        //                var cashFlowMap = MappingContent.GetCashFlowProjectPick(section.TreatmentCause, prevYearSection);
        //                worksheet.Cells[row, ++column].Value = cashFlowMap.currentPick; //Project Pick
        //                worksheet.Cells[row, column - 16].Value = cashFlowMap.previousPick; //Project Pick previous year
        //            }
        //            else
        //            {
        //                worksheet.Cells[row, ++column].Value = MappingContent.GetNonCashFlowProjectPick(section.TreatmentCause);//Project Pick
        //            }

        //            var treatmentConsideration = section.TreatmentConsiderations.FindAll(_ => _.TreatmentName == section.AppliedTreatment);
        //            BudgetUsageDetail budgetUsage = null;

        //            foreach (var item in treatmentConsideration)
        //            {
        //                budgetUsage = item.BudgetUsages.Find(_ => _.Status == BudgetUsageStatus.CostCoveredInFull ||
        //            _.Status == BudgetUsageStatus.CostCoveredInPart);
        //            }

        //            var budgetName = budgetUsage == null ? "" : budgetUsage.BudgetName;

        //            worksheet.Cells[row, ++column].Value = budgetName; // Budget
        //            worksheet.Cells[row, ++column].Value = section.AppliedTreatment; // Project
        //            var columnForAppliedTreatment = column;

        //            var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
        //            worksheet.Cells[row, ++column].Value = cost; // cost
        //            ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
        //            worksheet.Cells[row, ++column].Value = ""; // District Remarks

        //            if (row % 2 == 0)
        //            {
        //                ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
        //            }

        //            if (section.TreatmentCause == TreatmentCause.CashFlowProject)
        //            {
        //                ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(0, 255, 0));
        //                ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(255, 0, 0));

        //                // Color the previous year project also
        //                ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment - 16], Color.FromArgb(0, 255, 0));
        //                ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment - 16], Color.FromArgb(255, 0, 0));
        //            }

        //            column = column + 1;
        //            row++;
        //        }
        //        isInitialYear = false;
        //    }
        //}
    }
}
