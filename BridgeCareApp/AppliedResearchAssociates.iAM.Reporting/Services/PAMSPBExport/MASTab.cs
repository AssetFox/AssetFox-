using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSPBExport
{
    public class MASTab
    {
        private ReportHelper _reportHelper;

        public MASTab()
        {
            _reportHelper = new ReportHelper();
        }

        public void Fill(ExcelWorksheet masWorksheet, SimulationOutput simulationOutput, Guid simulationId, Guid networkId, List<MaintainableAsset> networkMaintainableAssets)
        {
            var currentCell = AddHeadersCells(masWorksheet);

            //  FillDynamicDataInWorkSheet(simulationOutput, masWorksheet, currentCell, simulationId, networkId, networkMaintainableAssets);

            masWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            masWorksheet.Cells.AutoFitColumns();
        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            int headerRow = 1;
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow, column++].Value = "NetworkID";
            worksheet.Cells[headerRow, column++].Value = "AssetId";
            worksheet.Cells[headerRow, column++].Value = "District";
            worksheet.Cells[headerRow, column++].Value = "Cnty";
            worksheet.Cells[headerRow, column++].Value = "Route";
            worksheet.Cells[headerRow, column++].Value = "Asset";
            worksheet.Cells[headerRow, column++].Value = "Direction";
            worksheet.Cells[headerRow, column++].Value = "FromSection";
            worksheet.Cells[headerRow, column++].Value = "ToSection";
            worksheet.Cells[headerRow, column++].Value = "Area";
            worksheet.Cells[headerRow, column++].Value = "Interstate";
            worksheet.Cells[headerRow, column++].Value = "Lanes";
            worksheet.Cells[headerRow, column++].Value = "Width";
            worksheet.Cells[headerRow, column++].Value = "Length";
            worksheet.Cells[headerRow, column++].Value = "CRS";
            worksheet.Cells[headerRow, column++].Value = "SurfaceName";
            worksheet.Cells[headerRow, column++].Value = "Risk";

            return new CurrentCell { Row = ++headerRow, Column = worksheet.Dimension.Columns + 1 };
        }
    }
}
