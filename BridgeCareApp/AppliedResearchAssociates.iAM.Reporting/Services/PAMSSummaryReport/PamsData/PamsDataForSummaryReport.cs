using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static System.Collections.Specialized.BitVector32;

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

        private static List<string> GetHeaders() => new()
        {
                "CRS",
                "County",
                "Route",
                "District",
                "Start",
                "End",
                "Length(ft)",
                "Width(ft)",
                "Pavement Depth(in)",
                "Direction",
                "Lanes",
                "FamilyID",
                "MPO/ RPO",
                "Posted Roads",
                "Surface Type",
                "BPN",
                "Year Built",
                "Year Last Resurface",
                "Year Last  Structural Overlay",
                "ADT",
                "Truck %",
                "Risk Score",
            };

        private List<string> GetSubHeaders()
        {
            return new List<string>
            {
                "OPI",
                "IRI",
                "Rut",
                "Fault",
                "Project Source",
                "Project Id",
                "Budget",
                "Recommended Treatment",
                "Cost",
                "Superseded Treatments",
                "Comments"
            };
        }


        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, bool shouldBundleFeasibleTreatments, List<DTOs.Abstract.BaseCommittedProjectDTO> committedProjectList)
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
            FillDynamicData(worksheet, reportOutputData, currentCell, shouldBundleFeasibleTreatments, committedProjectList);
            worksheet.Cells.AutoFitColumns();

            const double minimumColumnWidth = 15;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (worksheet.Column(col).Width < minimumColumnWidth)
                {
                    worksheet.Column(col).Width = minimumColumnWidth;
                }
            }

            foreach (var spacerNumber in _spacerColumnNumbers)
            {
                bool isColumnEmpty = true; 
                for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                {
                    if (worksheet.Cells[row, spacerNumber].Value != null)
                    {
                        isColumnEmpty = false;
                        break; 
                    }
                }
                if (isColumnEmpty)
                {
                    worksheet.Column(spacerNumber).Width = 3;
                }
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
            worksheet.Cells[headerRow, 1, headerRow + 1, headers.Count].AutoFitColumns();
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

            worksheet.Cells[row, ++column].Value = "Work Planned";
            // Merge "Work Planned" header with the cell below it
            ExcelHelper.MergeCells(worksheet, row, column, row + 1, column);

            ExcelHelper.ApplyStyle(worksheet.Cells[row, column-2, row, column]);
            
            worksheet.Row(row).Height = 40;            
            currentCell.Column = column;

            // Add Years Data headers
            var dataSubHeaders = GetSubHeaders();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = BuildDataSubHeaders(worksheet, column, row, dataSubHeaders, dataSubHeaders.Count - 6);
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
            worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].AutoFitColumns();
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

                var valuePerNumericAttribute = sectionSummary.ValuePerNumericAttribute;
                var valuePerTextAttribute = sectionSummary.ValuePerTextAttribute;

                var crs = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "CRS");
                worksheet.Cells[rowNo, columnNo++].Value = crs;
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "COUNTY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "SR");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "DISTRICT");
                var lastUnderScoreIndex = crs.LastIndexOf('_');
                var hyphenIndex = crs.IndexOf('-');
                var startSeg = crs.Substring(lastUnderScoreIndex + 1, hyphenIndex - lastUnderScoreIndex - 1);
                var endSeg = crs.Substring(hyphenIndex + 1);
                worksheet.Cells[rowNo, columnNo++].Value = startSeg;
                worksheet.Cells[rowNo, columnNo++].Value = endSeg;
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "SEGMENT_LENGTH");
                worksheet.Cells[rowNo, columnNo].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "WIDTH");
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo++], ExcelHelperCellFormat.Number);
                worksheet.Cells[rowNo, columnNo].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "PAVED_THICKNESS");
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo++], ExcelHelperCellFormat.DecimalPrecision2);
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "DIRECTION");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "LANES");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "FAMILY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "MPO_RPO");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "POSTED");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "SURFACEID").ToString() + "-" + _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "SURFACE_NAME");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "BUSIPLAN");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "YR_BUILT");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "YEAR_LAST_OVERLAY");
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "LAST_STRUCTURAL_OVERLAY");
                worksheet.Cells[rowNo, columnNo++].Value = Math.Round(_summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "AADT"));
                worksheet.Cells[rowNo, columnNo++].Value = Math.Round(_summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "TRK_PERCENT"));
                worksheet.Cells[rowNo, columnNo++].Value = Math.Round(_summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "RISKSCORE"));


                if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray); }
            }
            currentCell.Row = rowNo; currentCell.Column = columnNo;
        }

        private void FillDynamicData(ExcelWorksheet worksheet, SimulationOutput outputResults, CurrentCell currentCell, bool shouldBundleFeasibleTreatments, List<BaseCommittedProjectDTO> committedProjectList)
        {
            //initial row to populate data.
            const int initialRow = 4;

            var row = 4; // Data starts here
            var startingRow = row;
            var column = currentCell.Column;

            var workDoneData = new List<int>();
            if (outputResults.Years.Count > 0) { workDoneData = new List<int>(new int[outputResults.Years[0].Assets.Count]); }

            var isInitialYear = true;
            var lastYear = outputResults.Years.Last().Year;
            Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
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

                    AssetDetail prevYearSection = null;
                    if (section.TreatmentCause == TreatmentCause.CommittedProject && !isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _.AssetName == section.AssetName);
                        previousYearCause = prevYearSection.TreatmentCause;
                        previousYearTreatment = prevYearSection.AppliedTreatment;
                    }

                    CheckConditions(section.AppliedTreatment, previousYearTreatment, previousYearCause, section.TreatmentCause, worksheet, row, column);

                    // Work done and cost for the given year                    
                    var crs = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CRS");
                    // Build keyCashFlowFundingDetails
                    _summaryReportHelper.BuildKeyCashFlowFundingDetails(yearlySectionData, section, crs, keyCashFlowFundingDetails);

                    // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                    var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                  keyCashFlowFundingDetails[crs] :
                                                  section.TreatmentConsiderations ?? new();

                    var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                            section.AppliedTreatment.Contains(_.TreatmentName)) :
                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                            _.TreatmentName == section.AppliedTreatment);

                    var treatmentName = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                    var treatmentDone = section.AppliedTreatment.ToLower() == PAMSConstants.NoTreatment ? "--" : treatmentName;
                    worksheet.Cells[row, column].Value = treatmentDone;

                    var allocationMatrix = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix ?? new();
                    var sumCoveredCost = Math.Round(allocationMatrix?.Where(_ => _.Year == yearlySectionData.Year).Sum(_ => _.AllocatedAmount) ?? 0, 0);
                    worksheet.Cells[row, column + 1].Value = sumCoveredCost;
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);

                    if (!treatmentDone.Equals("--"))
                    {
                        workDoneData[i]++;
                    }
                    i++;

                    if (row % 2 == 0) {
                        if (section.TreatmentCause != TreatmentCause.CashFlowProject &&
                            !(section.TreatmentCause == TreatmentCause.CommittedProject && previousYearCause == TreatmentCause.CommittedProject && previousYearTreatment.ToLower() != PAMSConstants.NoTreatment))
                        {
                            ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.LightGray);
                        }
                    }

                    // Cash flow coloring
                    if (section.TreatmentCause == TreatmentCause.CashFlowProject)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(255, 0, 0));

                        // Color the previous year project also
                        ExcelHelper.ApplyColor(worksheet.Cells[row, column - 2, row, column - 1], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, column - 2, row, column - 1], Color.FromArgb(255, 0, 0));
                    }
                    if (yearlySectionData.Year == lastYear &&
                        section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                        section.TreatmentStatus == TreatmentStatus.Progressed)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(255, 0, 0));
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

            foreach (var wdInfo in workDoneData)
            {
                // Work Done
                worksheet.Cells[row, column].Value = wdInfo >= 1 ? "Yes" : "--";
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column]);
                if (row % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[row, column], Color.LightGray); }
                row++;
            }

          
            row = 4; // setting row back to start
            var initialColumn = column;            
            _parametersModel.nHSModel.NHS = "N";
            _parametersModel.nHSModel.NonNHS = "N";
            foreach (var initialSection in outputResults.InitialAssetSummaries)
            {
                TrackInitialYearDataForParametersTAB(initialSection);

                column = initialColumn; // Reset the column to the starting point for new data
                column = AddSimulationYearData(worksheet, row, column, initialSection, null);
                row++;
            }

            int columnsToSubtract = 12;
            currentCell.Column = column++;
            currentCell.Row = initialRow;
            isInitialYear = true;
            foreach (var yearlySectionData in outputResults.Years)
            {
                row = currentCell.Row; // setting row back to start
                currentCell.Column = column;
                foreach (var section in yearlySectionData.Assets)
                {
                    column = currentCell.Column;
                    column = isInitialYear ?
                             AddSimulationYearData(worksheet, row, column, null, section, true)
                             : AddSimulationYearData(worksheet, row, column, null, section);
                    var initialColumnForShade = column;
                    var crs = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CRS");

                    AssetDetail prevYearSection = null;
                    if (!isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _.AssetName == section.AssetName);
                    }

                    if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
                    {
                        var cashFlowMap = MappingContent.GetCashFlowProjectPick(section.TreatmentCause, prevYearSection);
                        worksheet.Cells[row, ++column].Value = cashFlowMap.currentPick; //Project Pick
                        worksheet.Cells[row, column - columnsToSubtract].Value = cashFlowMap.previousPick; //Project Pick previous year
                        column++;
                    }
                    else
                    {
                        // Add Project Source
                        var committedProject = committedProjectList.FirstOrDefault(_ => section.AppliedTreatment.Contains(_.Treatment) && _.Year == yearlySectionData.Year && _.LocationKeys["CRS"] == crs.ToString());
                        var projectSource = committedProject?.ProjectSource.ToString() ?? string.Empty;
                        worksheet.Cells[row, ++column].Value = MappingContent.GetNonCashFlowProjectPick(section.TreatmentCause, projectSource); //Project Pick

                        // Add Project Id
                        var projectId = committedProject?.ProjectId?.ToString() ?? string.Empty;
                        worksheet.Cells[row, ++column].Value = projectId;
                    }

                    // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                    var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                  keyCashFlowFundingDetails[crs] :
                                                  section.TreatmentConsiderations ?? new();

                    var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                            section.AppliedTreatment.Contains(_.TreatmentName)) :
                                         treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                            _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                            _.TreatmentName == section.AppliedTreatment);

                    var allocationMatrix = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix ?? new();
                    var budgetNames = allocationMatrix?.Where(_ => _.AllocatedAmount > 0 && _.Year == yearlySectionData.Year).
                                      Select(_ => _.BudgetName).Distinct().ToList() ?? new();
                    var budgetName = string.Join(",", budgetNames);
                    worksheet.Cells[row, ++column].Value = budgetName; // Budget
                    var project = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                    worksheet.Cells[row, ++column].Value = project;
                    var columnForAppliedTreatment = column;

                    var cost = Math.Round(allocationMatrix?.Where(_ => _.Year == yearlySectionData.Year).Sum(_ => _.AllocatedAmount) ?? 0, 0);
                    worksheet.Cells[row, ++column].Value = cost; // cost
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);

                    // Superseded Treatments
                    var supersededTreatments = section.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment ?
                                               section.TreatmentRejections.
                                               Where(_ => _.TreatmentRejectionReason == TreatmentRejectionReason.Superseded).
                                               Select(_ => _.TreatmentName).Distinct().ToList() ?? new() :
                                               new();
                    worksheet.Cells[row, ++column].Value = supersededTreatments.Count > 0 ?
                                                           string.Join(", ", supersededTreatments) :
                                                           string.Empty;

                    worksheet.Cells[row, ++column].Value = ""; // Comments

                    if (row % 2 == 0)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
                    }

                    if (section.TreatmentCause == TreatmentCause.CashFlowProject)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment], Color.FromArgb(255, 0, 0));

                        // Color the previous year project also
                        ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment - columnsToSubtract], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment - columnsToSubtract], Color.FromArgb(255, 0, 0));
                    }

                    column = column + 1;
                    row++;
                }
                isInitialYear = false;
            }
        }

        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, AssetSummaryDetail initialSection, AssetDetail section, bool updateColumn = false)
        {
            if(updateColumn)
            {
                column++;
            }
            var initialColumnForShade = column + 1;
            var selectedSection = initialSection ?? section;
            var averageRutting = CalculateRuttingBasedOnSurfaceType(selectedSection);
                        
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "OPI_CALCULATED")));
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, PAMSAuditReportConstants.IRI)));
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(averageRutting), 3);
            worksheet.Cells[row, ++column].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, PAMSAuditReportConstants.FAULT)), 3);

            if (row % 2 == 0) {
                var toColumn = section == null ? column + 1 : column;
                ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, toColumn], Color.LightGray);
            }

            return column;
        }

        private double CalculateRuttingBasedOnSurfaceType(AssetSummaryDetail selectedSection)
        {
            SurfaceType surfaceType = GetSurfaceType(selectedSection);

            if (surfaceType == SurfaceType.Asphalt)
            {
                return CalculateAverageRutting(PAMSAuditReportConstants.RUT_LEFT, PAMSAuditReportConstants.RUT_RIGHT, selectedSection);
            }
            else if (surfaceType == SurfaceType.Concrete)
            {
                return CalculateAverageRutting("CRJCPRU_Total", "CLJCPRU_Total", selectedSection);
            }
            return 0;
        }

        private double CalculateAverageRutting(string firstAttributeName, string secondAttributeName, AssetSummaryDetail selectedSection)
        {
            double firstValue = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, firstAttributeName);
            double secondValue = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, secondAttributeName);
            return (firstValue + secondValue) / 2;
        }

        private SurfaceType GetSurfaceType(AssetSummaryDetail selectedSection)
        {
            int surfaceId = Convert.ToInt32(_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SURFACEID"));
            return surfaceId > 62 ? SurfaceType.Concrete : SurfaceType.Asphalt;
        }

        enum SurfaceType
        {
            Asphalt,
            Concrete
        }

        /// <summary>
        /// If any section has NHS_IND value other than "N" then NHS should be Y
        /// If any section has NHS_IND value "N" then NON-NHS should be Y
        /// </summary>
        /// <param name="initialSection"></param>
        private void TrackInitialYearDataForParametersTAB(AssetSummaryDetail initialSection)
        {
            // Get NHS record for Parameter TAB            
            var nhsInd = _summaryReportHelper.checkAndGetValue<string>(initialSection.ValuePerTextAttribute, "NHS_IND");            
            if (nhsInd != "N")
            {
                _parametersModel.nHSModel.NHS = "Y";
            }
            else
            {
                _parametersModel.nHSModel.NonNHS = "Y";
            }

            // Get BPN data for parameter TAB
            var bpn = initialSection.ValuePerTextAttribute["BUSIPLAN"];
            if (!_parametersModel.BPNValues.Contains(bpn))
            {
                _parametersModel.BPNValues.Add(bpn);
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

        public void CheckConditions(string treatment, string previousYearTreatment, TreatmentCause previousYearCause,
            TreatmentCause treatmentCause, ExcelWorksheet worksheet, int row, int column)
        {
            if (treatment != null && treatment.ToLower() != PAMSConstants.NoTreatment)
            {
                var range = worksheet.Cells[row, column, row, column + 1];
                
                if (treatmentCause == TreatmentCause.CommittedProject
                    && previousYearCause == TreatmentCause.CommittedProject
                    && previousYearTreatment.ToLower() != PAMSConstants.NoTreatment)
                {
                    var rangeWithPreviousColumn = worksheet.Cells[row, column - 2, row, column - 1];
                    CommittedForConsecutiveYears(rangeWithPreviousColumn);
                    CommittedForConsecutiveYears(range);
                }
            }
        }

        private void CommittedForConsecutiveYears(ExcelRange range)
        {
            ExcelHelper.ApplyColor(range, Color.FromArgb(255, 153, 0));
            ExcelHelper.SetTextColor(range, Color.White);
        }

        
    }
}
