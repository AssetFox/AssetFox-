using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSAuditReport
{
    public class PAMSDataTab
    {
        private PavementUnfundedTreatments _pavementUnfundedTreatments;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;
        public static HashSet<string> GetRequiredAttributes() => new()
        {
            $"{PAMSAuditReportConstants.OPI}",
            $"{PAMSAuditReportConstants.IRI}",
            $"{PAMSAuditReportConstants.RUT}",
            $"{PAMSAuditReportConstants.FAULT}"
        };

        public PAMSDataTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _pavementUnfundedTreatments = new PavementUnfundedTreatments(_unitOfWork);
        }

        public void Fill(ExcelWorksheet pavementWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = _pavementUnfundedTreatments.AddHeadersCells(pavementWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = pavementWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            pavementWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(pavementWorksheet, simulationOutput, currentCell);

            pavementWorksheet.Cells.AutoFitColumns();
            _pavementUnfundedTreatments.PerformPostAutofitAdjustments(pavementWorksheet);
        }

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // TODO bridges in data tab need to match with bridges in Decision tab           
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var CRS = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, "CRS");

                // Generate data model
                var bridgeDataModel = GeneratePavementDataModel(CRS, initialAssetSummary);

                // Fill in excel
                _pavementUnfundedTreatments.FillDataInWorksheet(worksheet, currentCell, bridgeDataModel);
            }
        }

        private static PavementDataModel GeneratePavementDataModel(double CRS, AssetSummaryDetail initialAssetSummary) => new()
        {
            CRS = CRS,
            AssetSummaryDetail = initialAssetSummary
        };

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);
    }
}
