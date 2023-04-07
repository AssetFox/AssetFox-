using System;
using System.Collections.Generic;
using System.Linq;
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

            FillDynamicDataInWorkSheet(simulationOutput, masWorksheet, currentCell, simulationId, networkId, networkMaintainableAssets);

            masWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            masWorksheet.Cells.AutoFitColumns();
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, ExcelWorksheet masWorksheet, CurrentCell currentCell, Guid simulationId, Guid networkId, List<MaintainableAsset> networkMaintainableAssets)
        {
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                // Generate data model                    
                var masDataModel = GenerateMASDataModel(initialAssetSummary, simulationId, networkId, networkMaintainableAssets);

                // Fill in excel
                currentCell = FillDataInWorksheet(masWorksheet, masDataModel, currentCell);
               
            }
            ExcelHelper.ApplyBorder(masWorksheet.Cells[1, 1, currentCell.Row - 1, currentCell.Column]);
        }

        private CurrentCell FillDataInWorksheet(ExcelWorksheet masWorksheet, MASDataModel masDataModel, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;
                        
            masWorksheet.Cells[row, column++].Value = masDataModel.NetworkId;
            masWorksheet.Cells[row, column++].Value = masDataModel.MaintainableAssetId;
            masWorksheet.Cells[row, column++].Value = masDataModel.District;
            masWorksheet.Cells[row, column++].Value = masDataModel.Cnty;
            masWorksheet.Cells[row, column++].Value = masDataModel.Route;
            // masWorksheet.Cells[row, column++].Value = masDataModel.AssetName; // TODO Remove once confirmed from Dmitry
            masWorksheet.Cells[row, column++].Value = masDataModel.Direction;
            masWorksheet.Cells[row, column++].Value = masDataModel.FromSection;
            masWorksheet.Cells[row, column++].Value = masDataModel.ToSection;
            masWorksheet.Cells[row, column++].Value = masDataModel.Area;            
            masWorksheet.Cells[row, column++].Value = masDataModel.Interstate;
            masWorksheet.Cells[row, column++].Value = masDataModel.Lanes;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.Width;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.Length;
            masWorksheet.Cells[row, column++].Value = masDataModel.CRS;
            masWorksheet.Cells[row, column++].Value = masDataModel.surfaceName;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.RiskScore;

            return new CurrentCell { Row = ++row, Column = column - 1 }; ;
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private MASDataModel GenerateMASDataModel(AssetSummaryDetail initialAssetSummary, Guid simulationId, Guid networkId, List<MaintainableAsset> networkMaintainableAssets)
        {
            var assetId = initialAssetSummary.AssetId;
            MASDataModel masDataModel = new MASDataModel
            {
                NetworkId = networkId,
                MaintainableAssetId = assetId,
            };

            var locationIdentifier = networkMaintainableAssets.FirstOrDefault(_ => _.Id == assetId)?.Location?.LocationIdentifier;
            masDataModel.AssetName = locationIdentifier;
            masDataModel.CRS = locationIdentifier;
            var fromSection = string.Empty;
            var toSection = string.Empty;
            if (!string.IsNullOrEmpty(locationIdentifier))
            {
                var parts = locationIdentifier.Split(new char[] { '_' }).Last();
                var fromTo = parts.Split('-');
                fromSection = fromTo?.First();
                toSection = fromTo?.Last();
            }
            masDataModel.FromSection = fromSection;
            masDataModel.ToSection = toSection;            

            var valuePerTextAttribute = initialAssetSummary.ValuePerTextAttribute;
            var valuePerNumericAttribute = initialAssetSummary.ValuePerNumericAttribute;

            double pavementLength = CheckGetNumericValue(valuePerNumericAttribute, "SEGMENT_LENGTH");
            double pavementWidth = CheckGetNumericValue(valuePerNumericAttribute, "WIDTH");
            masDataModel.Length = pavementLength;
            masDataModel.Width = pavementWidth;
            masDataModel.Area = pavementLength * pavementWidth;
            masDataModel.District = CheckGetTextValue(valuePerTextAttribute, "DISTRICT");
            masDataModel.Cnty = CheckGetTextValue(valuePerTextAttribute, "CNTY");
            masDataModel.Route = CheckGetTextValue(valuePerTextAttribute, "SR");
            masDataModel.Direction = CheckGetTextValue(valuePerTextAttribute, "DIRECTION");            
            masDataModel.Interstate = CheckGetTextValue(valuePerTextAttribute, "INTERSTATE");
            masDataModel.Lanes = CheckGetNumericValue(valuePerNumericAttribute, "LANES");
            masDataModel.Interstate = CheckGetTextValue(valuePerTextAttribute, "SURFACE_NAME");
            masDataModel.RiskScore = CheckGetNumericValue(valuePerNumericAttribute, "RISKSCORE");

            return masDataModel;
        }

        private double CheckGetNumericValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) =>
          _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);

        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            int headerRow = 1;
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow, column++].Value = "NetworkID";
            worksheet.Cells[headerRow, column++].Value = "AssetId";
            worksheet.Cells[headerRow, column++].Value = "District";
            worksheet.Cells[headerRow, column++].Value = "Cnty";
            worksheet.Cells[headerRow, column++].Value = "Route";
            // worksheet.Cells[headerRow, column++].Value = "Asset"; // TODO Remove once confirmed from Dmitry
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

            return new CurrentCell { Row = ++headerRow, Column = column };
        }
    }
}
