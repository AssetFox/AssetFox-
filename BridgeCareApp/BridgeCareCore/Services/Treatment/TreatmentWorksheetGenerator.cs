using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentWorksheetGenerator
    {
        public static ExcelWorksheetModel ExportModelForTreatment(TreatmentDTO treatment)
        {
            var tabName = treatment.Name;
            var rows = TreatmentGeneralSectionGenerator.GeneralSectionRows(treatment);
            var r = new ExcelWorksheetModel
            {
                TabName = tabName,
                Content = rows,
            };
            return r;
        }
    }
}
