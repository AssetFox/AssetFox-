using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PamsData
{
    public class PamsDataForSummaryReport
    {
        private List<int> _spacerColumnNumbers;
        private readonly List<int> _simulationYears = new List<int>();
        private SummaryReportHelper _summaryReportHelper;

        // This is also used in Bridge Work Summary TAB
        private readonly List<double> _previousYearInitialMinC = new List<double>();
        private Dictionary<int, (int on, int off)> _poorOnOffCount = new Dictionary<int, (int on, int off)>();
        private Dictionary<int, Dictionary<string, int>> _bpnPoorOnPerYear = new Dictionary<int, Dictionary<string, int>>();
        private Dictionary<int, int> _nhsPoorOnPerYear = new Dictionary<int, int>();
        private Dictionary<int, int> _nonNhsPoorOnPerYear = new Dictionary<int, int>();

        // This will be used in Parameters TAB
        private readonly ParametersModel _parametersModel = new ParametersModel();

        public PamsDataForSummaryReport()
        {
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }
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

        private List<string> GetSubHeaders()
        {
            return new List<string>
            {
                "OPI",
                "IRI",
                "Rutting",
                "Faulting",
                "Cracking",
                "Project Source",
                "Budget",
                "Recommended Treatment",
                "Cost",
                "District Remarks"
            };
        }


        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            // Add data to excel.
            reportOutputData.Years.ForEach(_ => _simulationYears.Add(_.Year));
            var currentCell = BuildHeaderAndSubHeaders(worksheet, _simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1]) {
                autoFilterCells.AutoFilter = true;
            }

            FillData(worksheet, reportOutputData, currentCell);
            FillDynamicData(worksheet, reportOutputData, currentCell);

            worksheet.Cells.AutoFitColumns();
            var spacerBeforeFirstYear = _spacerColumnNumbers[0] - 11;
            worksheet.Column(spacerBeforeFirstYear).Width = 3;
            foreach (var spacerNumber in _spacerColumnNumbers)
            {
                worksheet.Column(spacerNumber).Width = 3;
            }
            var lastColumn = worksheet.Dimension.Columns + 1;
            worksheet.Column(lastColumn).Width = 3;

            var workSummaryModel = new WorkSummaryModel
            {
                PreviousYearInitialMinC = _previousYearInitialMinC,
                PoorOnOffCount = _poorOnOffCount,
                ParametersModel = _parametersModel,
                BpnPoorOnPerYear = _bpnPoorOnPerYear,
                NhsPoorOnPerYear = _nhsPoorOnPerYear,
                NonNhsPoorOnPerYear = _nonNhsPoorOnPerYear,
            };

            return workSummaryModel;
        }

        private CurrentCell BuildHeaderAndSubHeaders(ExcelWorksheet worksheet, List<int> simulationYears)
        {
            
            //Get Headers
            var headers = GetHeaders();

            //build header columns
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++) {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
                ExcelHelper.MergeCells(worksheet, headerRow, column + 1, headerRow + 1, column + 1);
            }

            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            BuildDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }

        private void BuildDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
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
            var dataSubHeaders = GetSubHeaders();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = BuildDataSubHeaders(worksheet, column, row, dataSubHeaders, dataSubHeaders.Count - 5);
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
                column = BuildDataSubHeaders(worksheet, column, row, dataSubHeaders, dataSubHeaders.Count);
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

        private int BuildDataSubHeaders(ExcelWorksheet worksheet, int column, int row, List<string> dataSubHeaders, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = dataSubHeaders[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }


        private void FillData(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row; var columnNo = currentCell.Column;
            foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
            {
                rowNo++; columnNo = 1;

                worksheet.Cells[rowNo, columnNo++].Value = ""; //Asset Management Section Column Blank
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DISTRICT");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "CNTY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SR");

                var segmentNumber = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "CNTY");
                segmentNumber += _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SR");
                segmentNumber += sectionSummary.AssetName;
                worksheet.Cells[rowNo, columnNo++].Value = segmentNumber;

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "SEGMENT_LENGTH");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "WIDTH");
                worksheet.Cells[rowNo, columnNo++].Value = ""; // _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DEPTH");

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DIRECTION");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "LANES");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FAMILY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "MPO_RPO");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "SURFACEID").ToString() + "-" + _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SURFACE_NAME");

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BUSIPLAN");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "YR_BUILT");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "YEAR_LAST_OVERLAY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "LAST_STRUCTURAL_OVERLAY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "AADT");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "TRK_PERCENT");
                worksheet.Cells[rowNo, columnNo++].Value = ""; //_summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "ESLAS"); 
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "RISKSCORE"); 

                if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray); }
            }
            currentCell.Row = rowNo; currentCell.Column = columnNo;
        }

        private void FillDynamicData(ExcelWorksheet worksheet, SimulationOutput outputResults, CurrentCell currentCell)
        {
            //initial row to populate data.
            const int initialRow = 4;

            var row = 4; // Data starts here
            var startingRow = row;
            var column = currentCell.Column;

            var workDoneData = new List<int>();
            if (outputResults.Years.Count > 0) { workDoneData = new List<int>(new int[outputResults.Years[0].Assets.Count]); }

            var isInitialYear = true;
            foreach (var yearlySectionData in outputResults.Years)
            {
                row = initialRow;

                // Add work done cells
                TreatmentCause previousYearCause = TreatmentCause.Undefined;
                var previousYearTreatment = PAMSConstants.NoTreatment;
                var i = 0;
                foreach (var section in yearlySectionData.Assets)
                {
                    TrackDataForParametersTAB(section.ValuePerNumericAttribute, section.ValuePerTextAttribute);

                    //bool isNHS = int.TryParse(section.ValuePerTextAttribute["NHS_IND"], out var numericValue) && numericValue > 0;

                    AssetDetail prevYearSection = null;
                    if (section.TreatmentCause == TreatmentCause.CommittedProject && !isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _.AssetName == section.AssetName);
                        previousYearCause = prevYearSection.TreatmentCause;
                        previousYearTreatment = prevYearSection.AppliedTreatment;
                    }

                    // Work done and cost for the given year
                    var treatmentDone = section.AppliedTreatment.ToLower() == PAMSConstants.NoTreatment ? "--" : section.AppliedTreatment;
                    var sumCoveredCost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));

                    worksheet.Cells[row, column].Value = treatmentDone;
                    worksheet.Cells[row, column + 1].Value = sumCoveredCost;
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);

                    if (!treatmentDone.Equals("--")) { workDoneData[i]++; } i++;
                    if (row % 2 == 0) {
                        if (section.TreatmentCause != TreatmentCause.CashFlowProject || section.TreatmentCause == TreatmentCause.CommittedProject) {
                            ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.LightGray);
                        }
                    }
                    ExcelHelper.ApplyLeftBorder(worksheet.Cells[row, column]);
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column + 1]);
                    row++;
                }

                column += 2;
                isInitialYear = false;
            }

            // work done information
            row = 4; // setting row back to start
            column++;
            var totalWorkMoreThanOnce = 0;
            foreach (var wdInfo in workDoneData)
            {
                // Work Done
                worksheet.Cells[row, column - 1].Value = wdInfo >= 1 ? "Yes" : "--";
                // Work done more than once
                worksheet.Cells[row, column].Value = wdInfo > 1 ? "Yes" : "--";
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1, row, column]);
                if (row % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1, row, column + 1], Color.LightGray); }
                row++;
                totalWorkMoreThanOnce += wdInfo > 1 ? 1 : 0;
            }

            // "Total" column
            worksheet.Cells[3, column + 1].Value = totalWorkMoreThanOnce;
            ExcelHelper.ApplyStyle(worksheet.Cells[3, column + 1]);

            row = 4; // setting row back to start
            var initialColumn = column + 1;
            foreach (var intialsection in outputResults.InitialAssetSummaries)
            {
                TrackInitialYearDataForParametersTAB(intialsection);

                column = initialColumn; // This is to reset the column
                column = AddSimulationYearData(worksheet, row, column, intialsection, null);
                row++;
            }

            currentCell.Column = column++;
            currentCell.Row = initialRow;
            isInitialYear = true;
            foreach (var sectionData in outputResults.Years)
            {
                row = currentCell.Row; // setting row back to start
                currentCell.Column = column;
                foreach (var section in sectionData.Assets)
                {
                    column = currentCell.Column;
                    column = AddSimulationYearData(worksheet, row, column, null, section);
                    var initialColumnForShade = column;

                    AssetDetail prevYearSection = null;
                    if (!isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == sectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _.AssetName == section.AssetName);
                    }

                    if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
                    {
                        var cashFlowMap = MappingContent.GetCashFlowProjectPick(section.TreatmentCause, prevYearSection);
                        worksheet.Cells[row, ++column].Value = cashFlowMap.currentPick; //Project Pick
                        worksheet.Cells[row, column - 16].Value = cashFlowMap.previousPick; //Project Pick previous year
                    }
                    else
                    {
                        worksheet.Cells[row, ++column].Value = MappingContent.GetNonCashFlowProjectPick(section.TreatmentCause); //Project Pick
                    }

                    var treatmentConsideration = section.TreatmentConsiderations.FindAll(_ => _.TreatmentName == section.AppliedTreatment);
                    BudgetUsageDetail budgetUsage = null;

                    foreach (var item in treatmentConsideration)
                    {
                        budgetUsage = item.BudgetUsages.Find(_ => _.Status == BudgetUsageStatus.CostCovered);
                    }

                    var budgetName = budgetUsage == null ? "" : budgetUsage.BudgetName;

                    worksheet.Cells[row, ++column].Value = budgetName; // Budget
                    worksheet.Cells[row, ++column].Value = section.AppliedTreatment; // Project
                    var columnForAppliedTreatment = column;

                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    worksheet.Cells[row, ++column].Value = cost; // cost
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);
                    worksheet.Cells[row, ++column].Value = ""; // District Remarks

                    if (row % 2 == 0)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
                    }

                    if (section.TreatmentCause == TreatmentCause.CashFlowProject)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(255, 0, 0));

                        // Color the previous year project also
                        ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment - 16], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment - 16], Color.FromArgb(255, 0, 0));
                    }

                    column = column + 1;
                    row++;
                }
                isInitialYear = false;
            }
        }

        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, AssetSummaryDetail initialSection, AssetDetail section)
        {
            var initialColumnForShade = column + 1;
            var selectedSection = initialSection ?? section;

            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "OPI")));
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "HPMS_IRI")));
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "HPMS_RUTTING")), 3);
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "HPMS_FAULTING")), 3);
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "HPMS_CRACKING")), 1);

            if (row % 2 == 0) {
                ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
            }

            return column;
        }


        private void TrackInitialYearDataForParametersTAB(AssetSummaryDetail initialSection)
        {
            // Get NHS record for Parameter TAB
            if (_parametersModel.nHSModel.NHS == null || _parametersModel.nHSModel.NonNHS == null)
            {
                int.TryParse(initialSection.ValuePerTextAttribute["NHS_IND"], out var numericValue);
                //var numericValue = _summaryReportHelper.checkAndGetValue<int>(initialSection.ValuePerTextAttribute, "NHS_IND");
                if (numericValue > 0)
                {
                    _parametersModel.nHSModel.NHS = "Y";
                    _parametersModel.nHSModel.NonNHS = "N"; //added by bimal
                }
                else
                {
                    _parametersModel.nHSModel.NonNHS = "Y";
                    _parametersModel.nHSModel.NHS = "N"; //added by bimal
                }
            }
            // Get BPN data for parameter TAB
            if (!_parametersModel.BPNValues.Contains(initialSection.ValuePerTextAttribute["BUSIPLAN"]))
            {
                _parametersModel.BPNValues.Add(initialSection.ValuePerTextAttribute["BUSIPLAN"]);
            }
        }

        private void TrackDataForParametersTAB(Dictionary<string, double> valuePerNumericAttribute, Dictionary<string, string> valuePerTextAttribute)
        {
            var structureLength = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "SEGMENT_LENGTH");

            if (structureLength > 20 && _parametersModel.LengthGreaterThan20 != "Y")
            {
                _parametersModel.LengthGreaterThan20 = "Y";
            }
            if (structureLength >= 8 && structureLength <= 20 && _parametersModel.LengthBetween8and20 != "Y")
            {
                _parametersModel.LengthBetween8and20 = "Y";
            }
        }
    }
}
