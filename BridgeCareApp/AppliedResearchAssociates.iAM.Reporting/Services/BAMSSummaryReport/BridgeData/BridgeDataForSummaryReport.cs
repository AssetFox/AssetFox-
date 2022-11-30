﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Utilities.Encoders;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData
{
    public class BridgeDataForSummaryReport : IBridgeDataForSummaryReport
    {
        private List<int> _spacerColumnNumbers;
        private IHighlightWorkDoneCells _highlightWorkDoneCells;
        private Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>> _valueForMinC;
        private readonly List<int> _simulationYears = new List<int>();
        private ISummaryReportHelper _summaryReportHelper;

        // This is also used in Bridge Work Summary TAB
        private readonly List<double> _previousYearInitialMinC = new List<double>();
        private Dictionary<int, (int on, int off)> _poorOnOffCount = new Dictionary<int, (int on, int off)>();
        private Dictionary<int, Dictionary<string, int>> _bpnPoorOnPerYear = new Dictionary<int, Dictionary<string, int>>();
        private Dictionary<int, int> _nhsPoorOnPerYear = new Dictionary<int, int>();
        private Dictionary<int, int> _nonNhsPoorOnPerYear = new Dictionary<int, int>();

        // This will be used in Parameters TAB
        private readonly ParametersModel _parametersModel = new ParametersModel();

        public BridgeDataForSummaryReport()
        {
            _highlightWorkDoneCells = new HighlightWorkDoneCells();
            _summaryReportHelper = new SummaryReportHelper();
        }

        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            // Add data to excel.
            var sectionHeaders = GetStaticSectionHeaders();
            var dataHeaders = GetStaticDataHeaders();
            var subHeaders = GetStaticSubHeaders();

            reportOutputData.Years.ForEach(_ => _simulationYears.Add(_.Year));
            var currentCell = AddHeadersCells(worksheet, sectionHeaders, dataHeaders, subHeaders, _simulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            AddBridgeDataModelsCells(worksheet, reportOutputData, currentCell);
            AddDynamicDataCells(worksheet, reportOutputData, currentCell);

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

        #region Private Methods

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput outputResults, CurrentCell currentCell)
        {
            var initialRow = 5;
            var row = initialRow; // Data starts here
            var startingRow = row;
            var column = currentCell.Column;
            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();

            // making dictionary to remove if else, which was used to enter value for MinC
            _valueForMinC = new Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>>();
            _valueForMinC.Add(MinCValue.defaultValue, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterDefaultMinCValue));
            _valueForMinC.Add(MinCValue.valueEqualsCulv, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterValueEqualsCulv));
            _valueForMinC.Add(MinCValue.minOfDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSub));
            _valueForMinC.Add(MinCValue.minOfCulvDeckSubSuper, new Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>(EnterMinDeckSuperSubCulv));

            var workDoneData = new List<int>();
            var previousYearSectionMinC = new List<double>();
            if (outputResults.Years.Count > 0)
            {
                workDoneData = new List<int>(new int[outputResults.Years[0].Assets.Count]);
                previousYearSectionMinC = new List<double>(new double[outputResults.Years[0].Assets.Count]);
            }
            var poorOnOffColumnStart = (outputResults.Years.Count * 2) + column + 3;
            var index = 1; // to track the initial section from rest of the years

            var isInitialYear = true;
            foreach (var yearlySectionData in outputResults.Years)
            {
                _poorOnOffCount.Add(yearlySectionData.Year, (on: 0, off: 0));

                row = initialRow;

                // Add work done cells
                TreatmentCause previousYearCause = TreatmentCause.Undefined;
                var previousYearTreatment = BAMSConstants.NoTreatment;
                var i = 0; double section_BRKEY = 0;
                foreach (var section in yearlySectionData.Assets)
                {
                    TrackDataForParametersTAB(section.ValuePerNumericAttribute, section.ValuePerTextAttribute);

                    //get unique key (brkey) to compare
                    section_BRKEY = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

                    if (!_bpnPoorOnPerYear.ContainsKey(yearlySectionData.Year))
                    {
                        _bpnPoorOnPerYear.Add(yearlySectionData.Year, new Dictionary<string, int>());
                    }

                    if (!_nhsPoorOnPerYear.ContainsKey(yearlySectionData.Year))
                    {
                        _nhsPoorOnPerYear.Add(yearlySectionData.Year, 0);
                    }

                    if (!_nonNhsPoorOnPerYear.ContainsKey(yearlySectionData.Year))
                    {
                        _nonNhsPoorOnPerYear.Add(yearlySectionData.Year, 0);
                    }

                    bool isNHS = int.TryParse(_summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND"), out var numericValue) && numericValue > 0;

                    int nhsOrNonPoorOnCount = isNHS ? _nhsPoorOnPerYear[yearlySectionData.Year] : _nonNhsPoorOnPerYear[yearlySectionData.Year];

                    Dictionary<string, int> bpnPoorOnDictionary = _bpnPoorOnPerYear[yearlySectionData.Year];
                    var busPlanNetwork = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
                    int bpnPoorOnCount;
                    // Create/Update BPN info for this Section/Year
                    if (!bpnPoorOnDictionary.ContainsKey(busPlanNetwork))
                    {
                        bpnPoorOnCount = 0;
                        bpnPoorOnDictionary.Add(busPlanNetwork, bpnPoorOnCount);
                    }
                    else
                    {
                        bpnPoorOnCount = bpnPoorOnDictionary[busPlanNetwork];
                    }

                    var thisYrMinc = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "MINCOND");
                    // poor on off Rate
                    var prevYrMinc = 0.0;
                    if (index == 1)
                    {
                        prevYrMinc = _previousYearInitialMinC[i];
                    }
                    else
                    {
                        prevYrMinc = previousYearSectionMinC[i];
                    }
                    AssetDetail prevYearSection = null;
                    if (section.TreatmentCause == TreatmentCause.CommittedProject && !isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY); 
                        previousYearCause = prevYearSection.TreatmentCause;
                        previousYearTreatment = prevYearSection.AppliedTreatment;
                    }
                    setColor((int)_summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "PARALLEL"), section.AppliedTreatment, previousYearTreatment, previousYearCause, section.TreatmentCause,
                        yearlySectionData.Year, index, worksheet, row, column);

                    // Work done in a year
                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    var range = worksheet.Cells[row, column];
                    if (abbreviatedTreatmentNames.ContainsKey(section.AppliedTreatment))
                    {
                        range.Value = abbreviatedTreatmentNames[section.AppliedTreatment];
                        worksheet.Cells[row, column + 1].Value = cost;


                        if (!isInitialYear && section.TreatmentCause == TreatmentCause.CashFlowProject)
                        {
                            if (prevYearSection == null)
                            {
                                prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                                    .Assets.FirstOrDefault(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY);
                            }
                            if (prevYearSection.AppliedTreatment == section.AppliedTreatment)
                            {
                                range.Value = "--";
                                worksheet.Cells[row, column + 1].Value = cost;
                            }
                        }
                        worksheet.Cells[row, column + 1].Value = cost;
                        ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);

                    }
                    else
                    {
                        range.Value = section.AppliedTreatment.ToLower() == BAMSConstants.NoTreatment ? "--" :
                            section.AppliedTreatment.ToLower();

                        worksheet.Cells[row, column + 1].Value = cost;
                        ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1]);
                    }
                    if (!range.Value.Equals("--"))
                    {
                        workDoneData[i]++;
                    }

                    worksheet.Cells[row, poorOnOffColumnStart].Value = prevYrMinc < 5 ? (thisYrMinc >= 5 ? "Off" : "--") :
                        (thisYrMinc < 5 ? "On" : "--");

                    var onOffCount = _poorOnOffCount[yearlySectionData.Year];
                    if (worksheet.Cells[row, poorOnOffColumnStart].Value.ToString() == "On")
                    {
                        onOffCount.on += 1;
                        bpnPoorOnCount += 1;
                        nhsOrNonPoorOnCount += 1;
                    }
                    else if(worksheet.Cells[row, poorOnOffColumnStart].Value.ToString() == "Off")
                    {
                        onOffCount.off += 1;
                    }
                    _poorOnOffCount[yearlySectionData.Year] = onOffCount;
                    bpnPoorOnDictionary[busPlanNetwork] = bpnPoorOnCount;
                    if (isNHS)
                    {
                        _nhsPoorOnPerYear[yearlySectionData.Year] = nhsOrNonPoorOnCount;
                    }
                    else
                    {
                        _nonNhsPoorOnPerYear[yearlySectionData.Year] = nhsOrNonPoorOnCount;
                    }

                    previousYearSectionMinC[i] = thisYrMinc;
                    i++;
                    if (row % 2 == 0)
                    {
                        if(section.TreatmentCause != TreatmentCause.CashFlowProject ||
                            section.TreatmentCause == TreatmentCause.CommittedProject)
                        {
                            ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.LightGray);
                        }
                        ExcelHelper.ApplyColor(worksheet.Cells[row, poorOnOffColumnStart], Color.LightGray);
                    }
                    ExcelHelper.ApplyLeftBorder(worksheet.Cells[row, column]);
                    ExcelHelper.ApplyRightBorder(worksheet.Cells[row, column + 1]);
                    row++;
                }
                index++;

                poorOnOffColumnStart++;
                column += 2;
                isInitialYear = false;
            }

            // work done information
            row = initialRow; // setting row back to start
            column++;
            var totalWorkMoreThanOnce = 0;
            foreach (var wdInfo in workDoneData)
            {
                // Work Done
                worksheet.Cells[row, column - 1].Value = wdInfo >= 1 ? "Yes" : "--";
                // Work done more than once
                worksheet.Cells[row, column].Value = wdInfo > 1 ? "Yes" : "--";
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1, row, column]);
                if (row % 2 == 0)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1, row, column + 1], Color.LightGray);
                }
                row++;
                totalWorkMoreThanOnce += wdInfo > 1 ? 1 : 0;
            }

            // "Total" column
            worksheet.Cells[initialRow - 1, column + 1].Value = totalWorkMoreThanOnce;
            ExcelHelper.ApplyStyle(worksheet.Cells[initialRow - 1, column + 1]);

            column = column + outputResults.Years.Count + 2; // this will take us to the empty column after "poor on off"
            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            row = initialRow; // setting row back to start
            var initialColumn = column;
            foreach (var intialsection in outputResults.InitialAssetSummaries)
            {
                TrackInitialYearDataForParametersTAB(intialsection);
                column = initialColumn; // This is to reset the column
                column = AddSimulationYearData(worksheet, row, column, intialsection, null);
                row++; column++;
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

                    //get unique key (brkey) to compare
                    var section_BRKEY = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

                    AssetDetail prevYearSection = null;
                    if (!isInitialYear)
                    {
                        prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == sectionData.Year - 1)
                            .Assets.FirstOrDefault(_ => _summaryReportHelper.checkAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY);
                    }

                    if(section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
                    {
                        var cashFlowMap = MappingContent.GetCashFlowProjectPick(section.TreatmentCause, prevYearSection);
                        worksheet.Cells[row, ++column].Value = cashFlowMap.currentPick; //Project Pick
                        worksheet.Cells[row, ++column].Value = ""; // TODO: Posted
                        worksheet.Cells[row, column - 17].Value = cashFlowMap.previousPick; //Project Pick previous year
                    }
                    else
                    {
                        worksheet.Cells[row, ++column].Value = ""; // TODO: Posted
                        worksheet.Cells[row, ++column].Value = MappingContent.GetNonCashFlowProjectPick(section.TreatmentCause);//Project Pick
                    }

                    var treatmentConsideration = section.TreatmentConsiderations.FindAll(_ => _.TreatmentName == section.AppliedTreatment);
                    BudgetUsageDetail budgetUsage = null;

                    foreach (var item in treatmentConsideration)
                    {
                        budgetUsage = item.BudgetUsages.Find(_ => _.Status == BudgetUsageStatus.CostCoveredInFull ||
                    _.Status == BudgetUsageStatus.CostCoveredInPart);
                    }

                    var budgetName = budgetUsage == null ? "" : budgetUsage.BudgetName;

                    worksheet.Cells[row, ++column].Value = budgetName; // Budget
                    worksheet.Cells[row, ++column].Value = section.AppliedTreatment; // Recommended Treatment
                    var columnForAppliedTreatment = column;

                    var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                    worksheet.Cells[row, ++column].Value = cost; // cost
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column]);

                    worksheet.Cells[row, ++column].Value = ""; // TODO: FHWA Work Types

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
            var minCActionCallDecider = MinCValue.minOfCulvDeckSubSuper;
            int.TryParse(_summaryReportHelper.checkAndGetValue<string>(selectedSection.ValuePerTextAttribute, "FAMILY_ID"), out var familyId);
            var familyIdLessThanEleven = familyId < 11;
            if (familyId > 10)
            {
                var columnForStyle = column + 1;
                worksheet.Cells[row, ++column].Value = "N"; // deck cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);

                worksheet.Cells[row, ++column].Value = "N"; // super cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);

                worksheet.Cells[row, ++column].Value = "N"; // sub cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);

                worksheet.Cells[row, column + 2].Value = "N"; // deck dur
                worksheet.Cells[row, column + 3].Value = "N"; // super dur
                worksheet.Cells[row, column + 4].Value = "N"; // sub dur
                minCActionCallDecider = MinCValue.valueEqualsCulv;
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnForStyle, row, column + 4]);
            }
            else
            {
                worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "DECK_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);

                worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUP_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);

                worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUB_SEEDED");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 1]);
                ExcelHelper.SetCustomFormat(worksheet.Cells[row, column - 2, row, column], ExcelHelperCellFormat.DecimalPrecision3);

                worksheet.Cells[row, column + 2].Value = (int)_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "DECK_DURATION_N");
                worksheet.Cells[row, column + 3].Value = (int)_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUP_DURATION_N");
                worksheet.Cells[row, column + 4].Value = (int)_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUB_DURATION_N");
            }
            if (familyIdLessThanEleven)
            {
                worksheet.Cells[row, ++column].Value = "N"; // culv cond
                worksheet.Cells[row, column + 4].Value = "N"; // culv seeded
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column, row, column + 4]);
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
                worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "CULV_SEEDED");
                ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);

                worksheet.Cells[row, column + 4].Value = (int)_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "CULV_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column + 4]);
            }
            column += 4; // this will take us to "Min cond" column

            // It returns the column number where MinC value is written
            column = _valueForMinC[minCActionCallDecider](worksheet, row, column, selectedSection.ValuePerNumericAttribute);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
            var minCondColumn = column;

            if (_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "P3") > 0 && _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") < 5)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, column], Color.Yellow);
                ExcelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
            }
            worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") < 5 ? "Y" : "N"; //poor
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column]);

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
            }

            // Setting color of MinCond over here, to avoid Color.LightGray overriding it
            if (_summaryReportHelper.checkAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") <= 3.5)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, minCondColumn], Color.FromArgb(112, 48, 160));
                ExcelHelper.SetTextColor(worksheet.Cells[row, minCondColumn], Color.White);
            }

            if(initialSection != null) {
                worksheet.Cells[row, column++].Value = ""; // TODO: Posted for initial year
            }

            return column;
        }

        private void AddBridgeDataModelsCells(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;
            foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
            {
                rowNo++; columnNo = 1;

                //--------------------- Asset ID ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BRIDGE_TYPE"); //Internet Report

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERNET_REPORT"); //Bridge (B/C)
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BMSID"); //Bridge ID

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "BRKEY_"); //BRKey
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                //--------------------- Ownership ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DISTRICT"); //District
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY"); //County

                var ownerName = ""; var ownerCode = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
                if (!string.IsNullOrEmpty(ownerCode) && !string.IsNullOrWhiteSpace(ownerCode)) { ownerName = MappingContent.OwnerCodeForReport(ownerCode); }
                worksheet.Cells[rowNo, columnNo++].Value = ownerName;

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SUBM_AGENCY"); //Submitting Agency
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "MPO_NAME"); // Planning Partner

                //--------------------- Structure ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "LENGTH"); //Structure Length
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DECK_AREA"); //Deck Area
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "LARGE_BRIDGE"); // Large Bridge
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                var spanType = ""; var spanTypeName = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SPANTYPE"); //Span Type
                if (!string.IsNullOrEmpty(spanTypeName) && !string.IsNullOrWhiteSpace(spanTypeName)) {
                    spanType = spanTypeName == SpanType.M.ToSpanTypeName() ? MappingContent.SpanTypeMap[SpanType.M] : MappingContent.SpanTypeMap[SpanType.S];
                }
                worksheet.Cells[rowNo, columnNo++].Value = spanType;
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FAMILY_ID"); //Bridge Family
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "STRUCTURE_TYPE"); //Structure Type

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FRACT_CRIT"); //Fractural Critical
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "PARALLEL") > 0 ? "Y" : "N"; //Parallel Structure
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                //--------------------- Network ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.FullFunctionalClassDescription(_summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FUNC_CLASS")); //Functional Class

                worksheet.Cells[rowNo, columnNo++].Value = int.TryParse(_summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NHS_IND"), out var numericValue) && numericValue > 0 ? "Y" : "N"; //NHS
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NBISLEN"); //NBIS Len
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BUS_PLAN_NETWORK"); //BPN

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERSTATE"); //Interstate
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                //--------------------- Asset Attibutes ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = MappingContent.GetDeckSurfaceType(_summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DECKSURF_TYPE")); //Deck Surface Type

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "WS_SEEDED"); // Wearing Surface Cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_COND"); //Paint Cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_EXTENT"); //Paint Ext
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = (int)_summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "YEAR_BUILT"); //Year Built
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "AGE"); //Age
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "ADTTOTAL"); //ADT
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "RISK_SCORE"); //Risk Score

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DET_LENGTH"); //Detour Length

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "POST_STATUS"); //Posting Status
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "SUFF_RATING"); //Suff Rating


                //--------------------- Funding ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "HBRR_ELIG"); //HBRR Elig
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "P3") > 0 ? "Y" : "N"; //P3
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FEDAID"); //Federal Aid

                _previousYearInitialMinC.Add(_summaryReportHelper.checkAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "MINCOND"));

                // Bridge Funding
                var columnForStyle = columnNo;
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingNHPP(sectionSummary) ? "Y" : "N"; //NHPP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingSTP(sectionSummary) ? "Y" : "N"; //STP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingBOF(sectionSummary) ? "Y" : "N"; //BOF

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFunding185(sectionSummary) ? "Y" : "N"; //BRIP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFunding581(sectionSummary) ? "Y" : "N"; //State
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFunding183(sectionSummary) ? "Y" : "N"; //NA

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnForStyle, rowNo, columnNo - 1]);

                if (rowNo % 2 == 0)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray);
                }
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        private void setColor(int parallelBridge, string treatment, string previousYearTreatment, TreatmentCause previousYearCause,
           TreatmentCause treatmentCause, int year, int index, ExcelWorksheet worksheet, int row, int column)
        {
            _highlightWorkDoneCells.CheckConditions(parallelBridge, treatment, previousYearTreatment, previousYearCause, treatmentCause, year, index, worksheet, row, column);
        }


        private List<string> GetStaticSectionHeaders()
        {
            return new List<string>
            {
                "Asset ID",
                "Ownership",
                "Structure",
                "Network",
                "Asset Attributes",
                "Funding",
            };
        }

        private List<string> GetStaticDataHeaders()
        {
            return new List<string>
            {
                //--------------------- Asset ID ---------------------
                "Internet\r\nReport",
                "Bridge\r\n(B/C)",
                "BridgeID\r\n(5A01)",
                "BRKey\r\n(5A03)",

                //--------------------- Ownership ---------------------
                "District\r\n(5A04)",
                "County\r\n(5A05)",
                "Owner Code\r\n(5A21)",
                "Submitting\r\nAgency\r\n(6A06)",
                "Planning Partner\r\n(5A13)",

                //--------------------- Structure ---------------------
                "Structure\r\nLength\r\n(5B18)",
                "Deck Area\r\n(5B19)",
                "Large\r\nBridge",
                "Span\r\nType",
                "Bridge\r\nFamily",
                "Struct Type\r\n(6A26-29)",
                "Fractural\r\nCritical",
                "Parallel\r\nStructure\r\n(5E02)",

                //--------------------- Network ---------------------

                "Functional Class\r\n(5C22)",
                "NHS\r\n(5C29)",
                "NBIS Len\r\n(5E01)",
                "BPN\r\n(6A19)",
                "Interstate",

                //--------------------- Asset Attributes ---------------------
                
                "Deck Surface\r\nType\r\n(5B02)",
                "Wearing\r\nSurface\r\nCond\r\n(6B40)",
                "Paint\r\nCond\r\n(6B36)",
                "Paint Ext\r\n(6B37)",
                "Year Built\r\n(5A15)",
                "Age",
                "ADTT\r\n(5C10)",
                "Risk Score",
                "Detour Length\r\n(5C15)",
                "Posting Status\r\n(VP02)",
                "Suff Rating\r\n(4A13)",

                //--------------------- Funding ---------------------
                "HBRR Elig\r\n(6B41)",
                "P3",
                "Federal Aid\r\n(6C06)",
                "Bridge Funding"
            };
        }

        private List<string> GetStaticSubHeaders()
        {
            return new List<string>
            {
                "NHPP",
                "STP",
                "BOF",
                "BRIP",
                "State",
                "N/A"
            };
        }


        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> sectionHeaders, List<string> dataHeaders, List<string> subHeaders, List<int> simulationYears)
        {
            //section header
            int sectionHeaderRow = 1; var totalNumOfColumns = 0; var cellBGColor = ColorTranslator.FromHtml("#FFFFFF"); //White
            var startColumn = 0; var endColumn = 0;
            for (int column = 0; column < sectionHeaders.Count; column++)
            {
                var headerName = sectionHeaders[column];
                var headerNameFormatted = headerName.ToUpper().Replace(" ", "_");                                
                switch(headerNameFormatted)
                {
                    case "ASSET_ID":
                        totalNumOfColumns = 4; cellBGColor = ColorTranslator.FromHtml("#BDD7EE");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);                        
                        break;

                    case "OWNERSHIP":
                        totalNumOfColumns = 5; cellBGColor = ColorTranslator.FromHtml("#C6E0B4");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);
                        break;

                    case "STRUCTURE":
                        totalNumOfColumns = 8; cellBGColor = ColorTranslator.FromHtml("#F8CBAD");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);
                        break;

                    case "NETWORK":
                        totalNumOfColumns = 5; cellBGColor = ColorTranslator.FromHtml("#ACB9CA");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);
                        break;

                    case "ASSET_ATTRIBUTES":
                        totalNumOfColumns = 11; cellBGColor = ColorTranslator.FromHtml("#FFF2CC");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);
                        break;

                    case "FUNDING":
                        totalNumOfColumns = 9; cellBGColor = ColorTranslator.FromHtml("#E2EFDA");
                        startColumn = endColumn + 1; endColumn = startColumn + (totalNumOfColumns - 1);
                        break;
                }

                //Add value
                worksheet.Cells[sectionHeaderRow, startColumn].Value = headerName;
                ExcelHelper.ApplyColor(worksheet.Cells[sectionHeaderRow, startColumn], cellBGColor);
                ExcelHelper.MergeCells(worksheet, sectionHeaderRow, startColumn, sectionHeaderRow, endColumn);
            }

            //data header
            int dataHeaderRow = 2;
            ExcelHelper.MergeCells(worksheet, dataHeaderRow, dataHeaders.Count, dataHeaderRow, dataHeaders.Count + 5); //Bridge Funding Merge Columns
            for (int column = 0; column < dataHeaders.Count; column++)
            {
                worksheet.Cells[dataHeaderRow, column + 1].Value = dataHeaders[column];
            }

            // sub header columns
            var subHeaderCol = dataHeaders.Count;
            for(int i = 0; i < subHeaders.Count; i++)
            {
                worksheet.Cells[dataHeaderRow + 1, subHeaderCol++].Value = subHeaders[i];
            }

            var currentCell = new CurrentCell { Row = dataHeaderRow, Column = dataHeaders.Count + 5 };
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow + 1, worksheet.Dimension.Columns]);

            //dynamic year headers
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);

            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work Done\r\n";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                ExcelHelper.MergeCells(worksheet, row, ++column, row, ++column);
                worksheet.Cells[row, column - 1].Value = HeaderConstText + year;
                worksheet.Cells[row + 2, column -1].Value = BAMSConstants.Work;
                worksheet.Cells[row + 2, column].Value = "Cost";
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column - 1, row + 2, column]);
                ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], Color.FromArgb(244, 176, 132));
            }

            worksheet.Cells[row, ++column].Value = "Work Done\r\nY/N";
            worksheet.Cells[row, ++column].Value = "Work Done\r\n> 1";
            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1, row, column], Color.FromArgb(244, 176, 132));

            worksheet.Cells[row, ++column].Value = "Total\r\nWork Done\r\n> 1";
            worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            var poorOnOffRateColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, column].Value = year;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                column++;
            }

            worksheet.Row(row).Height = 40;
            // Merge 2 rows for headers till column before Bridge Funding
            for (int cellColumn = 1; cellColumn < currentCell.Column - 5; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            // Merge 2 rows for headers of "Work Done" columns
            for (int cellColumn = currentCell.Column + 1; cellColumn < poorOnOffRateColumn - 4; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, ++cellColumn);
            }

            // Merge "Work Done", "Work done more than once", "Total"
            for(var cellColumn = poorOnOffRateColumn - 3; cellColumn < poorOnOffRateColumn; cellColumn++)
            {
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }
            // Merge columns for Poor On/Off Rate
            ExcelHelper.MergeCells(worksheet, row, poorOnOffRateColumn, row + 1, column - 1);

            //TODO: Hide Poor/On Off Columns

            currentCell.Column = column;

            // Add Years Data headers
            var simulationHeaderTexts = GetSimulationHeaderTexts();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count - 5);
            ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);

            // Empty column
            currentCell.Column = ++column;

            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);

            var yearHeaderColumn = currentCell.Column;
            simulationHeaderTexts.RemoveAll(_ => _.Equals("SD"));
            _spacerColumnNumbers = new List<int>();

            foreach (var simulationYear in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = simulationYear;
                column = currentCell.Column;
                column = AddSimulationHeaderTexts(worksheet, column, row, simulationHeaderTexts, simulationHeaderTexts.Count);
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

        private int AddSimulationHeaderTexts(ExcelWorksheet worksheet, int column, int row, List<string> simulationHeaderTexts, int length)
        {
            for (var index = 0; index < length; index++)
            {
                worksheet.Cells[row + 1, ++column].Value = simulationHeaderTexts[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);
            }

            return column;
        }

        private List<string> GetSimulationHeaderTexts()
        {
            return new List<string>
            {
                "GCR\r\nDeck",
                "GCR\r\nSup",
                "GCR\r\nSub",
                "GCR\r\nCulv",
                "Deck\r\nDur",
                "Super\r\nDur",
                "Sub\r\nDur",
                "Culv\r\nDur",
                "Min\r\nGCR",
                "Poor",
                "Posted",
                "Project Pick",
                "Budget",
                "Recommended Treatment",
                "Cost",
                "FHWA\r\nWork Types",
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
            var culvSeeded = _summaryReportHelper.checkAndGetValue<double>(numericAttribute, "CULV_SEEDED"); numericAttribute["CULV_SEEDED"] = culvSeeded;
            var minCond = _summaryReportHelper.checkAndGetValue<double>(numericAttribute, "MINCOND"); numericAttribute["MINCOND"] = minCond;

            numericAttribute["MINCOND"] = numericAttribute["CULV_SEEDED"];
            worksheet.Cells[row, ++column].Value = numericAttribute["MINCOND"];
            return column;
        }

        private int EnterMinDeckSuperSub(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            var minValue = Math.Min(_summaryReportHelper.checkAndGetValue<double>(numericAttribute, "DECK_SEEDED"),
                           Math.Min(_summaryReportHelper.checkAndGetValue<double>(numericAttribute, "SUP_SEEDED")
                                    , _summaryReportHelper.checkAndGetValue<double>(numericAttribute, "SUB_SEEDED")));
            worksheet.Cells[row, ++column].Value = minValue;
            numericAttribute["MINCOND"] = minValue;
            return column;
        }

        private int EnterMinDeckSuperSubCulv(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            worksheet.Cells[row, ++column].Value = _summaryReportHelper.checkAndGetValue<double>(numericAttribute, "MINCOND");
            return column;
        }

        private void TrackInitialYearDataForParametersTAB(AssetSummaryDetail intialsection)
        {
            // Get NHS record for Parameter TAB
            if (_parametersModel.nHSModel.NHS == null || _parametersModel.nHSModel.NonNHS == null)
            {
                int.TryParse(_summaryReportHelper.checkAndGetValue<string>(intialsection.ValuePerTextAttribute, "NHS_IND"), out var numericValue);
                if (numericValue > 0)
                {
                    _parametersModel.nHSModel.NHS = "Y";
                }
                else
                {
                    _parametersModel.nHSModel.NonNHS = "Y";
                }
            }
            // Get BPN data for parameter TAB
            if (!_parametersModel.BPNValues.Contains(_summaryReportHelper.checkAndGetValue<string>(intialsection.ValuePerTextAttribute, "BUS_PLAN_NETWORK")))
            {
                _parametersModel.BPNValues.Add(_summaryReportHelper.checkAndGetValue<string>(intialsection.ValuePerTextAttribute, "BUS_PLAN_NETWORK"));
            }
        }

        private void TrackDataForParametersTAB(Dictionary<string, double> valuePerNumericAttribute, Dictionary<string, string> valuePerTextAttribute)
        {
            // Track status for parameters TAB
            var postStatus = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "POST_STATUS").ToLower();
            if (!_parametersModel.Status.Contains(postStatus)) { _parametersModel.Status.Add(postStatus); }

            // Track P3 for parameters TAB
            var p3 = _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "P3");
            if (p3 > 0 && _parametersModel.P3 != 1) { _parametersModel.P3 = (int)p3; }

            var ownerCode = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "OWNER_CODE");
            if (!_parametersModel.OwnerCode.Contains(ownerCode)) { _parametersModel.OwnerCode.Add(ownerCode); }

            var structureLength = (int)_summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, "LENGTH");
            if (structureLength > 20 && _parametersModel.LengthGreaterThan20 != "Y") { _parametersModel.LengthGreaterThan20 = "Y"; }
            if (structureLength >= 8 && structureLength <= 20 && _parametersModel.LengthBetween8and20 != "Y") { _parametersModel.LengthBetween8and20 = "Y"; }

            var functionalClass = _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, "FUNC_CLASS");
            if (!_parametersModel.FunctionalClass.Contains(functionalClass)) { _parametersModel.FunctionalClass.Add(functionalClass); }
        }

        private enum MinCValue
        {
            minOfCulvDeckSubSuper,
            minOfDeckSubSuper,
            valueEqualsCulv,
            defaultValue
        }

        #endregion Private Methods
    }
}
