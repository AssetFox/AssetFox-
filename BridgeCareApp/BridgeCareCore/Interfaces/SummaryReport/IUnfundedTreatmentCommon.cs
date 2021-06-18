﻿using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface IUnfundedTreatmentCommon
    {
        void FillDataInWorkSheet(ExcelWorksheet worksheet, CurrentCell currentCell, SectionDetail section, int Year, TreatmentOptionDetail treatment);
        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet);
    }
}
