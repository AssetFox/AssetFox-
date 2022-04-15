using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

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
                Content = new List<IExcelWorksheetContentModel> { content },
                TabName = tabName,
            };
            return r;
        }
    }
}
