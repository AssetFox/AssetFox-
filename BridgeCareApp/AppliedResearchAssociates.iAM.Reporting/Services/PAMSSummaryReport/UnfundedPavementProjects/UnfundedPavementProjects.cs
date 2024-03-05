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
        private double PAVEMENT_AREA_THRESHOLD = 0;

        public UnfundedPavementProjects()
        {
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }
        }

        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = AddHeadersCells(unfundedRecommendationWorksheet);

            // Enable Auto Filter
            using (var autoFilterCells = unfundedRecommendationWorksheet.Cells[1, 1, currentCell.Row, currentCell.Column])
            {
                autoFilterCells.AutoFilter = true;
            }

            //set alignment
            unfundedRecommendationWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

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
                ExcelHelper.MergeCells(worksheet, headerRowIndex, column + 1, headerRowIndex + 1, column + 1);
            }
            
            var currentCell = new CurrentCell { Row = headerRowIndex + 1, Column = worksheet.Dimension.Columns };

            worksheet.Cells.AutoFitColumns();
            using (ExcelRange autoFilterCells = worksheet.Cells[2, 1, currentCell.Row, currentCell.Column])
            {
                autoFilterCells.AutoFilter = true;
            }

            worksheet.DefaultColWidth = 13;
            worksheet.Row(headerRowIndex).Height = 40;

            return currentCell;
        }

        private List<string> GetHeadersRow()
        {
            return new List<string>
            {
                "District",
                "County",
                "SR",
                "Section",
                "Pavement Length",
                "Pavement Area",
                "Lanes",
                "BPN",
                "MPO/RPO",
                "Risk Score",
                "State Contracted Funded",
                "Analysis Year",
                "Unfunded Treatment",
                "Cost",
                "Cash Flow Yrs/Amount",
                "OPI",
                "Roughness",
            };
        }


        public List<AssetDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections = simulationYearDetail.Assets.Where(s =>
            s.TreatmentCause == TreatmentCause.NoSelection
            //&& (!string.IsNullOrEmpty(s.ValuePerTextAttribute["NHS_IND"]) && !string.IsNullOrWhiteSpace(s.ValuePerTextAttribute["NHS_IND"]) && int.Parse(s.ValuePerTextAttribute["NHS_IND"]) == 1)
            && s.TreatmentOptions.Count > 0).ToList();
            return untreatedSections;
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, List<Tuple<SimulationYearDetail, AssetDetail, TreatmentOptionDetail>>>();
            var validFacilityIds = new List<int>(); // Unfunded IDs
            var firstYear = true;
            foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
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
                    validFacilityIds = validFacilityIds.Intersect(untreatedSections.Select(_ => Convert.ToInt32(_summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "CNTY") + _summaryReportHelper.checkAndGetValue<string>(_.ValuePerTextAttribute, "SR") + _.AssetName))).ToList();
                }

                foreach (var section in untreatedSections)
                {
                    var segmentNumber = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CNTY");
                    segmentNumber += _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "SR");
                    segmentNumber += section.AssetName;

                    var facilityId = Convert.ToInt32(segmentNumber);

                    var treatmentOptions = section.TreatmentOptions.Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
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

            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "DISTRICT");
            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "COUNTY");
            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "SR");

            var segmentNumber = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "CNTY");
            segmentNumber += _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "SR");
            segmentNumber += section.AssetName;
            worksheet.Cells[rowNo, columnNo++].Value = segmentNumber;

            //calculate area
            double pavementLength = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "SEGMENT_LENGTH");
            double pavementWidth = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "WIDTH");
            double pavementArea = pavementLength * pavementWidth;

            worksheet.Cells[rowNo, columnNo].Style.Numberformat.Format = "0";
            worksheet.Cells[rowNo, columnNo++].Value = pavementLength;

            worksheet.Cells[rowNo, columnNo].Style.Numberformat.Format = "0";
            worksheet.Cells[rowNo, columnNo++].Value = pavementArea;

            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "LANES");
            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "BUSIPLAN");
            worksheet.Cells[rowNo, columnNo++].Value = _summaryReportHelper.checkAndGetValue<string>(section.ValuePerTextAttribute, "MPO_RPO");
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "RISKSCORE")), 2);

            var stateContractedFunded = pavementArea >= PAVEMENT_AREA_THRESHOLD ? "Y" : "N"; stateContractedFunded += " - " + Year.ToString();
            worksheet.Cells[rowNo, columnNo++].Value = stateContractedFunded;

            worksheet.Cells[rowNo, columnNo++].Value = Year;
            worksheet.Cells[rowNo, columnNo++].Value = treatment?.TreatmentName ?? "";

            var treatmentCost = Math.Round(Convert.ToDecimal(treatment?.Cost ?? 0), 0);
            worksheet.Cells[rowNo, columnNo].Style.Numberformat.Format = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";
            worksheet.Cells[rowNo, columnNo++].Value = treatmentCost;

            var cashFlowPerYr = "1 / " + String.Format("{0:C0}", treatmentCost);
            worksheet.Cells[rowNo, columnNo++].Value = cashFlowPerYr;

            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "OPI_CALCULATED")));
            worksheet.Cells[rowNo, columnNo++].Value = Math.Round(Convert.ToDecimal(_summaryReportHelper.checkAndGetValue<double>(section.ValuePerNumericAttribute, "ROUGHNESS")), 2);

            if (rowNo % 2 == 0) { ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo - 1], Color.LightGray); }
            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, 1, rowNo, columnNo - 1]);
            currentCell.Column = columnNo;
        }
    }
}
