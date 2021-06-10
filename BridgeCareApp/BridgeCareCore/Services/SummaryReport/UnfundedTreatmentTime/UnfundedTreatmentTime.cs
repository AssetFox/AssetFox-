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


        private static readonly Dictionary<string, string> FunctionalClassDescriptions =
            new Dictionary<string, string>()
            {
                {"01", "Rural Interstate"},
                {"02", "Rural - Other Principal"},
                {"06", "Rural Minor Arterial"},
                {"07", "Rural Major Collector"},
                {"08", "Rural Minor Collector"},
                {"09", "Rural Local"},
                {"11", "Urban Interstate"},
                {"12", "Urban Other Freeway/Expressway"},
                {"14", "Urban Other Principal"},
                {"16", "Urban Minor Arterial"},
                {"17", "Urban Major Collector"},
                {"19", "Urban Local"},
                {"NN", "Other"},
                {"99", "Ramp" },
            };

        private class dataRow
        {
            //
        }

        public UnfundedTreatmentTime(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper;
        }

        public void Fill(ExcelWorksheet unfundedTreatmentTimeWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = AddHeadersCells(unfundedTreatmentTimeWorksheet);

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
            //// cache to store whether "No Treatment" is present for all of the years for a given section.
            //var treatmentDecisionPerSection = new Dictionary<int, bool>();
            //var nonSelectedFacilities = new HashSet<int>();
            //foreach (var year in simulationOutput.Years)
            //{
            //    foreach (var section in year.Sections)
            //    {
            //        var facilityId = int.Parse(section.FacilityName);
            //        if (!treatmentDecisionPerSection.ContainsKey(facilityId))
            //        {
            //            treatmentDecisionPerSection.Add(facilityId, false);
            //        }
            //        if (section.ValuePerNumericAttribute["RISK_SCORE"] > 15000
            //            && section.TreatmentConsiderations.Count > 0)
            //        {
            //            if (section.TreatmentCause == TreatmentCause.NoSelection && !nonSelectedFacilities.Contains(facilityId))
            //            {
            //                treatmentDecisionPerSection[facilityId] = true;
            //            }
            //            if (section.TreatmentCause != TreatmentCause.NoSelection)
            //            {
            //                treatmentDecisionPerSection[facilityId] = false;
            //                nonSelectedFacilities.Add(facilityId);
            //            }
            //        }
            //    }
            //}

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;
            //foreach (var initialSection in simulationOutput.InitialSectionSummaries)
            //{
            //    //if (initialSection.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
            //    //    treatmentDecisionPerSection[int.Parse(initialSection.FacilityName)] == true)
            //    //{
            //        currentCell.Column = 1;
            //        FillDataInWorkSheet(worksheet, currentCell, initialSection);
            //        currentCell.Row++;
            //    //}
            //}

            //currentCell.Row = 4; // Data starts here
            //currentCell.Column += 1; // feasible treatment starts here
            foreach (var item in simulationOutput.Years)
            {
                foreach (var section in item.Sections)
                {
                    if (section.TreatmentCause == TreatmentCause.NoSelection)
                    {
                        FillDataInWorkSheet(worksheet, currentCell, section, item.Year);
                        currentCell.Row++;
                        currentCell.Column = 1;
                    }


                //    if (section.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
                //        treatmentDecisionPerSection[int.Parse(section.FacilityName)] == true
                //        )
                //    {
                //        var filteredOptions = section.TreatmentOptions.
                //            Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                //        filteredOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));

                //        var requiredTreatmentName = filteredOptions.Count > 0 ? filteredOptions.FirstOrDefault().TreatmentName
                //            : "";

                //        worksheet.Cells[currentCell.Row, currentCell.Column].Value = requiredTreatmentName;
                //        currentCell.Row++;
                //    }
                }
                //currentCell.Column++;
                //currentCell.Row = 4;
            }
        }

        private void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionSummaryDetail sectionSummary, int Year)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DISTRICT"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["COUNTY"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.FacilityName;

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            var deckArea = sectionSummary.ValuePerNumericAttribute["DECK_AREA"];
            worksheet.Cells[row, columnNo++].Value = deckArea;

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LENGTH"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["MPO_NAME"];

            var functionalClassAbbr = sectionSummary.ValuePerTextAttribute["FUNC_CLASS"];
            var functionalClassDescription = FunctionalClassDescriptions.ContainsKey(functionalClassAbbr) ? $"{functionalClassAbbr} - {FunctionalClassDescriptions[functionalClassAbbr]}" : functionalClassAbbr;
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["NHS_IND"] == "0" ? "N" : "Y";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["INTERSTATE"];

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.00";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISK_SCORE"];

            worksheet.Cells[row, columnNo++].Value = deckArea >= 28500 ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding 185
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding 581
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding STP
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding NHPP
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding BOF
            worksheet.Cells[row, columnNo++].Value = "??"; // Bridge Funding 183

            worksheet.Cells[row, columnNo++].Value = Year;

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_SEEDED"];
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUP_SEEDED"];
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUB_SEEDED"];
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["CULV_SEEDED"];

            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUP_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["SUB_DURATION_N"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["CULV_DURATION_N"];


            worksheet.Cells[row, columnNo++].Value = "??"; // Unfunded Treatment
            worksheet.Cells[row, columnNo++].Value = "??"; // Cost

            currentCell.Column = columnNo;
        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRow = 1;
            var headersRow1 = GetHeadersRow1();
            var headersRow2 = GetHeadersRow2();

            var bridgeFundingColumn = headersRow1.IndexOf(BRIDGE_FUNDING) + 1;
            var analysisColumn = bridgeFundingColumn + headersRow2.Count;

            worksheet.Cells.Style.WrapText = false;

            // Add all Row 1 headers
            for (int column = 0; column < headersRow1.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headersRow1[column];
            }

            // Add Bridge Funding cells for Row 2
            for (int column = 0; column < headersRow2.Count; column++)
            {
                worksheet.Cells[headerRow + 1, column + bridgeFundingColumn].Value = headersRow2[column];
            }

            var row = headerRow;

            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;
            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);

            // Merge Bridge Funding cells in Row 1
            _excelHelper.MergeCells(worksheet, row, bridgeFundingColumn, row, analysisColumn - 1);

            // Merge 2 rows for headers EXCEPT Bridge Funding columns

            // Merge rows for Columns prior to Bridge Funding
            for (int cellColumn = 1; cellColumn < bridgeFundingColumn; cellColumn++)
            {
                _excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            // Merge rows for Columns after Bridge Funding
            for (int cellColumn = analysisColumn; cellColumn <= worksheet.Dimension.Columns; cellColumn++)
            {
                _excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            _excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);
            _excelHelper.ApplyStyle(worksheet.Cells[headerRow + 1, bridgeFundingColumn, headerRow + 1, analysisColumn - 1]);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            var currentCell = new CurrentCell { Row = headerRow + 2, Column = worksheet.Dimension.Columns + 1};
            return currentCell;
        }

        private const string BRIDGE_FUNDING = "Bridge Funding";

        private List<string> GetHeadersRow1()
        {
            return new List<string>
            {
                "District",
                "County",
                "BRKey",
                "Deck\r\nArea",
                "Bridge\r\nLength",
                "BPN",
                "MPO/RPO",
                "Functional\r\nClass",
                "NHS",
                "Interstate",
                "Risk\r\nScore",
                "Large\r\nBridge",

                BRIDGE_FUNDING, // row 1 header for six sub-sections
                "",
                "",
                "",
                "",
                "",

                "Analysis\r\nYear",
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

        private List<string> GetHeadersRow2()
        {
            return new List<string>
            {
                "185", // Six sub-sections for "Bridge Funding"
                "581",
                "STP",
                "NHPP",
                "BOF",
                "183",
            };
        }

        #endregion Private methods
    }
}
