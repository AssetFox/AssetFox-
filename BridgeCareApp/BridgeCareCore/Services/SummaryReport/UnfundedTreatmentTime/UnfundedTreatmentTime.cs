using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.UnfundedTreatmentTime
{
    public class UnfundedTreatmentTime : IUnfundedTreatmentTime
    {
        private readonly IExcelHelper _excelHelper;
        private readonly List<int> SimulationYears = new List<int>();

        public UnfundedTreatmentTime(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper;
        }

        public void Fill(ExcelWorksheet unfundedTreatmentTimeWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var headers = GetHeaders();
            var currentCell = AddHeadersCells(unfundedTreatmentTimeWorksheet, headers);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = unfundedTreatmentTimeWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }
            AddDynamicDataCells(unfundedTreatmentTimeWorksheet, SimulationYears, simulationOutput, currentCell);

            unfundedTreatmentTimeWorksheet.Cells.AutoFitColumns();
        }

        #region Private methods

        private void AddDynamicDataCells(ExcelWorksheet worksheet, List<int> simulationYears, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // cache to store whether "No Treatment" is present for all of the years for a given section.
            var treatmentDecisionPerSection = new Dictionary<int, bool>();
            var nonSelectedFacilities = new HashSet<int>();
            foreach (var year in simulationOutput.Years)
            {
                foreach (var section in year.Sections)
                {
                    var facilityId = int.Parse(section.FacilityName);
                    if (!treatmentDecisionPerSection.ContainsKey(facilityId))
                    {
                        treatmentDecisionPerSection.Add(facilityId, false);
                    }
                    if (section.ValuePerNumericAttribute["RISK_SCORE"] > 15000
                        && section.TreatmentConsiderations.Count > 0)
                    {
                        if (section.TreatmentCause == TreatmentCause.NoSelection && !nonSelectedFacilities.Contains(facilityId))
                        {
                            treatmentDecisionPerSection[facilityId] = true;
                        }
                        if (section.TreatmentCause != TreatmentCause.NoSelection)
                        {
                            treatmentDecisionPerSection[facilityId] = false;
                            nonSelectedFacilities.Add(facilityId);
                        }
                    }
                }
            }

            currentCell.Row = 4; // Data starts here
            currentCell.Column = 1;
            foreach (var initialSection in simulationOutput.InitialSectionSummaries)
            {
                if (initialSection.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
                    treatmentDecisionPerSection[int.Parse(initialSection.FacilityName)] == true)
                {
                    currentCell.Column = 1;
                    FillDataInWorkSheet(worksheet, currentCell, initialSection);
                    currentCell.Row++;
                }
            }

            currentCell.Row = 4; // Data starts here
            currentCell.Column += 1; // feasible treatment starts here
            foreach (var item in simulationOutput.Years)
            {
                foreach (var section in item.Sections)
                {
                    if (section.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
                        treatmentDecisionPerSection[int.Parse(section.FacilityName)] == true
                        )
                    {
                        var filteredOptions = section.TreatmentOptions.
                            Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                        filteredOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));

                        var requiredTreatmentName = filteredOptions.Count > 0 ? filteredOptions.FirstOrDefault().TreatmentName
                            : "";

                        worksheet.Cells[currentCell.Row, currentCell.Column].Value = requiredTreatmentName;
                        currentCell.Row++;
                    }
                }
                currentCell.Column++;
                currentCell.Row = 4;
            }
        }

        private void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionSummaryDetail sectionSummary)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DISTRICT"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["COUNTY"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.FacilityName;
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_AREA"];

            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["MPO_NAME"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FUNC_CLASS"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["NHS_IND"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["INTERSTATE"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISK_SCORE"];

            worksheet.Cells[row, columnNo++].Value = "??"; // Large Bridge
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding
            worksheet.Cells[row, columnNo++].Value = "??"; // Analysis

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUP"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUB"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["CULV"];

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUP_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUB_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["CULV_DURATION_N"];


            worksheet.Cells[row, columnNo++].Value = sectionSummary.SectionName;
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BRIDGE_TYPE"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LENGTH"];
            columnNo++; // temporary, because we can commented out 1 excel rows

            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FAMILY_ID"];
            //worksheet.Cells[row, columnNo++].Value = int.TryParse(sectionSummary.ValuePerTextAttribute["NHS_IND"],
            //        out var numericValue) && numericValue > 0 ? "Y" : "N";
            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["STRUCTURE_TYPE"];
            //worksheet.Cells[row, columnNo++].Value = (int)sectionSummary.ValuePerNumericAttribute["YEAR_BUILT"];
            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["AGE"];
            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["ADTTOTAL"];
            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["ADTTOTAL"] > 10000 ? "Y" : "N";
            //worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISK_SCORE"];
            //worksheet.Cells[row, columnNo].Value = sectionSummary.ValuePerNumericAttribute["P3"] > 0 ? "Y" : "N";

            currentCell.Column = columnNo;
        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            _excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);

            var lastColumn = headers.Count;
            var row = headerRow;

            // Merge 2 rows for headers
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn <= lastColumn; cellColumn++)
            {
                _excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            return currentCell;
        }

        private List<string> GetHeaders()
        {
            //return new List<string>
            //{
            //    "BridgeID",
            //    "BRKey",
            //    "District",
            //    "Bridge (B/C)",
            //    "Deck Area",
            //    "Structure Length",
            //    "Planning Partner",
            //    "Bridge Family",
            //    "NHS",
            //    "BPN",
            //    "Struct Type",
            //    "Functional Class",
            //    "Year Built",
            //    "Age",
            //    "ADTT",
            //    "ADT Over 10,000",
            //    "Risk Score",
            //    "P3"
            //};
            return new List<string>
            {
                "District",
                "County",
                "BRKey",
                "Deck Area",
                "Bridge",
                "BPN",
                "MPO/RPO",
                "Functional Class",
                "NHS",
                "Interstate",
                "Risk Score",
                "Large Bridge",
                "Bridge Funding", // Six sub-sections
                "Analysis",
                "GCR DECK",
                "GCR SUP",
                "GCR SUB",
                "GCR CULV",
                "DECK DUR",
                "SUP DUR",
                "SUB DUR",
                "CULV DUR",
                "Unfunded Treatment",
                "Cost",
            };
        }

        #endregion Private methods
    }
}
