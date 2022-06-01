using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface IUnfundedTreatmentCommon
    {
        void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionDetail section, int Year, TreatmentOptionDetail treatment);
        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet);
        public List<SectionDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail);
    }
}
