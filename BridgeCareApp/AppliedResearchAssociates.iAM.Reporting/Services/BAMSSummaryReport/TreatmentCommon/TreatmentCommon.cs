using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.TreatmentCommon
{
    public class TreatmentCommon : ITreatmentCommon
    {
        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet) => throw new NotImplementedException();
        public void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year, TreatmentOptionDetail treatment) => throw new NotImplementedException();
        public List<AssetDetail> GetUntreatedSections(SimulationYearDetail simulationYearDetail) => throw new NotImplementedException();
    }
}
