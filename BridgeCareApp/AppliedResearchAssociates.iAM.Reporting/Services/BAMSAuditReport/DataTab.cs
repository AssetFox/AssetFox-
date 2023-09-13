using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class DataTab
    {
        private BridgesUnfundedTreatments _bridgesUnfundedTreatments;
        private ReportHelper _reportHelper;
        private readonly IUnitOfWork _unitOfWork;

        public static HashSet<string> GetRequiredAttributes() => new()
        {
            $"{BAMSAuditReportConstants.DeckSeeded}",
            $"{BAMSAuditReportConstants.SupSeeded}",
            $"{BAMSAuditReportConstants.SubSeeded}",
            $"{BAMSAuditReportConstants.CulvSeeded}",
            $"{BAMSAuditReportConstants.DeckDurationN}",
            $"{BAMSAuditReportConstants.SupDurationN}",
            $"{BAMSAuditReportConstants.SubDurationN}",
            $"{BAMSAuditReportConstants.CulvDurationN}"
        };

        public DataTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
            _bridgesUnfundedTreatments = new BridgesUnfundedTreatments(_unitOfWork);
        }        

        public void Fill(ExcelWorksheet bridgesWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = _bridgesUnfundedTreatments.AddHeadersCells(bridgesWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = bridgesWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }            

            bridgesWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(bridgesWorksheet, simulationOutput, currentCell);

            bridgesWorksheet.Cells.AutoFitColumns();
            _bridgesUnfundedTreatments.PerformPostAutofitAdjustments(bridgesWorksheet);
        }        

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {               
            // TODO bridges in data tab need to match with bridges in Decision tab           
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var brKey = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, "BRKEY_");

                // Generate data model
                var bridgeDataModel = GenerateBridgeDataModel(brKey, initialAssetSummary);

                // Fill in excel
                _bridgesUnfundedTreatments.FillDataInWorksheet(worksheet, currentCell, bridgeDataModel);
            }
        }

        private static BridgeDataModel GenerateBridgeDataModel(double brKey, AssetSummaryDetail initialAssetSummary) => new()
        {
            BRKey = brKey,
            AssetSummaryDetail = initialAssetSummary
        };

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);        
    }
}
