using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSPBExport;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSPBExport
{
    public class TreatmentTab
    {
        private ReportHelper _reportHelper;
        private List<string> treatmentAttributes;
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet treatmentsWorksheet, SimulationOutput simulationOutput, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets, bool shouldBundleFeasibleTreatments)
        {
            var currentCell = AddHeadersCells(treatmentsWorksheet);

            FillDynamicDataInWorkSheet(simulationOutput, treatmentsWorksheet, currentCell, simulationId, networkId, treatments, networkMaintainableAssets, shouldBundleFeasibleTreatments);            
            treatmentsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            treatmentsWorksheet.Cells.AutoFitColumns();
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, ExcelWorksheet treatmentsWorksheet, CurrentCell currentCell, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets, bool shouldBundleFeasibleTreatments)
        {
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var assetId = initialAssetSummary.AssetId;
                var years = simulationOutput.Years.OrderBy(yr => yr.Year);
                Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
                foreach (var year in years)
                {
                    var section = year.Assets.FirstOrDefault(_ => _.AssetId == assetId);
                    if (section.TreatmentCause == TreatmentCause.NoSelection || section.TreatmentCause == TreatmentCause.Undefined)
                    {
                        continue;
                    }

                    // Generate data model                    
                    var treatmentDataModel = GenerateTreatmentDataModel(assetId, year, section, simulationId, networkId, treatments, networkMaintainableAssets, keyCashFlowFundingDetails, shouldBundleFeasibleTreatments);

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
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.MinYear;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.MaxYear;
            treatmentsWorksheet.Cells[row, column++].Value = treatmentDataModel.Interstate;
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

            foreach(var treatmentAttributeValue in treatmentDataModel.TreatmentAttributeValues)
            {
                SetDecimalFormat(treatmentsWorksheet.Cells[row, column]);
                treatmentsWorksheet.Cells[row, column++].Value = treatmentAttributeValue;
            }

            return new CurrentCell { Row = ++row, Column = column - 1 }; ;
        }

        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private TreatmentDataModel GenerateTreatmentDataModel(Guid assetId, SimulationYearDetail year, AssetDetail section, Guid simulationId, Guid networkId, IReadOnlyCollection<SelectableTreatment> treatments, List<MaintainableAsset> networkMaintainableAssets, Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails, bool shouldBundleFeasibleTreatments)
        {
            var appliedTreatment = section.AppliedTreatment;
            TreatmentDataModel treatmentDataModel = new TreatmentDataModel
            {
                SimulationId = simulationId,
                NetworkId = networkId,
                MaintainableAssetId = assetId,
                Year = year.Year,
                MinYear = year.Year,
                MaxYear = year.Year,
                Appliedtreatment = appliedTreatment,
            };

            var locationIdentifier = networkMaintainableAssets.FirstOrDefault(_ => _.Id == assetId)?.Location?.LocationIdentifier;
            treatmentDataModel.AssetName = locationIdentifier;
            var fromSection = string.Empty;
            var toSection = string.Empty;
            var direction = string.Empty;
            if (!string.IsNullOrEmpty(locationIdentifier))
            {
                var parts = locationIdentifier.Split(new char[] { '_' });
                direction = parts[2];
                var fromTo = parts.Last()?.Split('-');
                fromSection = fromTo?.First();
                toSection = fromTo?.Last();
            }
            treatmentDataModel.FromSection = fromSection;
            treatmentDataModel.ToSection = toSection;

            var valuePerTextAttribute = section.ValuePerTextAttribute;
            treatmentDataModel.District = CheckGetTextValue(valuePerTextAttribute, "DISTRICT");
            treatmentDataModel.Cnty = CheckGetTextValue(valuePerTextAttribute, "CNTY");
            treatmentDataModel.Route = CheckGetTextValue(valuePerTextAttribute, "SR");
            treatmentDataModel.Direction = direction; //CheckGetTextValue(valuePerTextAttribute, "DIRECTION");
            treatmentDataModel.RiskScore = CheckGetNumericValue(section.ValuePerNumericAttribute, "RISKSCORE");
            treatmentDataModel.Interstate = CheckGetTextValue(valuePerTextAttribute, "INTERSTATE");

            var treatmentOption = section.TreatmentOptions.FirstOrDefault(_ => _.TreatmentName == appliedTreatment);
            treatmentDataModel.Cost = treatmentOption != null ? treatmentOption.Cost : 0;
            treatmentDataModel.Benefit = treatmentOption != null ? treatmentOption.Benefit : 0;
            // TODO remove infinity condition once fix is available for such edge cases
            treatmentDataModel.RemainingLife = treatmentOption != null && treatmentOption.RemainingLife?.ToString() != "-∞" ? treatmentOption.RemainingLife : 0;

            // Build keyCashFlowFundingDetails
            var crs = CheckGetTextValue(section.ValuePerTextAttribute, "CRS");
            if (section.TreatmentStatus != TreatmentStatus.Applied)
            {
                var fundingSection = year.Assets.FirstOrDefault(_ => CheckGetTextValue(section.ValuePerTextAttribute, "CRS") == crs && _.TreatmentCause == TreatmentCause.SelectedTreatment && _.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment && _.AppliedTreatment == section.AppliedTreatment);
                if (fundingSection != null && !keyCashFlowFundingDetails.ContainsKey(crs))
                {
                    keyCashFlowFundingDetails.Add(crs, fundingSection?.TreatmentConsiderations ?? new());
                }
            }

            // If TreatmentStatus Applied and TreatmentCause is not CashFlowProject it means no CF then consider section obj and if Progressed that means it is CF then use obj from dict
            var treatmentConsiderations = section.TreatmentStatus == TreatmentStatus.Applied && section.TreatmentCause != TreatmentCause.CashFlowProject ?
                                          section.TreatmentConsiderations : keyCashFlowFundingDetails[crs];
            var treatmentConsideration = shouldBundleFeasibleTreatments ?
                                         treatmentConsiderations.FirstOrDefault() :
                                         treatmentConsiderations.FirstOrDefault(_ => _.TreatmentName == section.AppliedTreatment);         

            treatmentDataModel.PriorityLevel = treatmentConsideration?.BudgetPriorityLevel;
            treatmentDataModel.TreatmentFundingIgnoresSpendingLimit = section.TreatmentFundingIgnoresSpendingLimit ? 1 : 0;
            treatmentDataModel.TreatmentCause = section.TreatmentCause.ToString();
            treatmentDataModel.TreatmentStatus = section.TreatmentStatus.ToString();
          
            var budgetsUsed = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix?.Where(_ => _.AllocatedAmount > 0).Select(_ =>
                              _.BudgetName).Distinct().ToList() ?? new();
            treatmentDataModel.Budget = string.Join(", ", budgetsUsed);
            treatmentDataModel.Category = shouldBundleFeasibleTreatments && appliedTreatment.Contains("Bundle") ? PAMSConstants.Bundled : treatments.FirstOrDefault(_ => _.Name == appliedTreatment)?.Category.ToString();

            // Consequences
            var treatmentAttributeValues = new List<double>();
            foreach (var treatmentAttribute in treatmentAttributes)
            {
                treatmentAttributeValues.Add(CheckGetNumericValue(section.ValuePerNumericAttribute, treatmentAttribute));
            }
            treatmentDataModel.TreatmentAttributeValues = treatmentAttributeValues;

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
            worksheet.Cells[headerRow, column++].Value = "AssetId";
            worksheet.Cells[headerRow, column++].Value = "District";
            worksheet.Cells[headerRow, column++].Value = "Cnty";
            worksheet.Cells[headerRow, column++].Value = "Route";
            worksheet.Cells[headerRow, column++].Value = "Asset";
            worksheet.Cells[headerRow, column++].Value = "Direction";
            worksheet.Cells[headerRow, column++].Value = "FromSection";
            worksheet.Cells[headerRow, column++].Value = "ToSection";
            worksheet.Cells[headerRow, column++].Value = "Year";
            worksheet.Cells[headerRow, column++].Value = "MinYear";
            worksheet.Cells[headerRow, column++].Value = "MaxYear";
            worksheet.Cells[headerRow, column++].Value = "Interstate";
            worksheet.Cells[headerRow, column++].Value = "Treatment";
            worksheet.Cells[headerRow, column++].Value = "Cost";
            worksheet.Cells[headerRow, column++].Value = "Benefit";
            worksheet.Cells[headerRow, column++].Value = "Risk";
            worksheet.Cells[headerRow, column++].Value = "RemainingLife";
            worksheet.Cells[headerRow, column++].Value = "PriorityOrder";
            worksheet.Cells[headerRow, column++].Value = "TreatmentFundingIgnoresSpendingLimit";
            worksheet.Cells[headerRow, column++].Value = "TreatmentStatus";
            worksheet.Cells[headerRow, column++].Value = "TreatmentCause";
            worksheet.Cells[headerRow, column++].Value = "Budget";
            worksheet.Cells[headerRow, column++].Value = "Category";

            treatmentAttributes = GetTreatmentAttributes();
            foreach(var treatmentAttribute in treatmentAttributes)
            {
                worksheet.Cells[headerRow, column++].Value = treatmentAttribute;
            }

            return new CurrentCell { Row = ++headerRow, Column = worksheet.Dimension.Columns + 1 };
        }

        private static List<string> GetTreatmentAttributes() => new()
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
