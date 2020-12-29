﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using MoreLinq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace BridgeCareCore.Services.SummaryReport.UnfundedRecommendations
{
    public class UnfundedRecommendations : IUnfundedRecommendations
    {
        private readonly IExcelHelper _excelHelper;
        private readonly List<int> SimulationYears = new List<int>();

        public UnfundedRecommendations(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper;
        }

        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var headers = GetHeaders();
            simulationOutput.Years.ForEach(_ => SimulationYears.Add(_.Year));
            var currentCell = AddHeadersCells(unfundedRecommendationWorksheet, headers, SimulationYears);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from top, left to right, and bottom set of data.
            using (var autoFilterCells = unfundedRecommendationWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }
            AddDynamicDataCells(unfundedRecommendationWorksheet, SimulationYears, simulationOutput, currentCell);

            unfundedRecommendationWorksheet.Cells.AutoFitColumns();
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
                        if(section.TreatmentCause != TreatmentCause.NoSelection)
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

            worksheet.Cells[row, columnNo++].Value = sectionSummary.SectionName;
            worksheet.Cells[row, columnNo++].Value = sectionSummary.FacilityName;
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["DISTRICT"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BRIDGE_TYPE"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["DECK_AREA"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["LENGTH"];
            columnNo++; // temporary, because we can commented out 1 excel rows

            //worksheet.Cells[rowNo, columnNo++].Value = bridgeDataModel.PlanningPartner;
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FAMILY_ID"];
            worksheet.Cells[row, columnNo++].Value = int.TryParse(sectionSummary.ValuePerTextAttribute["NHS_IND"],
                    out var numericValue) && numericValue > 0 ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["STRUCTURE_TYPE"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerTextAttribute["FUNC_CLASS"]; 
            worksheet.Cells[row, columnNo++].Value = (int)sectionSummary.ValuePerNumericAttribute["YEAR_BUILT"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["AGE"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["ADTTOTAL"];
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["ADTTOTAL"] > 10000 ? "Y" : "N";
            worksheet.Cells[row, columnNo++].Value = sectionSummary.ValuePerNumericAttribute["RISK_SCORE"];
            worksheet.Cells[row, columnNo].Value = sectionSummary.ValuePerNumericAttribute["P3"] > 0 ? "Y" : "N";

            currentCell.Column = columnNo;
        }
        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> headers, List<int> simulationYears)
        {
            int headerRow = 1;
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
            }
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            _excelHelper.ApplyBorder(worksheet.Cells[headerRow, 1, headerRow + 1, worksheet.Dimension.Columns]);

            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            return currentCell;
        }
        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Feasible ";
            var column = currentCell.Column;
            var row = currentCell.Row;
            var initialColumn = column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, ++column].Value = HeaderConstText + year + " Treatment";
                worksheet.Cells[row + 2, column].Value = year;
                _excelHelper.ApplyStyle(worksheet.Cells[row + 2, column]);
                _excelHelper.ApplyColor(worksheet.Cells[row, column], Color.FromArgb(244, 176, 132));
            }
            // Merge 2 rows for headers
            worksheet.Row(row).Height = 40;
            for (int cellColumn = 1; cellColumn < column + 1; cellColumn++)
            {
                _excelHelper.MergeCells(worksheet, row, cellColumn, row + 1, cellColumn);
            }

            _excelHelper.ApplyBorder(worksheet.Cells[row, initialColumn, row + 1, worksheet.Dimension.Columns]);
            currentCell.Row = currentCell.Row + 2;
        }
        private List<string> GetHeaders()
        {
            return new List<string>
            {
                "BridgeID",
                "BRKey",
                "District",
                "Bridge (B/C)",
                "Deck Area",
                "Structure Length",
                "Planning Partner",
                "Bridge Family",
                "NHS",
                "BPN",
                "Struct Type",
                "Functional Class",
                "Year Built",
                "Age",
                "ADTT",
                "ADT Over 10,000",
                "Risk Score",
                "P3"
            };
        }
        #endregion
    }
}
