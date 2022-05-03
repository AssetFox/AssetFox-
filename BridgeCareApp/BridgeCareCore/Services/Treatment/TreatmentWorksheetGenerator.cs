using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Helpers.Excel.Visitors;
using OfficeOpenXml;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentWorksheetGenerator
    {
        public static void Fill(ExcelWorkbook workbook, TreatmentLibraryDTO dto)
        {
            foreach (var treatment in dto.Treatments)
            {
                var worksheetModel = ExcelTreatmentModels.TreatmentWorksheet(treatment);
                ExcelWorksheetAdder.AddWorksheet(workbook, worksheetModel);
            }
        }
    }
}
