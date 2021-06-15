using System;
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
        private readonly ISummaryReportHelper _summaryReportHelper;
        private readonly List<int> SimulationYears = new List<int>();

        public UnfundedTreatmentTime(IExcelHelper excelHelper, ISummaryReportHelper summaryReportHelper)
        {
            _excelHelper = excelHelper;
            _summaryReportHelper = summaryReportHelper;
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
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, List<Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>>>();
            foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
            {
                var untreatedSections =
                    year.Sections.Where(
                        sect => sect.TreatmentCause == TreatmentCause.NoSelection &&
                        sect.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
                        sect.TreatmentOptions.Count > 0
                        ).ToList();

                foreach (var section in untreatedSections)
                {
                    var facilityId = int.Parse(section.FacilityName);

                    var filteredOptions = section.TreatmentOptions.
                        Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                    filteredOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));

                    var newTuple = new Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>(year, section, filteredOptions.FirstOrDefault());
                    if (!treatmentsPerSection.ContainsKey(facilityId))
                    {
                        treatmentsPerSection.Add(facilityId, new List<Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>> { newTuple });
                    }
                    else
                    {
                        treatmentsPerSection[facilityId].Add(newTuple);
                    }
                }
            }

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

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



        private void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionDetail section, int Year, TreatmentOptionDetail treatment)
        {
            var row = currentCell.Row;
            var columnNo = currentCell.Column;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["DISTRICT"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["COUNTY"];
            worksheet.Cells[row, columnNo++].Value = section.FacilityName;

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
            var deckArea = section.ValuePerNumericAttribute["DECK_AREA"];
            worksheet.Cells[row, columnNo++].Value = deckArea;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["LENGTH"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["MPO_NAME"];

            var functionalClassAbbr = section.ValuePerTextAttribute["FUNC_CLASS"];
            var functionalClassDescription = _summaryReportHelper.FullFunctionalClassDescription(functionalClassAbbr);
            worksheet.Cells[row, columnNo++].Value = functionalClassDescription;

            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["NHS_IND"] == "0" ? "N" : "Y";
            worksheet.Cells[row, columnNo++].Value = section.ValuePerTextAttribute["INTERSTATE"];

            worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.00";
            worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["RISK_SCORE"];

            worksheet.Cells[row, columnNo++].Value = deckArea >= 28500 ? "Y" : "N";

            // ----------------------------------------------
            // TODO: FIX THESE WHEN "FEDAID" PARAMETER IS AVAILABLE
            //
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding185(section) ? " " : " "; // "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding581(section) ? " " : " "; // "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingSTP(section) ? " " : " "; // "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingNHPP(section) ? " " : " "; // "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFundingBOF(section) ? " " : " "; // "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = _summaryReportHelper.BridgeFunding183(section) ? " " : " "; // "Y" : "N";
            //
            // ----------------------------------------------

            worksheet.Cells[row, columnNo++].Value = Year;

            var familyId = int.Parse(section.ValuePerTextAttribute["FAMILY_ID"]);
            if (familyId < 11)
            {
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = section.ValuePerNumericAttribute["DECK_SEEDED"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = section.ValuePerNumericAttribute["SUP_SEEDED"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = section.ValuePerNumericAttribute["SUB_SEEDED"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_SEEDED     section.ValuePerNumericAttribute["CULV_SEEDED"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["DECK_DURATION_N"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUP_DURATION_N"];
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["SUB_DURATION_N"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = "N"; // CULV_DURATION_N
            }
            else
            {
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = "N"; // DECK_SEEDED
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = "N"; // SUP_SEEDED
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, ++columnNo].Value = "N"; // SUB_SEEDED

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0.000";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["CULV_SEEDED"];

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo + 2].Value = "N"; // DECK_DURATION_N
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo + 3].Value = "N"; // SUP_DURATION_N
                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo + 4].Value = "N"; // SUB_DURATION_N

                worksheet.Cells[row, columnNo].Style.Numberformat.Format = "0";
                worksheet.Cells[row, columnNo++].Value = section.ValuePerNumericAttribute["CULV_DURATION_N"];
            }

            worksheet.Cells[row, columnNo++].Value = treatment?.TreatmentName;
            worksheet.Cells[row, columnNo].Style.Numberformat.Format = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";
            worksheet.Cells[row, columnNo++].Value = treatment?.Cost;

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
