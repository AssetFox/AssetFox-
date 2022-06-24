using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using OfficeOpenXml;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentWorksheetGenerator
    {
        public static void Fill(ExcelWorkbook workbook, List<TreatmentDTO> treatments)
        {
            foreach (var treatment in treatments)
            {
                var worksheetModel = ExcelTreatmentModels.TreatmentWorksheet(treatment);
                ExcelWorksheetAdder.AddWorksheet(workbook, worksheetModel);
            }
        }
    }
}
