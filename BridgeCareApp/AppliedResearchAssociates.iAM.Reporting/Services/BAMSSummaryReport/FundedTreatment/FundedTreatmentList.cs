using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models;
using MoreLinq;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.FundedTreatment
{
    public class FundedTreatmentList : IFundedTreatmentList
    {
        private ITreatmentCommon _treatmentCommon;
        private ReportHelper _reportHelper;

        public FundedTreatmentList()
        {
            _treatmentCommon = new TreatmentCommon.TreatmentCommon();
            _reportHelper = new ReportHelper();
        }

        public void Fill(ExcelWorksheet fundedTreatmentWorksheet, SimulationOutput simulationOutput)
        {
            var currentCell = AddHeadersCells(fundedTreatmentWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = fundedTreatmentWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            fundedTreatmentWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(fundedTreatmentWorksheet, simulationOutput, currentCell);
            fundedTreatmentWorksheet.Calculate();

            fundedTreatmentWorksheet.Cells.AutoFitColumns();
            _treatmentCommon.PerformPostAutofitAdjustments(fundedTreatmentWorksheet);
        }

        #region Private methods

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            var currentCell = _treatmentCommon.AddHeadersCells(worksheet);

            var headerRow = 1;
            var startingColumn = currentCell.Column;

            void fillHeaders(int row, int firstColumn, IEnumerable<string> headers)
            {
                foreach (var (index, header) in headers.Index())
                {
                    worksheet.Cells[row, index + firstColumn].Style.WrapText = false;
                    worksheet.Cells[row, index + firstColumn].Value = header;
                }
            }

            int columnOf(string header) => (HeadersRow1 as List<string>).IndexOf(header) + startingColumn;

            fillHeaders(headerRow, startingColumn, HeadersRow1);

            fillHeaders(headerRow + 1, columnOf(ANALYSIS_YEARS), AnalysisYearSubsections);
            fillHeaders(headerRow + 1, columnOf(PRIOR_GCR), GCRSubsections);
            fillHeaders(headerRow + 1, columnOf(RESULTING_GCR), GCRSubsections);

            worksheet.Row(headerRow).Height = 15;
            worksheet.Row(headerRow + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);

            void styleColumnWithSubsections(int startingColumn, int columnCount, Color? color = null)
            {
                ExcelHelper.MergeCells(worksheet, headerRow, startingColumn, headerRow, startingColumn + columnCount - 1);
                ExcelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, startingColumn, headerRow + 1, startingColumn + columnCount - 1]);
                if (color is not null)
                {
                    ExcelHelper.ApplyColor(worksheet.Cells[headerRow, startingColumn, headerRow + 1, startingColumn + columnCount], color.Value);
                }
            }

            styleColumnWithSubsections(columnOf(ANALYSIS_YEARS), AnalysisYearSubsections.Count);
            styleColumnWithSubsections(columnOf(PRIOR_GCR), GCRSubsections.Count, Color.FromArgb(255, 242, 204));
            styleColumnWithSubsections(columnOf(RESULTING_GCR), GCRSubsections.Count, Color.FromArgb(226, 239, 218));

            void mergeHeaderRows(int column) => ExcelHelper.MergeCells(worksheet, headerRow, column, headerRow + 1, column);

            foreach (var (index, header) in HeadersRow1.Index())
            {
                if (header is "" or ANALYSIS_YEARS or PRIOR_GCR or RESULTING_GCR)
                {
                    continue;
                }
                mergeHeaderRows(index + startingColumn);
            }

            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            return new CurrentCell { Row = 3, Column = worksheet.Dimension.Columns + 1 };
        }

        private const string CostFormat = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";

        private void FillDataInWorksheet(ExcelWorksheet worksheet, CurrentCell currentCell, FundedTreatmentReportInfo treatmentInfo, SimulationOutput simulationOutput)
        {
            var section = treatmentInfo.Asset;
            var year = treatmentInfo.Year.Year;

            var treatmentOption = treatmentInfo.TreatmentOption;
            var treatmentConsideration = treatmentInfo.TreatmentConsideration;

            _treatmentCommon.FillDataInWorkSheet(worksheet, currentCell, section, year);

            var (row, columnNo) = (currentCell.Row, currentCell.Column);

            // Analysis year Start
            worksheet.Cells[row, columnNo++].Value = year;

            // Analysis year End
            if (treatmentInfo.IsAnalysisLengthExceeded)
            {
                ++columnNo;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = year + treatmentInfo.LengthInYears - 1;
            }

            // Project Pick
            worksheet.Cells[row, columnNo++].Value = treatmentInfo.IsCashFlowed
                ? "BAMS Pick CFB"
                : (object)(section.TreatmentCause switch
                {
                    TreatmentCause.SelectedTreatment => "BAMS Pick",
                    TreatmentCause.ScheduledTreatment => "BAMS Pick",
                    TreatmentCause.CommittedProject => "MPMS Pick",
                    TreatmentCause.CashFlowProject => "BAMS Pick CFB",
                    _ => throw new InvalidOperationException("Asset in funded treatment list has no treatment.")
                });

            var treatmentName = treatmentOption?.TreatmentName ?? treatmentConsideration.TreatmentName;
            var treatmentCost = treatmentOption?.Cost ?? (double) treatmentConsideration.BudgetUsages.Sum(usage => usage.CoveredCost);

            var budget = section.TreatmentConsiderations
                .Where(c => c.TreatmentName == treatmentName)
                .FirstOrDefault(c => c.BudgetUsages.Any(b => b.Status is BudgetUsageStatus.CostCovered))
                ?.BudgetUsages?.First(b => b.Status is BudgetUsageStatus.CostCovered);

            var budgetName = budget?.BudgetName ?? string.Empty;

            // Budget
            worksheet.Cells[row, columnNo++].Value = budgetName;

            // Treatment
            worksheet.Cells[row, columnNo++].Value = treatmentName;

            // Yearly Cost
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = CostFormat;
            worksheet.Cells[row, columnNo++].Value = treatmentCost / treatmentInfo.LengthInYears;

            // Total Project Cost
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = CostFormat;
            if (treatmentInfo.IsAnalysisLengthExceeded)
            {
                ++columnNo;
            }
            else
            {
                worksheet.Cells[row, columnNo++].Value = treatmentCost;
            }

            currentCell.Column = columnNo;

            // "Prior GCR" (GCR for the year previous to Analysis Year(s)/Start)
            IEnumerable<AssetSummaryDetail> priorYearAssets = simulationOutput.Years.FirstOrDefault(y => y.Year == year - 1)?.Assets;
            priorYearAssets ??= simulationOutput.InitialAssetSummaries;
            var priorSection = priorYearAssets.First(asset => asset.ValuePerNumericAttribute["BRKEY_"] == section.ValuePerNumericAttribute["BRKEY_"]);

            FillGCRData(worksheet, currentCell, priorSection);

            // "Resulting GCR" (GCR for Analysis Year(s)/End)
            AssetSummaryDetail resultSection = null;
            if (!treatmentInfo.IsAnalysisLengthExceeded)
            {
                var resultYear = year + treatmentInfo.LengthInYears - 1;
                var resultYearDetail = simulationOutput.Years.FirstOrDefault(y => y.Year == resultYear);
                if (resultYearDetail != null)
                {
                    IEnumerable<AssetSummaryDetail> resultYearAssets = resultYearDetail.Assets;
                    resultSection = resultYearAssets.First(asset => asset.ValuePerNumericAttribute["BRKEY_"] == section.ValuePerNumericAttribute["BRKEY_"]);
                }

                FillGCRData(worksheet, currentCell, resultSection, treatmentInfo.IsAnalysisLengthExceeded);
            }

            if (row % 2 == 0)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row, 1, row, currentCell.Column - 1], Color.LightGray);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[row, 1, row, currentCell.Column - 1]);
        }

        private void FillGCRData(ExcelWorksheet worksheet, CurrentCell currentCell, AssetSummaryDetail section, bool leaveEmpty = false)
        {
            var (row, columnNo) = (currentCell.Row, currentCell.Column);

            object empty = leaveEmpty ? "" : null;

            var familyId = int.Parse(_reportHelper.CheckAndGetValue<string>(section.ValuePerTextAttribute, "FAMILY_ID"));
            if (familyId < 11)
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                var deck = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "DECK_SEEDED");
                worksheet.Cells[row, columnNo++].Value = empty ?? deck;

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                var sup = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUP_SEEDED");
                worksheet.Cells[row, columnNo++].Value = empty ?? sup;

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                var sub = _reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "SUB_SEEDED");
                worksheet.Cells[row, columnNo++].Value = empty ?? sub;

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = BAMSConstants.No; // CULV_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                var min = Math.Min(Math.Min(deck, sup), sub);
                worksheet.Cells[row, columnNo++].Value = empty ?? min;
            }
            else
            {
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = empty ?? BAMSConstants.No; // DECK_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = empty ?? BAMSConstants.No; // SUP_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = empty ?? BAMSConstants.No; // SUB_SEEDED

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = empty ?? section.ValuePerNumericAttribute["CULV_SEEDED"];

                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[row, columnNo]);
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = empty ?? section.ValuePerNumericAttribute["CULV_SEEDED"];
            }

            currentCell.Column = columnNo;
        }

        private class FundedTreatmentReportInfo {
            public SimulationYearDetail Year { get; init; }
            public AssetDetail Asset { get; init; }
            public TreatmentOptionDetail? TreatmentOption { get; init; }
            public TreatmentConsiderationDetail? TreatmentConsideration { get; init; }
            public bool IsCashFlowed { get; set; }
            public int LengthInYears { get; set; } = 1;
            public bool IsAnalysisLengthExceeded { get; set; } = false;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, List<FundedTreatmentReportInfo>>();
            var yearCountPerTreatment = new Dictionary<int, int>();
            var validFacilityIds = new List<int>();
            foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
            {
                var treatedSections = year.Assets
                    .Where(section => section.TreatmentCause is not TreatmentCause.Undefined and not TreatmentCause.NoSelection
                        && !(section.TreatmentCause is TreatmentCause.CommittedProject && section.AppliedTreatment.ToLower() == BAMSConstants.NoTreatment.ToLower()))
                    .ToList();

                var facilityIds = treatedSections.Select(section => Convert.ToInt32(_reportHelper.CheckAndGetValue<double>(section.ValuePerNumericAttribute, "BRKEY_")));

                treatedSections.ForEach(asset =>
                {
                    var id = Convert.ToInt32(_reportHelper.CheckAndGetValue<double>(asset.ValuePerNumericAttribute, "BRKEY_"));
                    var treatmentOption = asset.TreatmentOptions.FirstOrDefault(o => o.TreatmentName == asset.AppliedTreatment);
                    var treatmentConsideration = asset.TreatmentConsiderations.FirstOrDefault(c => c.TreatmentName == asset.AppliedTreatment);

                    var isCashFlowed = asset.TreatmentCause == TreatmentCause.CashFlowProject ||
                        asset.TreatmentConsiderations.Any(consideration =>
                            consideration.TreatmentName == asset.AppliedTreatment &&
                            consideration.CashFlowConsiderations.Any(cashFlow =>
                                cashFlow.ReasonAgainstCashFlow is ReasonAgainstCashFlow.None or ReasonAgainstCashFlow.LastYearOfCashFlowIsOutsideOfAnalysisPeriod));

                    var exceedsLastYear = isCashFlowed && asset.TreatmentConsiderations.Any(c => c.TreatmentName == asset.AppliedTreatment
                        && c.CashFlowConsiderations.Any(flow => flow.ReasonAgainstCashFlow is ReasonAgainstCashFlow.LastYearOfCashFlowIsOutsideOfAnalysisPeriod));

                    if (treatmentsPerSection.ContainsKey(id))
                    {
                        // brkey already present
                        if (asset.TreatmentCause == TreatmentCause.CashFlowProject
                            && treatmentsPerSection[id].Any(treatment =>
                            treatment.Asset.AppliedTreatment == asset.AppliedTreatment))
                        {
                            // Continuation of a cash flow project; do not include in report but increment length of existing treatment
                            var candidateTreatments = treatmentsPerSection[id]
                                .Where(t => t.TreatmentOption != null)
                                .Where(t => t.TreatmentOption.TreatmentName == asset.AppliedTreatment)
                                .ToList();

                            var initialTreatment =
                                candidateTreatments.Count == 1 ? candidateTreatments.Single() :
                                Enumerable.MinBy(candidateTreatments, t => year.Year - t.Year.Year);

                            initialTreatment.LengthInYears++;
                            initialTreatment.IsAnalysisLengthExceeded = initialTreatment.IsAnalysisLengthExceeded || exceedsLastYear;
                        }
                        else
                        {
                            treatmentsPerSection[id].Add(new() {
                                Year = year,
                                Asset = asset,
                                TreatmentOption = treatmentOption,
                                TreatmentConsideration = treatmentConsideration,
                                IsCashFlowed = isCashFlowed,
                                IsAnalysisLengthExceeded = exceedsLastYear
                            });
                        }
                    }
                    else
                    {
                        treatmentsPerSection.Add(id, new List<FundedTreatmentReportInfo>() {
                            new()
                            {
                                Year = year,
                                Asset = asset,
                                TreatmentOption = treatmentOption,
                                TreatmentConsideration = treatmentConsideration,
                                IsCashFlowed = isCashFlowed,
                                IsAnalysisLengthExceeded = exceedsLastYear
                            }
                        });
                    }
                });
            }

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

            foreach (var treatmentInfo in treatmentsPerSection.Values.SelectMany(a => a))
            {
                FillDataInWorksheet(worksheet, currentCell, treatmentInfo, simulationOutput);
                
                currentCell.Row++;
                currentCell.Column = 1;
            }
        }

        private const string ANALYSIS_YEARS = "Analysis Year(s)";
        private const string PRIOR_GCR = "Prior GCR";
        private const string RESULTING_GCR = "Resulting GCR";

        private static IReadOnlyList<string> HeadersRow1 { get; } = new List<string>()
        {
            ANALYSIS_YEARS, // Row 1 heading for 2 subsections
            "",

            "Project Pick",
            "Budget",
            "Treatment",
            "Yearly\r\nCost",
            "Total Project\r\nCost",

            PRIOR_GCR, // 4 subsections
            "",
            "",
            "",

            "Prior\r\nMIN",

            RESULTING_GCR, // 4 subsections
            "",
            "",
            "",

            "Resulting\r\nMIN"
        };

        private static IReadOnlyList<string> AnalysisYearSubsections { get; } = new List<string>()
        {
            "Start",
            "End"
        };

        private static IReadOnlyList<string> GCRSubsections { get; } = new List<string>()
        {
            "DECK",
            "SUP",
            "SUB",
            "CULV"
        };

        #endregion Private methods
    }
}
