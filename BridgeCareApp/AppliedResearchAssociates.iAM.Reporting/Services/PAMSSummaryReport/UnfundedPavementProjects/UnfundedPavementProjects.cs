using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.UnfundedPavementProjects
{
    internal class UnfundedPavementProjects
    {
        private SummaryReportHelper _summaryReportHelper;

        public UnfundedPavementProjects()
        {
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }
        }

        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = AddHeadersCells(unfundedRecommendationWorksheet);

            //fill work sheet
            AddDynamicDataCells(unfundedRecommendationWorksheet, simulationOutput, currentCell);
            unfundedRecommendationWorksheet.Calculate();  // force calculation of the total now

            unfundedRecommendationWorksheet.Cells.AutoFitColumns();
        }

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRowIndex = 1; var headersRow = GetHeadersRow();
            for (int column = 0; column < headersRow.Count; column++)
            {
                var headerCell = worksheet.Cells[headerRowIndex, column + 1];
                headerCell.Value = headersRow[column];
                headerCell.Style.WrapText = true;                
            }            

            var currentCell = new CurrentCell { Row = headerRowIndex, Column = worksheet.Dimension.Columns };

            worksheet.Cells.AutoFitColumns();
            using (ExcelRange autoFilterCells = worksheet.Cells[1, 1, currentCell.Row, currentCell.Column])
            {
                autoFilterCells.AutoFilter = true;
            }

            worksheet.DefaultColWidth = 13;
            worksheet.Row(headerRowIndex).Height = 42;
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRowIndex, 1, headerRowIndex, headersRow.Count]);

            return currentCell;
        }

        private static List<string> GetHeadersRow() => new()
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
                "BPN",
                "FamilyID",
                "MPO/RPO",
                "Surface Type",
                "Unfunded Year",
                "Unfunded Treatment",
                "Cost",
                "Cash Flow Yrs/Amount",
                "OPI",
                "Roughness",
                "Year Built",
                "Year Last Resurface",
                "Year Last  Structural Overlay",
                "ADT",
                "Truck %",
                "Risk Score",
        };

        public List<AssetDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections = simulationYearDetail.Assets.Where(s =>
            s.TreatmentCause == TreatmentCause.NoSelection
            && s.TreatmentOptions.Count > 0).ToList();
            return untreatedSections;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, List<Tuple<SimulationYearDetail, AssetDetail, TreatmentOptionDetail>>>();
            var validFacilityIds = new List<int>(); // Unfunded IDs
            var firstYear = true;
            var years = simulationOutput.Years.OrderBy(yr => yr.Year);
            foreach (var year in years)
            {
                //get untreated sections
                var untreatedSections = GetUntreatedSections(year);

                //get unfunded IDs
                if (firstYear)
                {
                    validFacilityIds.AddRange(untreatedSections.Select(_ => Convert.ToInt32(_summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "CNTY") + _summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "SR") + _.AssetName)));
                    firstYear = false; if (simulationOutput.Years.Count > 1) { continue; }
                }
                else
                {
                    validFacilityIds = validFacilityIds.Any() ?
                        validFacilityIds.Intersect(untreatedSections.Select(_ => Convert.ToInt32(_summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "CNTY") + _summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "SR") + _.AssetName))).ToList() :
                        untreatedSections.Select(_ => Convert.ToInt32(_summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "CNTY") + _summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "SR") + _.AssetName)).ToList();
                }

                foreach (var section in untreatedSections)
                {
                    var segmentNumber = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CNTY");
                    segmentNumber += _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "SR");
                    segmentNumber += section.AssetName;

                    var facilityId = Convert.ToInt32(segmentNumber);
                    var treatmentOptions = section.TreatmentConsiderations.Any() ?
                                            section.TreatmentOptions.Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList() :
                                            section.TreatmentOptions;
                    treatmentOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));
                    var chosenTreatment = treatmentOptions.FirstOrDefault();
                    if (chosenTreatment != null)
                    {
                        var newTuple = new Tuple<SimulationYearDetail, AssetDetail, TreatmentOptionDetail>(year, section, chosenTreatment);

                        if (!validFacilityIds.Contains(facilityId)) {
                            if (treatmentsPerSection.ContainsKey(facilityId)) { treatmentsPerSection.Remove(facilityId); }
                        }
                        else {
                            if (!treatmentsPerSection.ContainsKey(facilityId)) {
                                treatmentsPerSection.Add(facilityId, new List<Tuple<SimulationYearDetail, AssetDetail, TreatmentOptionDetail>> { newTuple });
                            }
                            else { treatmentsPerSection[facilityId].Add(newTuple); }
                        }
                    }
                }
            }

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

            var totalYears = simulationOutput.Years.Count;
            foreach (var facilityList in treatmentsPerSection.Values)
            {
                foreach (var facilityTuple in facilityList)
                {
                    var section = facilityTuple.Item2;
                    var year = facilityTuple.Item1;
                    var treatment = facilityTuple.Item3;
                    FillDataInWorkSheet(worksheet, currentCell, section, year.Year, treatment);
                    currentCell.Row++;
                    currentCell.Column = 1;
                }
            }
        }

        private void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year, TreatmentOptionDetail treatment)
        {
            var rowNo = currentCell.Row; var columnNo = currentCell.Column;
            var valuePerNumericAttribute = section.ValuePerNumericAttribute;
            var valuePerTextAttribute = section.ValuePerTextAttribute;
            var crs = CheckGetTextValue(valuePerTextAttribute, "CRS");
            worksheet.Cells[rowNo, columnNo++].Value = crs;            
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "COUNTY");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "SR");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "DISTRICT");
            var lastUnderScoreIndex = crs.LastIndexOf('_');
            var hyphenIndex = crs.IndexOf('-');
            var startSeg = crs.Substring(lastUnderScoreIndex + 1, hyphenIndex - lastUnderScoreIndex - 1);
            var endSeg = crs.Substring(hyphenIndex + 1);
            worksheet.Cells[rowNo, columnNo++].Value = startSeg;
            worksheet.Cells[rowNo, columnNo++].Value = endSeg;

            //calculate area
            double pavementLength = CheckGetValue(valuePerNumericAttribute, "SEGMENT_LENGTH");
            double pavementWidth = CheckGetValue(valuePerNumericAttribute, "WIDTH");

            worksheet.Cells[rowNo, columnNo].Style.Numberformat.Format = "0";
            worksheet.Cells[rowNo, columnNo++].Value = pavementLength;
            worksheet.Cells[rowNo, columnNo].Value = pavementWidth;
            ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo++], ExcelHelperCellFormat.Number);            
            worksheet.Cells[rowNo, columnNo].Value = CheckGetValue(valuePerNumericAttribute, "PAVED_THICKNESS");
            ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo++], ExcelHelperCellFormat.DecimalPrecision2);
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "DIRECTION");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetValue(valuePerNumericAttribute, "LANES");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "BUSIPLAN");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "FAMILY");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetTextValue(valuePerTextAttribute, "MPO_RPO");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetValue(valuePerNumericAttribute, "SURFACEID").ToString() + "-" + CheckGetTextValue(valuePerTextAttribute, "SURFACE_NAME");

            worksheet.Cells[rowNo, columnNo++].Value = Year;
            worksheet.Cells[rowNo, columnNo++].Value = treatment?.TreatmentName ?? "";

            var treatmentCost = Math.Round(Convert.ToDecimal(treatment?.Cost ?? 0), 0);
            worksheet.Cells[rowNo, columnNo].Style.Numberformat.Format = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";
            worksheet.Cells[rowNo, columnNo++].Value = treatmentCost;

            var cashFlowPerYr = "1 / " + string.Format("{0:C0}", treatmentCost);
            worksheet.Cells[rowNo, columnNo++].Value = cashFlowPerYr;

            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(CheckGetValue(valuePerNumericAttribute, "OPI_CALCULATED")));
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(CheckGetValue(valuePerNumericAttribute, "ROUGHNESS")), 2);
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetValue(valuePerNumericAttribute, "YR_BUILT");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetValue(valuePerNumericAttribute, "YEAR_LAST_OVERLAY");
            worksheet.Cells[rowNo, columnNo++].Value = CheckGetValue(valuePerNumericAttribute, "LAST_STRUCTURAL_OVERLAY");
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(CheckGetValue(valuePerNumericAttribute, "AADT"));
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(CheckGetValue(valuePerNumericAttribute, "TRK_PERCENT"));
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(CheckGetValue(valuePerNumericAttribute, "RISKSCORE")), 2);

            if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo - 1], Color.LightGray); }
            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, 1, rowNo, columnNo - 1]);
            currentCell.Column = columnNo;
        }

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _summaryReportHelper.checkAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) => _summaryReportHelper.checkAndGetValue<string>(valuePerTextAttribute, attribute);
    }
}
