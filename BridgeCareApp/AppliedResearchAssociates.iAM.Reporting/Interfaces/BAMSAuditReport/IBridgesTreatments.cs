using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSAuditReport
{
    internal interface IBridgesTreatments
    {
        void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year);

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet);

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet);
    }
}
