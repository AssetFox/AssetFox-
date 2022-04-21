using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IUnfundedTreatmentCommon
    {
        void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionDetail section, int Year, TreatmentOptionDetail treatment);
        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet);
        public List<SectionDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail);
    }
}
