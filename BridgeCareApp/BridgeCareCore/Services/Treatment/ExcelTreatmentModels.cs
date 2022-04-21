using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Models.Worksheets;

namespace BridgeCareCore.Services.Treatment
{
    public static class ExcelTreatmentModels
    {
        public static AnchoredExcelRegionModel TreatmentContent(TreatmentDTO dto)
        {
            var r = new AnchoredExcelRegionModel
            {
                Region = CombinedRegion(dto),
            };
            return r;
        }

        public static RowBasedExcelRegionModel CombinedRegion(TreatmentDTO dto)
        {
            var r = RowBasedExcelRegionModels.Concat(
                TreatmentDetailsRegion.DetailsRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentCostsRegion.CostsRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentConsequencesRegion.ConsequencesRegion(dto)
                );
            return r;
        }

        public static ExcelWorksheetModel TreatmentWorksheet(TreatmentDTO dto)
        {
            var tabName = dto.Name;
            var content = TreatmentContent(dto);
            var r = new ExcelWorksheetModel
            {
                Content = new List<IExcelWorksheetContentModel> {
                    content,
                    ExcelWorksheetContentModels.ColumnWidth(1, 32.27),
                    ExcelWorksheetContentModels.ColumnWidth(2, 30.4),
                    ExcelWorksheetContentModels.ColumnWidth(3, 32.53),
                    ExcelWorksheetContentModels.ColumnWidth(4, 80),
                    ExcelWorksheetContentModels.ColumnWidth(5, 40),
                    ExcelWorksheetContentModels.ColumnWidth(6, 40),
                    ExcelWorksheetContentModels.ColumnWidth(7, 40),
                    ExcelWorksheetContentModels.ColumnWidth(8, 40),
                    ExcelWorksheetContentModels.ColumnWidth(9, 40),
                },
                TabName = tabName,
            };
            return r;
        }
    }
}
