using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Common;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static AppliedResearchAssociates.iAM.Analysis.Engine.FundingCalculationOutput;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData
{
    public class BridgeDataForSummaryReport
    {
        private List<int> _spacerColumnNumbers;
        private HighlightWorkDoneCells _highlightWorkDoneCells;
        private Dictionary<MinCValue, Func<ExcelWorksheet, int, int, Dictionary<string, double>, int>> _valueForMinC;
        private readonly List<int> _simulationYears = new List<int>();
        private SummaryReportHelper _summaryReportHelper;
        private ReportHelper _reportHelper;

        // This is also used in Bridge Work Summary TAB
        private readonly List<double> _previousYearInitialMinC = new List<double>();
        private Dictionary<int, (int on, int off)> _poorOnOffCount = new Dictionary<int, (int on, int off)>();
        private Dictionary<int, Dictionary<string, int>> _bpnPoorOnPerYear = new Dictionary<int, Dictionary<string, int>>();
        private Dictionary<int, int> _nhsPoorOnPerYear = new Dictionary<int, int>();
        private Dictionary<int, int> _nonNhsPoorOnPerYear = new Dictionary<int, int>();

        // This will be used in Parameters TAB
        private readonly ParametersModel _parametersModel = new ParametersModel();
        private readonly IUnitOfWork _unitOfWork;

        public BridgeDataForSummaryReport(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _highlightWorkDoneCells = new HighlightWorkDoneCells();
            _summaryReportHelper = new SummaryReportHelper();
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public WorkSummaryModel Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData
            , Dictionary<string, string> treatmentCategoryLookup, bool allowFundingFromMultipleBudgets)
        {
            //set default width
            worksheet.DefaultColWidth = 13;

            // Add data to excel.
            var sectionHeaders = GetStaticSectionHeaders();
            var dataHeaders = GetStaticDataHeaders();
            var subHeaders = GetStaticSubHeaders();

            reportOutputData.Years.ForEach(_ => _simulationYears.Add(_.Year));

            int poorOnOffColStart = -1; int poorOnOffColEnd = -1; 
            var currentCell = AddHeadersCells(worksheet, sectionHeaders, dataHeaders, subHeaders, _simulationYears, ref poorOnOffColStart, ref poorOnOffColEnd);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (ExcelRange autoFilterCells = worksheet.Cells[currentCell.Row, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            AddBridgeDataModelsCells(worksheet, reportOutputData, currentCell);
            AddDynamicDataCells(worksheet, reportOutputData, currentCell, treatmentCategoryLookup, allowFundingFromMultipleBudgets);

            //autofit columns
            worksheet.Cells.AutoFitColumns();

            //turn off auto fit to hide poor on/off
            worksheet.Cells[1, poorOnOffColStart, worksheet.Cells.Rows, poorOnOffColEnd].AutoFitColumns(0, 0);

            //set default width for years seperator columns
            foreach(var spacerColumn in _spacerColumnNumbers) {
                worksheet.Cells[1, spacerColumn, worksheet.Cells.Rows, spacerColumn].AutoFitColumns(0, 0);
                worksheet.Column(spacerColumn).SetTrueWidth(3);
            }

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


        private void AddBridgeDataModelsCells(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row;
            var columnNo = currentCell.Column;
            foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
            {
                rowNo++; columnNo = 1;

                //--------------------- Asset ID ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERNET_REPORT"); //Internet Report 
                
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BRIDGE_TYPE"); //Bridge (B/C)
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BMSID"); //Bridge ID
                
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "BRKEY_"); //BRKey
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);                

                //--------------------- Ownership ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DISTRICT"); //District
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY"); //County

                var ownerName = ""; var ownerCode = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
                if (!string.IsNullOrEmpty(ownerCode) && !string.IsNullOrWhiteSpace(ownerCode)) { ownerName = MappingContent.OwnerCodeForReport(ownerCode); }
                worksheet.Cells[rowNo, columnNo++].Value = ownerName;

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SUBM_AGENCY"); //Submitting Agency
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);
               
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "MPO_NAME"); // Planning Partner

                worksheet.Cells[rowNo, columnNo++].Value = ""; // TODO: City/Town/Place data here

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FEATURE_INTERSECTED"); //Feature Intersected

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FEATURE_CARRIED"); //Feature Carried

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "LOCATION"); // Location / Structure Name

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "CUSTODIAN"); //Maintenance Responsibility                

                //--------------------- Structure ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "LENGTH"); //Structure Length
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DECK_AREA"); //Deck Area
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "LARGE_BRIDGE"); //Large Bridge
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);
                
                var spanType = ""; var spanTypeName = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SPANTYPE"); //Span Type
                if (!string.IsNullOrEmpty(spanTypeName) && !string.IsNullOrWhiteSpace(spanTypeName)) {
                    spanType = spanTypeName == SpanType.M.ToSpanTypeName() ? MappingContent.SpanTypeMap[SpanType.M] : MappingContent.SpanTypeMap[SpanType.S];
                }
                worksheet.Cells[rowNo, columnNo++].Value = spanType;

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FAMILY_ID"); //Bridge Family
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "STRUCTURE_TYPE"); //Structure Type

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FRACT_CRIT"); //Fractural Critical
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "PARALLEL") > 0 ? "Y" : "N"; //Parallel Structure
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                //--------------------- Network ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.FullFunctionalClassDescription(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FUNC_CLASS")); //Functional Class

                worksheet.Cells[rowNo, columnNo++].Value = int.TryParse(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NHS_IND"), out var numericValue) && numericValue > 0 ? "Y" : "N"; //NHS
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NBISLEN"); //NBIS Len
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BUS_PLAN_NETWORK"); //BPN
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERSTATE"); //Interstate
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                //--------------------- Asset Attibutes ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = MappingContent.GetDeckSurfaceType(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DECKSURF_TYPE")); //Deck Surface Type

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "WS_SEEDED"); // Wearing Surface Cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_COND"); //Paint Cond
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_EXTENT"); //Paint Ext
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = (int)_reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "YEAR_BUILT"); //Year Built
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "AGE"); //Age
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "ADTTOTAL"); //ADT
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "RISK_SCORE"); //Risk Score
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DET_LENGTH"); //Detour Length
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "POST_STATUS") == 0 ? "OPEN" : "POSTED"; //Posting Status
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "SUFF_RATING"); //Suff Rating
                ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

                worksheet.Cells[rowNo, columnNo++].Value = ""; // TODO: Leaking Joints data here

                worksheet.Cells[rowNo, columnNo++].Value = ""; // TODO: Scour Critical data here

                //--------------------- Funding ---------------------
                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "HBRR_ELIG"); //HBRR Elig
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "P3") > 0 ? "Y" : "N"; //P3
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

                worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FEDAID"); //Federal Aid

                _previousYearInitialMinC.Add(_reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "MINCOND"));

                // Bridge Funding
                var columnForStyle = columnNo;
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingNHPP(sectionSummary) ? "Y" : "N"; //NHPP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingSTP(sectionSummary) ? "Y" : "N"; //STP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingBOF(sectionSummary) ? "Y" : "N"; //BOF

                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingBRIP(sectionSummary) ? "Y" : "N"; //BRIP
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingState(sectionSummary) ? "Y" : "N"; //State
                worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.BridgeFundingNotApplicable(sectionSummary) ? "Y" : "N"; //NA

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnForStyle, rowNo, columnNo - 1]);

                if (rowNo % 2 == 0)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray);
                }
            }
            currentCell.Row = rowNo;
            currentCell.Column = columnNo;
        }

        private int AddSimulationYearData(ExcelWorksheet worksheet, int row, int column, AssetSummaryDetail initialSection, AssetDetail section)
        {
            var initialColumnForShade = column + 1;
            var selectedSection = initialSection ?? section;
            var minCActionCallDecider = MinCValue.minOfCulvDeckSubSuper;
            int.TryParse(_reportHelper.CheckAndGetValue<string>(selectedSection.ValuePerTextAttribute, "FAMILY_ID"), out var familyId);
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
                worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "DECK_SEEDED");
                worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUP_SEEDED");
                worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUB_SEEDED");
                ExcelHelper.SetCustomFormat(worksheet.Cells[row, column - 2, row, column], ExcelHelperCellFormat.DecimalPrecision3);
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column - 2, row, column]);

                worksheet.Cells[row, column + 2].Value = (int)_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "DECK_DURATION_N");
                worksheet.Cells[row, column + 3].Value = (int)_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUP_DURATION_N");
                worksheet.Cells[row, column + 4].Value = (int)_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "SUB_DURATION_N");
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
                worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "CULV_SEEDED");
                ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);

                worksheet.Cells[row, column + 4].Value = (int)_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "CULV_DURATION_N");
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column + 4]);
            }
            column += 4; // this will take us to "Min cond" column

            // It returns the column number where MinC value is written
            column = _valueForMinC[minCActionCallDecider](worksheet, row, column, selectedSection.ValuePerNumericAttribute);
            ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision3);
            var minCondColumn = column;

            if (_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "P3") > 0 && _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") < 5)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, column], Color.Yellow);
                ExcelHelper.SetTextColor(worksheet.Cells[row, column], Color.Black);
            }
            worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") < 5 ? BAMSConstants.Yes : BAMSConstants.No; //poor
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column]);

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, initialColumnForShade, row, column], Color.LightGray);
            }

            // Setting color of MinCond over here, to avoid Color.LightGray overriding it
            if (_reportHelper.CheckAndGetValue<double>(selectedSection.ValuePerNumericAttribute, "MINCOND") <= 3.5)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, minCondColumn], Color.FromArgb(112, 48, 160));
                ExcelHelper.SetTextColor(worksheet.Cells[row, minCondColumn], Color.White);
            }

            worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<string>(selectedSection.ValuePerTextAttribute, "POST_STATUS") == "POSTED" ? "Y" : "N"; //Posted
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, column]);

            return column;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput outputResults, CurrentCell currentCell, Dictionary<string, string> treatmentCategoryLookup, bool allowFundingFromMultipleBudgets)
        {
            var initialRow = 6;
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

            Dictionary<double, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new Dictionary<double, List<TreatmentConsiderationDetail>>();
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
                    section_BRKEY = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");

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

                    bool isNHS = int.TryParse(_reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "NHS_IND"), out var numericValue) && numericValue > 0;

                    int nhsOrNonPoorOnCount = isNHS ? _nhsPoorOnPerYear[yearlySectionData.Year] : _nonNhsPoorOnPerYear[yearlySectionData.Year];

                    Dictionary<string, int> bpnPoorOnDictionary = _bpnPoorOnPerYear[yearlySectionData.Year];
                    var busPlanNetwork = _reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "BUS_PLAN_NETWORK");
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

                    var thisYrMinc = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "MINCOND");
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
                            .Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY);
                        previousYearCause = prevYearSection.TreatmentCause;
                        previousYearTreatment = prevYearSection.AppliedTreatment;
                    }
                    setColor((int)_reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "PARALLEL"), section.AppliedTreatment, previousYearTreatment, previousYearCause, section.TreatmentCause,
                        yearlySectionData.Year, index, worksheet, row, column);

                    // Work done in a year
                    // Build keyCashFlowFundingDetails                    
                    if(section.TreatmentStatus != TreatmentStatus.Applied)
                    {
                        var fundingSection = yearlySectionData.Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY && _.TreatmentCause == TreatmentCause.SelectedTreatment && _.AppliedTreatment.ToLower() != BAMSConstants.NoTreatment);
                        if (fundingSection != null && !keyCashFlowFundingDetails.ContainsKey(section_BRKEY))
                        {
                            keyCashFlowFundingDetails.Add(section_BRKEY, fundingSection?.TreatmentConsiderations ?? new());
                        }
                    }                    

                    // If TreatmentStatus Applied and TreatmentCause is not CashFlowProject it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
                    var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause != TreatmentCause.CashFlowProject ?
                                                  section.TreatmentConsiderations : keyCashFlowFundingDetails[section_BRKEY];
                    var appliedTreatmentConsideration = treatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment);
                    var cost = appliedTreatmentConsideration == null ? 0 : Math.Round(appliedTreatmentConsideration.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == yearlySectionData.Year)?.Sum(b => b.AllocatedAmount) ?? 0, 0); // Rounded cost to whole number based on comments from Jeff Davis                    
                    var workCell = worksheet.Cells[row, column];
                    if (abbreviatedTreatmentNames.ContainsKey(section.AppliedTreatment))
                    {
                        workCell.Value = abbreviatedTreatmentNames[section.AppliedTreatment];
                        worksheet.Cells[row, column + 1].Value = cost;

                        if (!isInitialYear && section.TreatmentCause == TreatmentCause.CashFlowProject)
                        {
                            if (prevYearSection == null)
                            {
                                prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                                    .Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY);
                            }
                            if (prevYearSection.AppliedTreatment == section.AppliedTreatment)
                            {
                                workCell.Value = "--";
                                worksheet.Cells[row, column + 1].Value = cost;
                            }
                        }
                        worksheet.Cells[row, column + 1].Value = cost;
                        ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1], ExcelFormatStrings.CurrencyWithoutCents);

                    }
                    else
                    {
                        workCell.Value = section.AppliedTreatment.ToLower() == BAMSConstants.NoTreatment ? "--" :
                            section.AppliedTreatment.ToLower();

                        worksheet.Cells[row, column + 1].Value = cost;
                        ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1], ExcelFormatStrings.CurrencyWithoutCents);
                    }
                    if (!workCell.Value.Equals("--"))
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
                    else if (worksheet.Cells[row, poorOnOffColumnStart].Value.ToString() == "Off")
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
                        var cellColor = worksheet.Cells[row, column, row, column + 1].Style.Fill.BackgroundColor;
                        if (section.TreatmentCause != TreatmentCause.CashFlowProject &&
                            !(section.TreatmentCause == TreatmentCause.CommittedProject && previousYearCause == TreatmentCause.CommittedProject && previousYearTreatment.ToLower() != BAMSConstants.NoTreatment))
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
                    try
                    {
                        column = currentCell.Column;
                        column = AddSimulationYearData(worksheet, row, column, null, section);
                        var initialColumnForShade = column;

                        //get unique key (brkey) to compare
                        var section_BRKEY = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_");
                        AssetDetail prevYearSection = null;
                        if (!isInitialYear)
                        {
                            prevYearSection = outputResults.Years.FirstOrDefault(f => f.Year == sectionData.Year - 1)?
                                .Assets.FirstOrDefault(_ => _reportHelper.CheckAndGetValue<double>(_.ValuePerNumericAttribute, "BRKEY_") == section_BRKEY);
                        }

                        if (section.TreatmentCause == TreatmentCause.CashFlowProject && !isInitialYear)
                        {
                            var cashFlowMap = MappingContent.GetCashFlowProjectPick(section.TreatmentCause, prevYearSection);
                            worksheet.Cells[row, ++column].Value = cashFlowMap.currentPick; //Project Pick
                            worksheet.Cells[row, column - 18].Value = cashFlowMap.previousPick; //Project Pick previous year
                        }
                        else
                        {
                            worksheet.Cells[row, ++column].Value = MappingContent.GetNonCashFlowProjectPick(section.TreatmentCause, section.ProjectSource);
                        }

                        var recommendedTreatment = section.AppliedTreatment ?? ""; // Recommended Treatment

                        // If TreatmentStatus Applied it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
                        var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied ? section.TreatmentConsiderations : keyCashFlowFundingDetails[section_BRKEY] ?? new();
                        var cost = Math.Round(treatmentConsiderations.Sum(_ => _.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == sectionData.Year)?.Sum(_ => _.AllocatedAmount) ?? 0), 0); // Rounded cost to whole number based on comments from Jeff Davis

                        //get budget usages
                        var allocations = new List<Allocation>();
                        var allocationMatrices = treatmentConsiderations.Select(_ => _.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == sectionData.Year)).ToList() ?? new();
                        foreach (var allocationMatrix in allocationMatrices)
                        {
                            if (allocationMatrix != null && allocationMatrix.Any())
                            {
                                allocations.AddRange(allocationMatrix);
                            }
                        }

                        //check budget usages
                        var budgetName = "";
                        if (allocationMatrices?.Any() == true && cost > 0)
                        {
                            var budgetNames = allocations.Select(_ => _.BudgetName).Distinct().ToList();
                            if (allocationMatrices.Count == 1 && budgetNames.Count() == 1) //single budget
                            {
                                budgetName = budgetNames.First() ?? ""; // Budget
                                if (string.IsNullOrEmpty(budgetName) || string.IsNullOrWhiteSpace(budgetName))
                                {
                                    budgetName = BAMSConstants.Unspecified_Budget;
                                }
                            }
                            else //multiple budgets
                            {
                                //check for multi year budget
                                if (allowFundingFromMultipleBudgets == true || budgetNames.Count > 1)
                                {
                                    foreach (var allocationBudgetName in budgetNames)
                                    {
                                        var multiYearBudgetCost = allocations.Where(_ => _.BudgetName == allocationBudgetName).Sum(_ => _.AllocatedAmount);
                                        var multiYearBudgetName = allocationBudgetName ?? ""; // Budget;
                                        if (string.IsNullOrEmpty(multiYearBudgetName) || string.IsNullOrWhiteSpace(multiYearBudgetName))
                                        {
                                            multiYearBudgetName = BAMSConstants.Unspecified_Budget;
                                        }

                                        var budgetAmountAbbrName = "";
                                        if (multiYearBudgetCost > 0)
                                        {
                                            //check budget and add abbreviation
                                            budgetAmountAbbrName = "$" + ReportCommon.FormatNumber(multiYearBudgetCost, 1);

                                            //set budget header name
                                            if (!string.IsNullOrEmpty(budgetAmountAbbrName) && !string.IsNullOrWhiteSpace(budgetAmountAbbrName))
                                            {
                                                multiYearBudgetName += " (" + budgetAmountAbbrName + ")";
                                            }

                                            if (string.IsNullOrEmpty(allocationBudgetName) || string.IsNullOrWhiteSpace(allocationBudgetName))
                                            {
                                                budgetName = multiYearBudgetName;
                                            }
                                            else
                                            {
                                                budgetName += ", " + multiYearBudgetName;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        worksheet.Cells[row, ++column].Value = budgetName; // Budget
                        worksheet.Cells[row, ++column].Value = recommendedTreatment; // Recommended Treatment
                        var columnForAppliedTreatment = column;

                        worksheet.Cells[row, ++column].Value = cost; // cost
                        ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column], ExcelFormatStrings.CurrencyWithoutCents);

                        if (!string.IsNullOrEmpty(recommendedTreatment) && !string.IsNullOrWhiteSpace(recommendedTreatment) && treatmentCategoryLookup.ContainsKey(recommendedTreatment))
                        {
                            worksheet.Cells[row, ++column].Value = treatmentCategoryLookup[recommendedTreatment]?.ToString(); // FHWA Work Type
                        }
                        else
                        {
                            worksheet.Cells[row, ++column].Value = ""; // FHWA Work Type
                        }
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
                            ExcelHelper.ApplyColor(worksheet.Cells[row, columnForAppliedTreatment - 18], Color.FromArgb(0, 255, 0));
                            ExcelHelper.SetTextColor(worksheet.Cells[row, columnForAppliedTreatment - 18], Color.FromArgb(255, 0, 0));
                        }

                        column = column + 1;
                        row++;
                    }

                    catch (Exception ex)
                    {

                    }
                    isInitialYear = false;
                }
            }
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
                "Bridge (B/C)",
                "BridgeID\r\n(5A01)",
                "BRKey\r\n(5A03)",               


                //--------------------- Ownership ---------------------
                "District\r\n(5A04)",
                "County\r\n(5A05)",
                "Owner Code\r\n(5A21)",
                "Submitting Agency\r\n(6A06)",
                "Planning Partner\r\n(5A13)",
                "City / Town / Place\r\n(5A06)",
                "Feature Intersected\r\n(5A07)",
                "Facility Carried\r\n(5A08)",
                "Location / Structure Name\r\n(5A02)",
                "Maintenance Responsibility\r\n(5A20)",

                //--------------------- Structure ---------------------
                "Structure Length\r\n(5B18)",
                "Deck Area (5B19)",
                "Large Bridge",
                "Span Type",
                "Bridge Family",
                "Struct Type\r\n(6A26-29)",
                "Fractural Critical",
                "Parallel Structure\r\n(5E02)",                


                //--------------------- Network ---------------------

                "Functional Class\r\n(5C22)",
                "NHS\r\n(5C29)",
                "NBIS Len\r\n(5E01)",
                "BPN\r\n(6A19)",
                "Interstate",

                //--------------------- Asset Attributes ---------------------
                
                "Deck Surface Type\r\n(5B02)",
                "Wearing Surface Cond\r\n(6B40)",
                "Paint Cond\r\n(6B36)",
                "Paint Ext\r\n(6B37)",
                "Year Built\r\n(5A15)",
                "Age",
                "ADTT\r\n(5C10)",
                "Risk Score",
                "Detour Length\r\n(5C15)",
                "Posting Status\r\n(VP02)",
                "Suff Rating\r\n(4A13)",
                "Leaking Joints\r\n",
                "Scour Critical",

                //--------------------- Funding ---------------------
                "HBRR Elig\r\n(6B41)",
                "P3\r\n(5E24)",
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


        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> sectionHeaders
                                            , List<string> dataHeaders, List<string> subHeaders, List<int> simulationYears
                                            , ref int poorOnOffColStart, ref int poorOnOffColEnd)
        {
            //section header
            int sectionHeaderRow = 2; var totalNumOfColumns = 0; var cellBGColor = ColorTranslator.FromHtml("#FFFFFF"); //White
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
                        totalNumOfColumns = 10; cellBGColor = ColorTranslator.FromHtml("#C6E0B4");
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
                        totalNumOfColumns = 13; cellBGColor = ColorTranslator.FromHtml("#FFF2CC");
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
            int dataHeaderRow = sectionHeaderRow + 1;
            ExcelHelper.MergeCells(worksheet, dataHeaderRow, dataHeaders.Count, dataHeaderRow, dataHeaders.Count + 5); //Bridge Funding Merge Columns
            for (int column = 0; column < dataHeaders.Count; column++)
            {
                var dataHeaderText = dataHeaders[column];
                worksheet.Cells[dataHeaderRow, column + 1].Value = dataHeaderText;
            }

            // sub header columns
            var subHeaderCol = dataHeaders.Count;
            for(int i = 0; i < subHeaders.Count; i++)
            {
                worksheet.Cells[dataHeaderRow + 1, subHeaderCol++].Value = subHeaders[i];
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[dataHeaderRow + 1, subHeaderCol - 1]);
            }

            var currentCell = new CurrentCell { Row = dataHeaderRow, Column = dataHeaders.Count + 5 };
            ExcelHelper.ApplyStyle(worksheet.Cells[dataHeaderRow + 1, dataHeaders.Count, dataHeaderRow + 1, dataHeaders.Count + 5]);
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow + 1, worksheet.Dimension.Columns]);

            //dynamic year headers
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears, ref poorOnOffColStart, ref poorOnOffColEnd);

            return currentCell;
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears
                                            , ref int poorOnOffColStart, ref int poorOnOffColEnd)
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
                ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], ColorTranslator.FromHtml("#D9E1F2"));
            }

            worksheet.Cells[row, ++column].Value = "Work Done\r\nY/N";
            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], ColorTranslator.FromHtml("#B4C6E7"));

            worksheet.Cells[row, ++column].Value = "Work Done\r\n> 1";
            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], ColorTranslator.FromHtml("#B4C6E7"));

            worksheet.Cells[row, ++column].Value = "Total\r\nWork Done\r\n> 1";
            ExcelHelper.ApplyColor(worksheet.Cells[row, column - 1], ColorTranslator.FromHtml("#B4C6E7"));
            ExcelHelper.SetCustomFormat(worksheet.Cells[row, column - 1], ExcelHelperCellFormat.Number);

            worksheet.Cells[row, ++column].Value = "Poor On/Off Rate";
            var poorOnOffRateColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row + 2, column].Value = year;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                column++;
            }

            //set poor on/off column start and end index
            poorOnOffColStart = poorOnOffRateColumn; poorOnOffColEnd = column - 1;

            // Merge 2 rows for headers till column before Bridge Funding
            worksheet.Row(row).Height = 25;
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
                ExcelHelper.ApplyColor(worksheet.Cells[row, cellColumn], ColorTranslator.FromHtml("#B4C6E7"));
                ExcelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            // Merge columns for Poor On/Off Rate
            ExcelHelper.MergeCells(worksheet, row, poorOnOffRateColumn, row + 1, poorOnOffColEnd);

            //hide poor on/off columns            
            for (var columnIndex = poorOnOffRateColumn; columnIndex <= poorOnOffColEnd; columnIndex++)
            {
                worksheet.Column(columnIndex).Hidden = true;
            }

            _spacerColumnNumbers = new List<int>();

            //set current column
            currentCell.Column = column;
            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
            _spacerColumnNumbers.Add(currentCell.Column);

            // Add Years Data headers
            var initialSimulationHeaderTexts = GetInitialSimulationHeaderTexts();
            worksheet.Cells[row, ++column].Value = simulationYears[0] - 1;
            column = currentCell.Column;
            column = AddSimulationHeaderTexts(worksheet, column, row, initialSimulationHeaderTexts, initialSimulationHeaderTexts.Count);
            ExcelHelper.MergeCells(worksheet, row, currentCell.Column + 1, row, column);
            ExcelHelper.ApplyColor(worksheet.Cells[row, currentCell.Column + 1, row, column], ColorTranslator.FromHtml("#E2EFDA"));

            // Empty column
            currentCell.Column = ++column; 
            worksheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Column(column).Style.Fill.BackgroundColor.SetColor(Color.Gray);
                        
            var simulationHeaderTexts = GetSimulationHeaderTexts();
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

                //sperator column                
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
                var simulationHeaderText = simulationHeaderTexts[index];
                worksheet.Cells[row + 1, ++column].Value = simulationHeaderText;
                ExcelHelper.ApplyStyle(worksheet.Cells[row + 1, column]);                
            }

            return column;
        }

        private List<string> GetInitialSimulationHeaderTexts()
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
                "Posted"
            };
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
            var culvSeeded = _reportHelper.CheckAndGetValue<double>(numericAttribute, "CULV_SEEDED"); numericAttribute["CULV_SEEDED"] = culvSeeded;
            var minCond = _reportHelper.CheckAndGetValue<double>(numericAttribute, "MINCOND"); numericAttribute["MINCOND"] = minCond;

            numericAttribute["MINCOND"] = numericAttribute["CULV_SEEDED"];
            worksheet.Cells[row, ++column].Value = numericAttribute["MINCOND"];
            return column;
        }

        private int EnterMinDeckSuperSub(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            var minValue = Math.Min(_reportHelper.CheckAndGetValue<double>(numericAttribute, "DECK_SEEDED"),
                           Math.Min(_reportHelper.CheckAndGetValue<double>(numericAttribute, "SUP_SEEDED")
                                    , _reportHelper.CheckAndGetValue<double>(numericAttribute, "SUB_SEEDED")));
            worksheet.Cells[row, ++column].Value = minValue;
            numericAttribute["MINCOND"] = minValue;
            return column;
        }

        private int EnterMinDeckSuperSubCulv(ExcelWorksheet worksheet, int row, int column, Dictionary<string, double> numericAttribute)
        {
            worksheet.Cells[row, ++column].Value = _reportHelper.CheckAndGetValue<double>(numericAttribute, "MINCOND");
            return column;
        }

        private void TrackInitialYearDataForParametersTAB(AssetSummaryDetail intialsection)
        {
            // Get NHS record for Parameter TAB
            if (_parametersModel.nHSModel.NHS == null || _parametersModel.nHSModel.NonNHS == null)
            {
                int.TryParse(_reportHelper.CheckAndGetValue<string>(intialsection.ValuePerTextAttribute, "NHS_IND"), out var numericValue);
                if (numericValue > 0)
                {
                    _parametersModel.nHSModel.NHS = "Y";
                    _parametersModel.nHSModel.NonNHS = "N";
                }
                else
                {
                    _parametersModel.nHSModel.NonNHS = "Y";
                    _parametersModel.nHSModel.NHS = "N";
                }
            }
            // Get BPN data for parameter TAB
            if (!_parametersModel.BPNValues.Contains(_reportHelper.CheckAndGetValue<string>(intialsection.ValuePerTextAttribute, "BUS_PLAN_NETWORK")))
            {
                _parametersModel.BPNValues.Add(_reportHelper.CheckAndGetValue<string>(intialsection.ValuePerTextAttribute, "BUS_PLAN_NETWORK"));
            }
        }

        private void TrackDataForParametersTAB(Dictionary<string, double> valuePerNumericAttribute, Dictionary<string, string> valuePerTextAttribute)
        {
            // Track status for parameters TAB
            var postStatus = _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, "POST_STATUS").ToLower();
            if (!_parametersModel.Status.Contains(postStatus)) { _parametersModel.Status.Add(postStatus); }

            // Track P3 for parameters TAB
            var p3 = _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, "P3");
            if (p3 > 0 && _parametersModel.P3 != 1) { _parametersModel.P3 = (int)p3; }

            var ownerCode = _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, "OWNER_CODE");
            if (!_parametersModel.OwnerCode.Contains(ownerCode)) { _parametersModel.OwnerCode.Add(ownerCode); }

            var structureLength = (int)_reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, "LENGTH");
            if (structureLength > 20 && _parametersModel.LengthGreaterThan20 != "Y") { _parametersModel.LengthGreaterThan20 = "Y"; }
            if (structureLength >= 8 && structureLength <= 20 && _parametersModel.LengthBetween8and20 != "Y") { _parametersModel.LengthBetween8and20 = "Y"; }

            var functionalClass = _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, "FUNC_CLASS");
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
