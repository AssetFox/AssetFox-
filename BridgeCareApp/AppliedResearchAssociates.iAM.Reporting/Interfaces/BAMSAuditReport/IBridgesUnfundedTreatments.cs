using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSAuditReport
{
    public interface IBridgesUnfundedTreatments
    {
        void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, AssetDetail section, int Year, TreatmentOptionDetail treatment);

        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet);

        public List<AssetDetail> GetSectionsWithUnfundedTreatments(SimulationYearDetail simulationYearDetail);

        public List<AssetDetail> GetSectionsWithFundedTreatments(SimulationYearDetail simulationYearDetail);

        public void PerformPostAutofitAdjustments(ExcelWorksheet worksheet);
    }
}
