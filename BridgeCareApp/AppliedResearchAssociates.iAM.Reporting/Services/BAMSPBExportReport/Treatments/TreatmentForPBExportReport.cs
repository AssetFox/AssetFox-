using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSPBExportReport;
using AppliedResearchAssociates.iAM.Reporting.Models;

using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Utilities.Encoders;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSPBExportReport.Treatments
{
    public class TreatmentForPBExportReport : ITreatmentForPBExportReport
    {
        private ReportHelper _reportHelper;

        public TreatmentForPBExportReport()
        {
            _reportHelper = new ReportHelper();
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData)
        {
            //set default width
            worksheet.DefaultColWidth = 13;

            // Add data to excel.
            //var sectionHeaders = GetStaticSectionHeaders();
            var dataHeaders = GetStaticDataHeaders();

            //add header
            var currentCell = AddDataHeadersCells(worksheet, dataHeaders);

            //add data to cells
            FillTreatmentData(worksheet, reportOutputData, currentCell);
        }

        #region Private Methods

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
                "AssetType",
                "MaintainableAssetid",
                "AssetName",
                "District",
                "Cnty",
                "Route",
                "OddityDirection",
                "BRKEY",
                "BRIDGE_ID",
                "FromSection",
                "ToSection",
                "Offset",
                "OWNER_CODE",
                "INTERSTATE",
                "PreferredYear",
                "Appliedtreatment",
                "TreatmentFundingIgnoresSpendingLimit",
                "TreatmentStatusTreatmentCause",
                "Cost",
                "Benefit",
                "PriorityLevel",
                "RISK_SCORE",
                "RemainingLife",
                "SimulationOutputId",
                "SimulationId",
                "COUNTY"
            };
        }

        private CurrentCell AddDataHeadersCells(ExcelWorksheet worksheet, List<string> dataHeaders)
        {
            //section header
            int dataHeaderRow = 1; var totalNumOfColumns = 0; var cellBGColor = ColorTranslator.FromHtml("#FFFFFF"); //White
            var startColumn = 0; var endColumn = 0;

            //data header
            for (int column = 0; column < dataHeaders.Count; column++)
            {
                var dataHeaderText = dataHeaders[column];
                worksheet.Cells[dataHeaderRow, column + 1].Value = dataHeaderText;
            }

            var currentCell = new CurrentCell { Row = dataHeaderRow, Column = dataHeaders.Count };
            ExcelHelper.ApplyStyle(worksheet.Cells[dataHeaderRow + 1, dataHeaders.Count, dataHeaderRow + 1, dataHeaders.Count]);
            ExcelHelper.ApplyBorder(worksheet.Cells[dataHeaderRow, 1, dataHeaderRow + 1, worksheet.Dimension.Columns]);

            return currentCell;
        }

        private void FillTreatmentData(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell)
        {
            //var rowNo = currentCell.Row;
            //var columnNo = currentCell.Column;
            //foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
            //{
            //    rowNo++; columnNo = 1;

            //    //--------------------- Asset ID ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERNET_REPORT"); //Internet Report 

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BRIDGE_TYPE"); //Bridge (B/C)
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BMSID"); //Bridge ID

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "BRKEY_"); //BRKey
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    //--------------------- Ownership ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DISTRICT"); //District
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY"); //County

            //    var ownerName = ""; var ownerCode = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "OWNER_CODE"); //Owner Code
            //    if (!string.IsNullOrEmpty(ownerCode) && !string.IsNullOrWhiteSpace(ownerCode)) { ownerName = MappingContent.OwnerCodeForReport(ownerCode); }
            //    worksheet.Cells[rowNo, columnNo++].Value = ownerName;

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SUBM_AGENCY"); //Submitting Agency
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "MPO_NAME"); // Planning Partner

            //    //--------------------- Structure ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "LENGTH"); //Structure Length
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DECK_AREA"); //Deck Area
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "LARGE_BRIDGE"); //Large Bridge
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    var spanType = ""; var spanTypeName = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "SPANTYPE"); //Span Type
            //    if (!string.IsNullOrEmpty(spanTypeName) && !string.IsNullOrWhiteSpace(spanTypeName))
            //    {
            //        spanType = spanTypeName == SpanType.M.ToSpanTypeName() ? MappingContent.SpanTypeMap[SpanType.M] : MappingContent.SpanTypeMap[SpanType.S];
            //    }
            //    worksheet.Cells[rowNo, columnNo++].Value = spanType;

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FAMILY_ID"); //Bridge Family
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "STRUCTURE_TYPE"); //Structure Type

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FRACT_CRIT"); //Fractural Critical
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "PARALLEL") > 0 ? "Y" : "N"; //Parallel Structure
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    //--------------------- Network ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.FullFunctionalClassDescription(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FUNC_CLASS")); //Functional Class

            //    worksheet.Cells[rowNo, columnNo++].Value = int.TryParse(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NHS_IND"), out var numericValue) && numericValue > 0 ? "Y" : "N"; //NHS
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "NBISLEN"); //NBIS Len
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "BUS_PLAN_NETWORK"); //BPN
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "INTERSTATE"); //Interstate
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    //--------------------- Asset Attibutes ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = MappingContent.GetDeckSurfaceType(_reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DECKSURF_TYPE")); //Deck Surface Type

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "WS_SEEDED"); // Wearing Surface Cond
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_COND"); //Paint Cond
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "PAINT_EXTENT"); //Paint Ext
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = (int)_reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "YEAR_BUILT"); //Year Built
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "AGE"); //Age
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "ADTTOTAL"); //ADT
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "RISK_SCORE"); //Risk Score
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "DET_LENGTH"); //Detour Length
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "POST_STATUS") == 0 ? "OPEN" : "POSTED"; //Posting Status
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "SUFF_RATING"); //Suff Rating
            //    ExcelHelper.SetCustomFormat(worksheet.Cells[rowNo, columnNo - 1], ExcelHelperCellFormat.Number);

            //    //--------------------- Funding ---------------------
            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "HBRR_ELIG"); //HBRR Elig
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "P3") > 0 ? "Y" : "N"; //P3
            //    ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo, columnNo - 1]);

            //    worksheet.Cells[rowNo, columnNo++].Value = _reportHelper.CheckAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "FEDAID"); //Federal Aid

            //    _previousYearInitialMinC.Add(_reportHelper.CheckAndGetValue<double>(sectionSummary.ValuePerNumericAttribute, "MINCOND"));


            //    if (rowNo % 2 == 0)
            //    {
            //        ExcelHelper.ApplyColor(worksheet.Cells[rowNo, 1, rowNo, columnNo], Color.LightGray);
            //    }
            //}
            //currentCell.Row = rowNo;
            //currentCell.Column = columnNo;
        }

        #endregion Private Methods
    }
}
