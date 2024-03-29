using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.NetworkExportReport
{
    public class NetworkTab
    {
        public void Fill(ExcelWorksheet worksheet, List<Data.Networking.MaintainableAsset> maintainableAssets, Dictionary<Guid, List<(string, string)>> aggregatedResults, List<string> attributes)
        {
            var currentCell = AddHeadersCells(worksheet, attributes);
            FillDynamicDataInWorkSheet(worksheet, currentCell, maintainableAssets, aggregatedResults, attributes);
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            worksheet.Cells.AutoFitColumns();
        }

        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<string> attributes)
        {
            int headerRow = 1;
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow, column++].Value = "MaintainableAssetID";
            foreach (var attr in attributes)
                worksheet.Cells[headerRow, column++].Value = attr;

            return new CurrentCell { Row = ++headerRow, Column = column };
        }

        private void FillDynamicDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, List<Data.Networking.MaintainableAsset> maintainableAssets, Dictionary<Guid, List<(string, string)>> aggregatedResults, List<string> uniqueAttributeNames)
        {
            foreach (var asset in maintainableAssets)
            {
                aggregatedResults.TryGetValue(asset.Id, out var aggregatedResultsForAsset);

                var networkModel = GenerateNetworkModel(asset.Id, aggregatedResultsForAsset);
                currentCell = FillDataInWorksheet(worksheet, networkModel, currentCell, aggregatedResults.Select(_ => _.Attribute).ToList(), uniqueAttributeNames);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[1, 1, currentCell.Row - 1, currentCell.Column]);
        }

        private NetworkExportReportModel GenerateNetworkModel(Guid maintainableAssetID, List<(string, string)> aggregatedResultsForAsset)
        {
            var networkModel = new NetworkExportReportModel() {
                MaintainableAssetID = maintainableAssetID,
                Attributes = aggregatedResultsForAsset.ToDictionary(_ => _.Item1, _ => _.Item2)
            };
            return networkModel;
        }

        private CurrentCell FillDataInWorksheet(ExcelWorksheet worksheet, NetworkExportReportModel networkModel, CurrentCell currentCell, List<AttributeDTO> attributeDTOs, List<string> uniqueAttributeNames)
        {
            var row = currentCell.Row;
            int column = 1;
            worksheet.Cells[row, column++].Value = networkModel.MaintainableAssetID;

            foreach (var attrName in uniqueAttributeNames)
            {
                SetDecimalFormat(worksheet.Cells[row, column]);
                var val = "";
                if (networkModel.Attributes.TryGetValue(attrName, out val))  
                    worksheet.Cells[row, column++].Value = val;
                else
                    worksheet.Cells[row, column++].Value = attributeDTOs.FirstOrDefault(_ => _.Name == attrName).DefaultValue;
            }

            return new CurrentCell { Row = ++row, Column = column - 1 };
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);
    }
}
