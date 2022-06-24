using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.UnfundedPavementProjects
{
    internal class UnfundedPavementProjects : IUnfundedPavementProjects
    {
        private ISummaryReportHelper _summaryReportHelper;

        public UnfundedPavementProjects()
        {
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }
        }

        public void Fill(ExcelWorksheet unfundedRecommendationWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = AddHeadersCells(unfundedRecommendationWorksheet);

            // Enable Auto Filter
            using (var autoFilterCells = unfundedRecommendationWorksheet.Cells[1, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }
        }




        public CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            // Row 1
            int headerRowIndex = 1;
            var headersRow = GetHeadersRow();

            worksheet.Cells.Style.WrapText = false;

            // Add all Row 1 headers
            for (int column = 0; column < headersRow.Count; column++)
            {
                worksheet.Cells[headerRowIndex, column + 1].Value = headersRow[column];
            }

            var row = headerRowIndex;

            worksheet.Row(row).Height = 15;
            worksheet.Row(row + 1).Height = 15;

            // Autofit before the merges
            worksheet.Cells.AutoFitColumns(0);

            var currentCell = new CurrentCell { Row = row + 2, Column = worksheet.Dimension.Columns + 1 };
            return currentCell;
        }

        private List<string> GetHeadersRow()
        {
            return new List<string>
            {
                "District",
                "County",
                "SR",
                "Segment",
                "Pavement\r\nLength",
                "Pavement\r\nArea",
                "Lanes",
                "BPN",
                "MPO/RPO",
                "Risk\r\nScore",
                "State Contracted\r\nFunded",
                "Analysis\r\nYear",
                "Unfunded\r\nTreatment",
                "Cost",
                "Cash Flow\r\nYrs/Amount",
                "OPI",
                "Roughness",
            };
        }
    }
}
