using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport;
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

        public void Fill(ExcelWorksheet treatmentsWorksheet, SimulationOutput simulationOutput, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets)
        {
            var currentCell = AddHeadersCells(treatmentsWorksheet);

            FillDynamicDataInWorkSheet(simulationOutput, treatmentsWorksheet, currentCell, simulationId, networkId, treatments, networkMaintainableAssets);            
            treatmentsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            treatmentsWorksheet.Cells.AutoFitColumns();
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, ExcelWorksheet treatmentsWorksheet, CurrentCell currentCell, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets)
        {   
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var assetId = initialAssetSummary.AssetId;
                var years = simulationOutput.Years.OrderBy(yr => yr.Year);

                foreach (var year in years)
                {
                    var section = year.Assets.FirstOrDefault(_ => _.AssetId == assetId);
                    if (section.TreatmentCause == TreatmentCause.NoSelection || section.TreatmentCause == TreatmentCause.Undefined)
                    {
                        continue;
                    }

                    // Generate data model                    
                    var treatmentDataModel = GenerateTreatmentDataModel(assetId, year, section, simulationId, networkId, treatments, networkMaintainableAssets);

                    // Fill in excel
                    currentCell = FillDataInWorksheet(treatmentsWorksheet, treatmentDataModel, currentCell);
                }                
            }
            ExcelHelper.ApplyBorder(treatmentsWorksheet.Cells[1, 1, currentCell.Row - 1, currentCell.Column]);
        }

        private CurrentCell FillDataInWorksheet(ExcelWorksheet treatmentsWorksheet, TreatmentDataModel treatmentDataModel, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;

            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.SimulationId;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.NetworkId;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.MaintainableAssetId;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.District;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Cnty;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Route;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.AssetName;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Direction;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.FromSection;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.ToSection;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Year;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Appliedtreatment;
            SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Cost;
            SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Benefit;
            SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.RiskScore;
            SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.RemainingLife;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.PriorityLevel;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.TreatmentFundingIgnoresSpendingLimit;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.TreatmentStatus;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.TreatmentCause;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Budget;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Category;

            foreach(var consequence in treatmentDataModel.Consequences)
            {
                SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
                treatmentsWorksheet.Cells[row, column++].Value = consequence;
            }

            return new CurrentCell { Row = ++row, Column = column - 1 }; ;
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private TreatmentDataModel GenerateTreatmentDataModel(Guid assetId, SimulationYearDetail year, AssetDetail section, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets)
        {
            var appliedTreatment = section.AppliedTreatment;
            TreatmentDataModel treatmentDataModel = new TreatmentDataModel
            {
                SimulationId = simulationId,
                NetworkId = networkId,
                MaintainableAssetId = assetId,                
                Year = year.Year,
                Appliedtreatment = appliedTreatment,
            };

            var locationIdentifier = networkMaintainableAssets.FirstOrDefault(_ => _.Id == assetId)?.Location?.LocationIdentifier;
            treatmentDataModel.AssetName = locationIdentifier;
            var fromSection = string.Empty;
            var toSection = string.Empty;
            if (!string.IsNullOrEmpty(locationIdentifier))
            {
                var parts = locationIdentifier.Split(new char[] { '_' }).Last();
                var fromTo = parts.Split('-');
                fromSection = fromTo?.First();
                toSection = fromTo?.Last();
            }
            treatmentDataModel.FromSection = fromSection;
            treatmentDataModel.ToSection = toSection;

            treatmentDataModel.District = CheckGetTextValue(section.ValuePerTextAttribute, "DISTRICT");
            treatmentDataModel.Cnty = CheckGetTextValue(section.ValuePerTextAttribute, "CNTY");
            treatmentDataModel.Route = CheckGetTextValue(section.ValuePerTextAttribute, "SR");
            treatmentDataModel.Direction = CheckGetTextValue(section.ValuePerTextAttribute, "DIRECTION");
            treatmentDataModel.RiskScore = CheckGetNumericValue(section.ValuePerNumericAttribute, "RISKSCORE");            

            var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == appliedTreatment);
            treatmentDataModel.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
            treatmentDataModel.Benefit = treatmentOption != null ? treatmentOption.Benefit : 0;
            // TODO remove infinity condition once fix is available for such edge cases
            treatmentDataModel.RemainingLife = treatmentOption != null && treatmentOption.RemainingLife?.ToString() != "-∞" ? treatmentOption.RemainingLife : 0;
            var treatmentConsideration = section.TreatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == appliedTreatment);
            treatmentDataModel.PriorityLevel = treatmentConsideration?.BudgetPriorityLevel;

            treatmentDataModel.TreatmentFundingIgnoresSpendingLimit = section.TreatmentFundingIgnoresSpendingLimit ? 1 : 0;
            treatmentDataModel.TreatmentCause = (int)section.TreatmentCause;
            treatmentDataModel.TreatmentStatus = (int)section.TreatmentStatus;            
            var budgetName = treatmentConsideration != null ? treatmentConsideration.BudgetUsages.OrderByDescending(_ => _.CoveredCost).FirstOrDefault()?.BudgetName : string.Empty;
            treatmentDataModel.Budget = budgetName;
            treatmentDataModel.Category = treatments.FirstOrDefault(_ => _.Name == appliedTreatment)?.Category.ToString();

            // Consequences
            var consequences = new List<double>();
            foreach (var consequenceColumn in consequencesColumns)
            {
                consequences.Add(CheckGetNumericValue(section.ValuePerNumericAttribute, consequenceColumn));
            }
            treatmentDataModel.Consequences = consequences;

            return treatmentDataModel;
        }

        private double CheckGetNumericValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) =>
          _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);

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
            "BMISCCK1","BLRUTDP2","CLJCPRU3","BRAVLWT3","SURFACEID","LAST_STRUCTURAL_OVERLAY","BEDGDTR1","CBPATCCT","CLNGCRK3",
            "BRAVLWT2","CNSLABCT","BMISCCK3","CFLTJNT2","CTRNCRK3","CTRNJNT3","BTRNSFT3","BTRNSCT2","BPATCHSF","CTRNJNT1",
            "CBPATCSF","YEAR_LAST_OVERLAY","ROUGHNESS","CLNGJNT1","CLNGJNT2","AGE","BLTEDGE3","CLJCPRU2","BTRNSCT3","BRRUTDP1",
            "CLNGCRK1","CBRKSLB3","BLTEDGE2","CBRKSLB2","CRJCPRU2","CFLTJNT3","BPATCHCT","BMISCCK2","CPCCPACT","CTRNCRK1",
            "CTRNJNT2","CJOINTCT","YR_BUILT","BLRUTDP1","CRJCPRU3","BRRUTDP3","BTRNSCT1","CLJCPRU1","BLTEDGE1","CTRNCRK2",
            "BEDGDTR3","BRRUTDP2","BFATICR1","CPCCPASF","CLNGCRK2","BEDGDTR2","BTRNSFT1","BFATICR2","BFATICR3","BLRUTDP3",
            "CBRKSLB1","CLNGJNT3","BTRNSFT2","CRJCPRU1","OPI"
        };
    }
}
