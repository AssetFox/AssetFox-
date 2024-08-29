using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSDistressProgressionReport
{
    public class OPICalculations
    {
        private readonly IUnitOfWork _unitOfWork;
        private ReportHelper _reportHelper;

        public OPICalculations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, bool shouldBundleFeasibleTreatments)
        {
            var currentCell = FillHeaders(worksheet);
            FillDynamicData(worksheet, reportOutputData, currentCell, shouldBundleFeasibleTreatments);
        }

        private void FillDynamicData(ExcelWorksheet worksheet, SimulationOutput reportOutputData, CurrentCell currentCell, bool shouldBundleFeasibleTreatments)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;

            foreach (var initialAssetSummary in reportOutputData.InitialAssetSummaries)
            {   
                var valuePerTextAttribute = initialAssetSummary.ValuePerTextAttribute;
                var crs = CheckGetTextValue(valuePerTextAttribute, "CRS");
                Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
                foreach (var yearData in reportOutputData.Years)
                {
                    column = 1;
                    var section = yearData.Assets.FirstOrDefault(_ => CheckGetTextValue(_.ValuePerTextAttribute, "CRS") == crs);
                    var sectionValuePerNumericAttribute = section.ValuePerNumericAttribute;
                    var sectionValuePerTextAttribute = section.ValuePerTextAttribute;

                    // Build keyCashFlowFundingDetails
                    if (section.TreatmentStatus != TreatmentStatus.Applied)
                    {
                        var fundingSection = yearData.Assets.
                                              FirstOrDefault(_ => CheckGetTextValue(_.ValuePerTextAttribute, "CRS") == crs &&
                                                            _.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                            _.AppliedTreatment.ToLower() != PAMSConstants.NoTreatment &&
                                                            _.AppliedTreatment == section.AppliedTreatment);
                        if (fundingSection != null)
                        {
                            if (!keyCashFlowFundingDetails.ContainsKey(crs))
                            {
                                keyCashFlowFundingDetails.Add(crs, fundingSection.TreatmentConsiderations ?? new());
                            }
                            else
                            {
                                keyCashFlowFundingDetails[crs].AddRange(fundingSection.TreatmentConsiderations);
                            }
                        }
                    }

                    worksheet.Cells[row, column++].Value = yearData.Year;
                    worksheet.Cells[row, column].Value = crs;
                    worksheet.Column(column++).Width = 18;
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "DISTRICT");
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "CNTY");
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "SR");

                    var lastUnderScoreIndex = crs.LastIndexOf('_');
                    var hyphenIndex = crs.IndexOf('-');
                    var startSeg = crs.Substring(lastUnderScoreIndex + 1, hyphenIndex - lastUnderScoreIndex - 1);
                    var endSeg = crs.Substring(hyphenIndex + 1);
                    worksheet.Cells[row, column++].Value = startSeg;
                    worksheet.Cells[row, column++].Value = endSeg;
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "DIRECTION");
                    worksheet.Cells[row, column++].Value = CheckGetValue(sectionValuePerNumericAttribute, "SEGMENT_LENGTH");
                    worksheet.Cells[row, column++].Value = CheckGetValue(sectionValuePerNumericAttribute, "WIDTH");
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "BUSIPLAN");
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "SURFACEID") + "-" + CheckGetTextValue(valuePerTextAttribute, "SURFACE_NAME");
                    worksheet.Column(column++).Width = 37;
                    worksheet.Cells[row, column++].Value = CheckGetTextValue(valuePerTextAttribute, "FAMILY");

                    // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                    var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                  keyCashFlowFundingDetails[crs] :
                                                  section.TreatmentConsiderations ?? new();
                    var treatmentConsideration = treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                 _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearData.Year));
                    worksheet.Cells[row, column].Value = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                    worksheet.Column(column).Width = 71;
                    worksheet.Column(column).Style.WrapText = true;
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);


                    // part 1 data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "OPI");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "OPI_CALCULATED");                     
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "ROUGHNESS");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);


                    // Bituminous data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLRUTDP_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRRUTDP_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BFATICR_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSCT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BTRNSFT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BMISCCK_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BPATCHCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BPATCHSF");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BRAVLWT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "BLTEDGE_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);


                    // Concrete data
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CNSLABCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CJOINTCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CFLTJNT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBRKSLB_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNCRK_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CTRNJNT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGCRK_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLNGJNT_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBPATCCT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CBPATCSF");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CPCCPACT");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CPCCPASF");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CLJCPRU_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column++]);
                    
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU1");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU2");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU3");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column++], ExcelHelperCellFormat.DecimalPrecision2);
                    worksheet.Cells[row, column].Value = CheckGetValue(sectionValuePerNumericAttribute, "CRJCPRU_Total");
                    ExcelHelper.SetCustomFormat(worksheet.Cells[row, column], ExcelHelperCellFormat.DecimalPrecision2);
                    // right border line
                    ExcelHelper.ApplyRightTickBorder(worksheet.Cells[row, column]);
                    
                    row++;
                }
            }
        }

        private static CurrentCell FillHeaders(ExcelWorksheet worksheet)
        {
            int headerRow = 1;
            const double minimumColumnWidth = 15;
            var headers = GetInitialHeaders();
            var currentCell = new CurrentCell { Row = headerRow, Column = headers.Count };
            for (int column = 0; column < headers.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = headers[column];
                worksheet.Column(column + 1).Width = minimumColumnWidth;
                ExcelHelper.ApplyBorder(worksheet.Cells[headerRow, column + 1, headerRow, column + 1]);
            }

            currentCell = FillDynamicHeaders(worksheet, currentCell);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow, 1, headerRow, currentCell.Column - 1]);

            return currentCell;
        }

        private static CurrentCell FillDynamicHeaders(ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var yearPartOneHeaders = GetPartOneHeaders();
            var yearBituminousHeaders = GetBituminousHeaders();
            var yearConcreteHeaders = GetConcreteHeaders();
            var column = currentCell.Column + 1;
            var dynamicStartColumn = column;

            column = BuildDataSubHeaders(worksheet, row, column, yearPartOneHeaders, ColorTranslator.FromHtml("#70AD47"));
            column = BuildDataSubHeaders(worksheet, row, column, yearBituminousHeaders, Color.Black);
            column = BuildDataSubHeaders(worksheet, row, column, yearConcreteHeaders, ColorTranslator.FromHtml("#FFF2CC"));
            
            ExcelHelper.ApplyBorder(worksheet.Cells[row, dynamicStartColumn, row, column - 1]);
            worksheet.Cells[row, dynamicStartColumn, row, column - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            currentCell.Column = column;

            const double minimumColumnWidth = 15;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (worksheet.Column(col).Width < minimumColumnWidth)
                {
                    worksheet.Column(col).Width = minimumColumnWidth;
                }
            }
            currentCell.Row = row + 1;
            return currentCell;
        }

        private static int BuildDataSubHeaders(ExcelWorksheet worksheet, int row, int column, List<string> subHeaders, Color color)
        {
            var startColumn = column;
            for (var index = 0; index < subHeaders.Count; index++)
            {
                worksheet.Cells[row, column].Value = subHeaders[index];
                ExcelHelper.ApplyStyle(worksheet.Cells[row, column]);
                column++;
            }
            column--;
            worksheet.Cells[row, startColumn, row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, startColumn, row, column].Style.Fill.BackgroundColor.SetColor(color);
            ExcelHelper.ApplyBorder(worksheet.Cells[row, startColumn, row, column]);
            if (color == Color.Black)
            {
                worksheet.Cells[row, startColumn, row, column].Style.Font.Color.SetColor(Color.White);
            }
            worksheet.Cells[row, startColumn, row, column].AutoFitColumns();

            return ++column;
        }

        // yellow shade       
        private static List<string> GetConcreteHeaders() => new()
        {            
            "SLAB COUNT",
            "JOINT COUNT",
            "CFLTJNT2",
            "CFLTJNT3",
            "Total CFLTJNT",
            "CBRKSLB1",
            "CBRKSLB2",
            "CBRKSLB3",
            "Total CBRKSLB",
            "CTRNCRK1",
            "CTRNCRK2",
            "CTRNCRK3",
            "Total CTRNCRK",
            "CTRNJNT1",
            "CTRNJNT2",
            "CTRNJNT3",
            "Total CTRNJNT",
            "CLNGCRK1",
            "CLNGCRK2",
            "CLNGCRK3",
            "Total CLNGCRK",
            "CLNGJNT1",
            "CLNGJNT2",
            "CLNGJNT3",
            "Total CLNGJNT",
            "CBPATCCT",
            "CBPATCSF",
            "CPCCPACT",
            "CPCCPASF",
            "CLJCPRU1",
            "CLJCPRU2",
            "CLJCPRU3",
            "Total CLJCPRU",
            "CRJCPRU1",
            "CRJCPRU2",
            "CRJCPRU3",
            "Total CRJCPRU",
        };

        // black background with text white
        private static List<string> GetBituminousHeaders() => new()
        {
            "BLRUTDP1",
            "BLRUTDP2",
            "BLRUTDP3",
            "Total BLRUTDP",
            "BRRUTDP1",
            "BRRUTDP2",
            "BRRUTDP3",
            "Total BRRUTDP",
            "BFATICR1",
            "BFATICR2",
            "BFATICR3",
            "Total BFATICR",
            "BTRNSCT1",
            "BTRNSFT1",
            "BTRNSCT2",
            "BTRNSFT2",
            "BTRNSCT3",
            "BTRNSFT3",
            "Total BTRNSCT",
            "Total BTRNSFT",
            "BMISCCK1",
            "BMISCCK2",
            "BMISCCK3",
            "Total BMISCCK", 
            "BEDGDTR1",
            "BEDGDTR2",
            "BEDGDTR3",
            "Total BEDGDTR",
            "BPATCHCT",
            "BPATCHSF",
            "BRAVLWT2",
            "BRAVLWT3",
            "Total BRAVLWT",
            "BLTEDGE1",
            "BLTEDGE2",
            "BLTEDGE3",
            "Total BLTEDGE",
        };

        // green shade
        private static List<string> GetPartOneHeaders() => new()
        {
            "AVERAGE OPI",
            "CALCULATED OPI",            
            "AVERAGE ROUGHNESS",
        };

        private static List<string> GetInitialHeaders() => new()
        {
            "CONDITION YEAR",
            "CRS",
            "DISTRICT",
            "COUNTY",
            "STATE ROUTE",
            "SEGMENT START",
            "SEGMENT END",
            "DIRECTION",
            "LENGTH (FT)",
            "WIDTH (FT)",
            "BPN",
            "Pavement Surface Type",
            "Family ID",
            "Treatment Selected"
        };

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) => _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);
    }
}
