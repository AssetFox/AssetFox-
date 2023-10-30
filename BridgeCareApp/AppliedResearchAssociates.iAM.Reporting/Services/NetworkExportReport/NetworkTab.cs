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
        public void Fill(ExcelWorksheet worksheet, List<Data.Networking.MaintainableAsset> maintainableAssets, List<AggregatedResultDTO> aggregatedResults)
        {
            var currentCell = AddHeadersCells(worksheet, aggregatedResults.Select(_ => _.Attribute).OrderBy(_ => _.Name).ToList());
            FillDynamicDataInWorkSheet(worksheet, currentCell, maintainableAssets, aggregatedResults.OrderBy(_ => _.Attribute.Name).ToList(), aggregatedResults.OrderBy(_ => _.Attribute.Name).Select(_ => _.Attribute.Name).Distinct().ToList());
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            worksheet.Cells.AutoFitColumns();
        }

        private static CurrentCell AddHeadersCells(ExcelWorksheet worksheet, List<AttributeDTO> attributeDTOs)
        {
            int headerRow = 1;
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow, column++].Value = "MaintainableAssetID";
            foreach (var attr in attributeDTOs.Select(_ => _.Name).Distinct().ToList())
                worksheet.Cells[headerRow, column++].Value = attr;

            return new CurrentCell { Row = ++headerRow, Column = column };
        }

        private void FillDynamicDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, List<Data.Networking.MaintainableAsset> maintainableAssets, List<AggregatedResultDTO> aggregatedResults, List<string> uniqueAttributeNames)
        {
            foreach (var asset in maintainableAssets)
            {
                var aggregatedResultsForAsset = aggregatedResults.Where(_ => _.MaintainableAssetId == asset.Id).ToList();
                var attributes = aggregatedResultsForAsset.Select(_ => _.Attribute).ToList();
                var values = new List<string>();

                foreach (var result in aggregatedResultsForAsset)
                {
                    if (result.TextValue != null) values.Add(result.TextValue);
                    else if (result.NumericValue != null) values.Add(result.NumericValue.ToString());
                    else values.Add(attributes.FirstOrDefault(_ => _.Name == result.Attribute.Name).DefaultValue);
                }

                var networkModel = GenerateNetworkModel(asset.Id, attributes, values);
                currentCell = FillDataInWorksheet(worksheet, networkModel, currentCell, aggregatedResults.Select(_ => _.Attribute).ToList(), uniqueAttributeNames);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[1, 1, currentCell.Row - 1, currentCell.Column]);
        }

        private NetworkExportReportModel GenerateNetworkModel(Guid maintainableAssetID, List<AttributeDTO> attributeDTOs, List<string> values)
        {
            var networkModel = new NetworkExportReportModel() {
                MaintainableAssetID = maintainableAssetID,
                Attributes = new Dictionary<string, string>()
            };
            for (int i = 0; i < attributeDTOs.Count; i++)
                networkModel.Attributes.Add(attributeDTOs[i].Name, values[i]);
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
