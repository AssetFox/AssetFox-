using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DTOs;
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

        public void Fill(ExcelWorksheet masWorksheet, SimulationOutput simulationOutput, Guid networkId, List<MaintainableAsset> networkMaintainableAssets, List<AttributeDatumDTO> attributeDatumDTOs)
        {
            var currentCell = AddHeadersCells(masWorksheet);

            FillDynamicDataInWorkSheet(simulationOutput, masWorksheet, currentCell, networkId, networkMaintainableAssets, attributeDatumDTOs);

            masWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            masWorksheet.Cells.AutoFitColumns();
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, ExcelWorksheet masWorksheet, CurrentCell currentCell, Guid networkId, List<MaintainableAsset> networkMaintainableAssets, List<AttributeDatumDTO> attributeDatumDTOs)
        {
            foreach (var networkMaintainableAsset in networkMaintainableAssets)
            {
                var assetId = networkMaintainableAsset.Id;

                // Generate data model
                var attributeDatumDTOsForAsset = attributeDatumDTOs.Where(_ => _.MaintainableAssetId == assetId).ToList();
                var masDataModel = GenerateMASDataModel(assetId, networkId, networkMaintainableAsset, attributeDatumDTOsForAsset);

                // Fill in excel
                currentCell = FillDataInWorksheet(masWorksheet, masDataModel, currentCell);

            }
            ExcelHelper.ApplyBorder(masWorksheet.Cells[1, 1, currentCell.Row - 1, currentCell.Column]);
        }

        private static CurrentCell FillDataInWorksheet(ExcelWorksheet masWorksheet, MASDataModel masDataModel, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;
                        
            masWorksheet.Cells[row, column++].Value = masDataModel.NetworkId;
            masWorksheet.Cells[row, column++].Value = masDataModel.MaintainableAssetId;
            masWorksheet.Cells[row, column++].Value = masDataModel.District;
            masWorksheet.Cells[row, column++].Value = masDataModel.Cnty;
            masWorksheet.Cells[row, column++].Value = masDataModel.Route;            
            masWorksheet.Cells[row, column++].Value = masDataModel.AssetName;
            masWorksheet.Cells[row, column++].Value = masDataModel.Direction;
            masWorksheet.Cells[row, column++].Value = masDataModel.FromSection;
            masWorksheet.Cells[row, column++].Value = masDataModel.ToSection;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.Area;            
            masWorksheet.Cells[row, column++].Value = masDataModel.Interstate;
            masWorksheet.Cells[row, column++].Value = masDataModel.Lanes;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.Width;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.Length;            
            masWorksheet.Cells[row, column++].Value = masDataModel.surfaceName;
            SetDecimalFormat(masWorksheet.Cells[row, column]);
            masWorksheet.Cells[row, column++].Value = masDataModel.RiskScore;

            return new CurrentCell { Row = ++row, Column = column - 1 }; ;
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private MASDataModel GenerateMASDataModel(Guid assetId, Guid networkId, MaintainableAsset networkMaintainableAsset, List<AttributeDatumDTO> attributeDatumDTOsForAsset)
        {
            MASDataModel masDataModel = new MASDataModel
            {
                NetworkId = networkId,
                MaintainableAssetId = assetId
            };

            var locationIdentifier = networkMaintainableAsset.Location?.LocationIdentifier;
            masDataModel.AssetName = locationIdentifier;
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
                        
            double pavementLength = GetNumericValue(attributeDatumDTOsForAsset, "SEGMENT_LENGTH");
            double pavementWidth = GetNumericValue(attributeDatumDTOsForAsset, "WIDTH");
            masDataModel.Length = pavementLength;
            masDataModel.Width = pavementWidth;
            masDataModel.Area = pavementLength * pavementWidth;
            masDataModel.District = GetTextValue(attributeDatumDTOsForAsset, "DISTRICT");
            masDataModel.Cnty = GetTextValue(attributeDatumDTOsForAsset, "CNTY");
            masDataModel.Route = GetTextValue(attributeDatumDTOsForAsset, "SR");
            masDataModel.Direction = GetTextValue(attributeDatumDTOsForAsset, "DIRECTION");            
            masDataModel.Interstate = GetTextValue(attributeDatumDTOsForAsset, "INTERSTATE");
            masDataModel.Lanes = GetNumericValue(attributeDatumDTOsForAsset, "LANES");
            masDataModel.surfaceName = GetTextValue(attributeDatumDTOsForAsset, "SURFACE_NAME");
            masDataModel.RiskScore = GetNumericValue(attributeDatumDTOsForAsset, "RISKSCORE");

            return masDataModel;
        }

        private double GetNumericValue(List<AttributeDatumDTO> attributeDatumDTOsForAsset, string attribute) => (double)attributeDatumDTOsForAsset.FirstOrDefault(_ => _.Attribute == attribute).NumericValue;

        private string GetTextValue(List<AttributeDatumDTO> attributeDatumDTOsForAsset, string attribute) => attributeDatumDTOsForAsset.FirstOrDefault(_ => _.Attribute == attribute).TextValue;

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
            worksheet.Cells[headerRow, column++].Value = "Asset";
            worksheet.Cells[headerRow, column++].Value = "Direction";
            worksheet.Cells[headerRow, column++].Value = "FromSection";
            worksheet.Cells[headerRow, column++].Value = "ToSection";
            worksheet.Cells[headerRow, column++].Value = "Area";
            worksheet.Cells[headerRow, column++].Value = "Interstate";
            worksheet.Cells[headerRow, column++].Value = "Lanes";
            worksheet.Cells[headerRow, column++].Value = "Width";
            worksheet.Cells[headerRow, column++].Value = "Length";            
            worksheet.Cells[headerRow, column++].Value = "SurfaceName";
            worksheet.Cells[headerRow, column++].Value = "Risk";

            return new CurrentCell { Row = ++headerRow, Column = column };
        }
    }
}
