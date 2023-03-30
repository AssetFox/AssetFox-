using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSPBExport
{
    public class TreatmentTab
    {
        private ReportHelper _reportHelper;
        private List<string> consequencesColumns;

        public TreatmentTab()
        {
            _reportHelper = new ReportHelper();
        }

        public void Fill(ExcelWorksheet treatmentsWorksheet, SimulationOutput simulationOutput)
        {
            var currentCell = AddHeadersCells(treatmentsWorksheet);

            // TODO for data
            // Years => Assets based on condi.n i.e. TreatmentCause except Undefined, NoSelection

        }

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet)
        {
            int headerRow = 1;
            int column = 1;

            worksheet.Cells.Style.WrapText = false;
            worksheet.Cells[headerRow, column++].Value = "SimulationID";
            worksheet.Cells[headerRow, column++].Value = "NetworkID";
            worksheet.Cells[headerRow, column++].Value = "MaintainableAssetId";
            worksheet.Cells[headerRow, column++].Value = "District";
            worksheet.Cells[headerRow, column++].Value = "Cnty";
            worksheet.Cells[headerRow, column++].Value = "Route";
            worksheet.Cells[headerRow, column++].Value = "AssetName";
            worksheet.Cells[headerRow, column++].Value = "Direction";
            worksheet.Cells[headerRow, column++].Value = "FromSection";
            worksheet.Cells[headerRow, column++].Value = "ToSection";
            worksheet.Cells[headerRow, column++].Value = "AssetName";
            worksheet.Cells[headerRow, column++].Value = "Year";
            worksheet.Cells[headerRow, column++].Value = "Appliedtreatment";
            worksheet.Cells[headerRow, column++].Value = "Cost";
            worksheet.Cells[headerRow, column++].Value = "Benefit";
            worksheet.Cells[headerRow, column++].Value = "RiskScore";
            worksheet.Cells[headerRow, column++].Value = "RemainingLife";
            worksheet.Cells[headerRow, column++].Value = "PriorityLevel";
            worksheet.Cells[headerRow, column++].Value = "TreatmentFundingIgnoresSpendingLimit";
            worksheet.Cells[headerRow, column++].Value = "TreatmentStatus";
            worksheet.Cells[headerRow, column++].Value = "TreatmentCause";
            worksheet.Cells[headerRow, column++].Value = "Budget";
            worksheet.Cells[headerRow, column++].Value = "Category";

            consequencesColumns = GetconsequencesHeaders();
            foreach(var consequence in consequencesColumns)
            {
                worksheet.Cells[headerRow, column++].Value = consequence;
            }

            return new CurrentCell { Row = ++headerRow, Column = worksheet.Dimension.Columns + 1 };
        }

        private static List<string> GetconsequencesHeaders() => new()
        {
            "BMISCCK1","CBRKSLB_SUM","BLRUTDP2","CLJCPRU3","BRAVLWT3","BEDGDTR0","SURFACEID","CTRNCRK0",
            "BLRUTDP_SUM","LAST_STRUCTURAL_OVERLAY","BEDGDTR1","BTRNSCT_SUM","CBPATCCT","BFATICR0","CLNGCRK3",
            "CRJCPRU0","BRAVLWT2","CNSLABCT","CRJCPRU_SUM","BTRNSFT_SUM","BMISCCK3","CFLTJNT2","CTRNCRK3","CTRNJNT3",
            "BTRNSCT3_264","CLJCPRU_SUM","BTRNSFT3","BTRNSCT2","BPATCHSF","CTRNJNT1","BLTEDGE_SUM","CBPATCSF",
            "YEAR_LAST_OVERLAY","BTRNSCT1_264","ROUGHNESS","BTRNSFT0","FAULTING_CALCULATED","CLNGJNT1","CLNGJNT2",
            "CLNGCRK0","AGE","BTRNSCT2_264","BLTEDGE3","CLJCPRU2","BTRNSCT3","BRRUTDP1","CLNGCRK1","CBRKSLB3",
            "BLTEDGE2","CBRKSLB2","CRJCPRU2","CFLTJNT3","CFLTJNT_SUM","OPI_CALCULATED","BPATCHCT","BMISCCK2",
            "CPCCPACT","CTRNCRK1","CTRNJNT_SUM","CTRNJNT2","CJOINTCT","CTRNJNT0","YR_BUILT","BLRUTDP1","CRJCPRU3",
            "BRRUTDP3","BMISCCK_SUM","CLNGJNT_SUM","BTRNSCT1","CLJCPRU1","BRAVLWT0","BRAVLWT_SUM","BLTEDGE1","CTRNCRK2",
            "CLNGRK_SUM","BEDGDTR3","BMISCCK0","BRRUTDP2","CFLTJNT0","BFATICR1","CPCCPASF","BLRUTDP0","CLNGCRK2","CBRKSLB0",
            "BTRNSCT0","BEDGDTR2","BTRNSFT1","BFATICR2","BFATICR3","CLJCPRU0","CLNGJNT0","BLRUTDP3","BRRUTDP_SUM",
            "BEDGDTR_SUM","CBRKSLB1","BAFTICR_SUM","CLNGJNT3","BLTEDGE0","BRRUTDP0","CTRNCRK_SUM","BTRNSFT2","CRJCPRU1","OPI"
        };
    }
}
