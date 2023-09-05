using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace BridgeCareCore.Services.Treatment
{
    public static class ExcelTreatmentModels
    {
        // Jake says --
        // When exporting treatments, don't export any ids or criteria. On re-import, we will delete all existing treatments
        // and forget their ids. They will be recreated from scratch with new ids and freshly-created criteria..
        public static AnchoredExcelRegionModel TreatmentContent(TreatmentDTO dto)
        {
            var returnValue = new AnchoredExcelRegionModel
            {
                Region = CombinedRegion(dto),
            };
            return returnValue;
        }

        public static RowBasedExcelRegionModel CombinedRegion(TreatmentDTO dto)
        {
            var returnValue = RowBasedExcelRegionModels.Concat(
                TreatmentDetailsRegion.DetailsRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentCostsRegion.CostsRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentConsequencesRegion.ConsequencesRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentBudgetsRegion.BudgetsRegion(dto),
                RowBasedExcelRegionModels.BlankLine,
                TreatmentPerformanceRecordRegion.PerformanceFactorsRegion(dto)
                );
            return returnValue;
        }

        public static ExcelWorksheetModel TreatmentWorksheet(TreatmentDTO dto)
        {
            var tabName = dto.Name;
            var content = TreatmentContent(dto);
            var returnValue = new ExcelWorksheetModel
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
            return returnValue;
        }
    }
}
