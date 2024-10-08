using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.BridgeData;
using AppliedResearchAssociates.iAM.Reporting.Services.FlexibileAuditReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.GeneralSummaryReport.GeneralBudgetSummary
{
    public class GeneralWorkDoneTab
    {
        private ReportHelper _reportHelper;
        private const int headerRow1 = 1;
        private const int headerRow2 = 2;        
        private bool ShouldBundleFeasibleTreatments;
        private readonly IUnitOfWork _unitOfWork;
        private HighlightWorkDoneCells _highlightWorkDoneCells;
        private readonly List<int> _simulationYears = new();
        int workDoneStartColumn = 0;

        public GeneralWorkDoneTab(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _highlightWorkDoneCells = new HighlightWorkDoneCells();
            _reportHelper = new ReportHelper(_unitOfWork);
        }

        public void Fill(ExcelWorksheet decisionsWorksheet, SimulationOutput simulationOutput, Simulation simulation, HashSet<string> performanceCurvesAttributes)
        {            
            // Distinct performance curves' attributes
            var currentAttributes = performanceCurvesAttributes;
            // Benefit attribute
            currentAttributes.Add(_reportHelper.GetBenefitAttribute(simulation));

            ShouldBundleFeasibleTreatments = simulation.ShouldBundleFeasibleTreatments;

            simulationOutput.Years.ForEach(_ => _simulationYears.Add(_.Year));
            // Add headers to excel
            var currentCell = AddHeadersCells(decisionsWorksheet, currentAttributes, _simulationYears);

            // Fill data in excel
            FillDynamicDataInWorkSheet(simulationOutput, currentAttributes, decisionsWorksheet, currentCell);

            decisionsWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
            decisionsWorksheet.Cells.AutoFitColumns();
            PerformPostAutofitAdjustments(decisionsWorksheet);
        }

        private void FillDynamicDataInWorkSheet(SimulationOutput simulationOutput, HashSet<string> currentAttributes, ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            Dictionary<string, List<TreatmentConsiderationDetail>> keyCashFlowFundingDetails = new();
            var primaryKey = _unitOfWork.AdminSettingsRepo.GetKeyFields();
            var firstPrimaryKey = primaryKey[0];            
            var initialRow = currentCell.Row; // should be 4
            var isInitialYear = true;
            var row = initialRow; // Data starts here

            // Initial data columns
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                // get unique key
                var primaryKeyValue = CheckGetTextValue(initialAssetSummary.ValuePerTextAttribute, primaryKey[0]);
                if (string.IsNullOrEmpty(primaryKeyValue) && initialAssetSummary.ValuePerNumericAttribute != null)
                {
                    primaryKeyValue = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, primaryKey[0]).ToString();
                }

                // Current attributes
                var currentAttributesValues = new List<double>();
                for (int index = 0; index < currentAttributes.Count - 1; index++)
                {
                    var attributeValue = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, currentAttributes.ElementAt(index));
                    currentAttributesValues.Add(attributeValue);
                }
                // analysis benefit attribute
                currentAttributesValues.Add(CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, currentAttributes.Last()));
                currentCell = FillDataInWorksheet(worksheet, primaryKeyValue, currentAttributesValues, currentAttributes, currentCell);
            }

            var column = workDoneStartColumn;
            foreach (var yearlySectionData in simulationOutput.Years)
            {
                row = initialRow;                

                // Add work done cells
                var previousYearCause = TreatmentCause.Undefined;
                var previousYearTreatment = BAMSConstants.NoTreatment;                
                foreach (var section in yearlySectionData.Assets)
                {
                    // get unique key
                    var primaryKeyValue = CheckGetTextValue(section.ValuePerTextAttribute, primaryKey[0]);
                    if (string.IsNullOrEmpty(primaryKeyValue) && section.ValuePerNumericAttribute != null)
                    {
                        primaryKeyValue = CheckGetValue(section.ValuePerNumericAttribute, primaryKey[0]).ToString();
                    }

                    AssetDetail prevYearSection = null;
                    if (section.TreatmentCause == TreatmentCause.CommittedProject && !isInitialYear)
                    {
                        // generic
                        prevYearSection = simulationOutput.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                                         .Assets.FirstOrDefault(_ => CheckGetTextValue(section.ValuePerTextAttribute, primaryKey[0]) == primaryKeyValue);
                        prevYearSection ??= simulationOutput.Years.FirstOrDefault(f => f.Year == yearlySectionData.Year - 1)
                                         .Assets.FirstOrDefault(_ => CheckGetValue(section.ValuePerNumericAttribute, primaryKey[0]).ToString() == primaryKeyValue);
                        previousYearCause = prevYearSection.TreatmentCause;
                        previousYearTreatment = prevYearSection.AppliedTreatment;
                    }                   

                    // Work done in a year                                      
                    // Build keyCashFlowFundingDetails                    
                    if (section.TreatmentStatus != TreatmentStatus.Applied)
                    {
                        var fundingSection = section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                             section.AppliedTreatment.ToLower() != FlexibleAuditReportConstants.NoTreatment ? section : null;

                        if (fundingSection != null)
                        {
                            if (!keyCashFlowFundingDetails.ContainsKey(primaryKeyValue))
                            {
                                keyCashFlowFundingDetails.Add(primaryKeyValue, fundingSection.TreatmentConsiderations ?? new());
                            }
                            else
                            {
                                keyCashFlowFundingDetails[primaryKeyValue].AddRange(fundingSection.TreatmentConsiderations);
                            }
                        }
                    }

                    // If CF then use obj from keyCashFlowFundingDetails otherwise from section
                    var treatmentConsiderations = ((section.TreatmentCause == TreatmentCause.SelectedTreatment &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Progressed) ||
                                                  (section.TreatmentCause == TreatmentCause.CashFlowProject &&
                                                  section.TreatmentStatus == TreatmentStatus.Applied)) ?
                                                  keyCashFlowFundingDetails[primaryKeyValue] :
                                                  section.TreatmentConsiderations ?? new();                    

                    var treatmentConsideration = ShouldBundleFeasibleTreatments ?
                                                 treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                    _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                                    section.AppliedTreatment.Contains(_.TreatmentName)) :
                                                 treatmentConsiderations.FirstOrDefault(_ => _.FundingCalculationOutput != null &&
                                                    _.FundingCalculationOutput.AllocationMatrix.Any(_ => _.Year == yearlySectionData.Year) &&
                                                    _.TreatmentName == section.AppliedTreatment);

                    var appliedTreatment = treatmentConsideration?.TreatmentName ?? section.AppliedTreatment;
                    var allocationMatrix = treatmentConsideration?.FundingCalculationOutput?.AllocationMatrix ?? new();
                    var cost = Math.Round(allocationMatrix?.Where(_ => _.Year == yearlySectionData.Year).Sum(_ => _.AllocatedAmount) ?? 0, 0); // Rounded cost to whole number based on comments from Jeff Davis
                    var workCell = worksheet.Cells[row, column];
                    workCell.Value = appliedTreatment.ToLower() == BAMSConstants.NoTreatment ? "--" : appliedTreatment.ToLower();                    
                    worksheet.Cells[row, column + 1].Value = cost;
                    ExcelHelper.SetCurrencyFormat(worksheet.Cells[row, column + 1], ExcelFormatStrings.CurrencyWithoutCents);

                    //if (row % 2 == 0)
                    //{
                    //    var cellColor = worksheet.Cells[row, column, row, column + 1].Style.Fill.BackgroundColor;
                    //    if (section.TreatmentCause != TreatmentCause.CashFlowProject &&
                    //        !(section.TreatmentCause == TreatmentCause.CommittedProject && previousYearCause == TreatmentCause.CommittedProject && previousYearTreatment.ToLower() != BAMSConstants.NoTreatment))
                    //    {
                    //        ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.LightGray);
                    //    }                        
                    //}

                    if (section.TreatmentCause == TreatmentCause.CashFlowProject)
                    {
                        ExcelHelper.ApplyColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, column, row, column + 1], Color.FromArgb(255, 0, 0));

                        // Color the previous year project also
                        ExcelHelper.ApplyColor(worksheet.Cells[row, column - 2, row, column - 1], Color.FromArgb(0, 255, 0));
                        ExcelHelper.SetTextColor(worksheet.Cells[row, column - 2, row, column - 1], Color.FromArgb(255, 0, 0));
                    }

                    ExcelHelper.ApplyBorder(worksheet.Cells[row, column, row, column + 1]);                    
                    row++;
                }                
                
                column += 2;
                isInitialYear = false;
            }
        }

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);

        private string CheckGetTextValue(Dictionary<string, string> valuePerTextAttribute, string attribute) => _reportHelper.CheckAndGetValue<string>(valuePerTextAttribute, attribute);

        private CurrentCell AddHeadersCells(ExcelWorksheet worksheet, HashSet<string> currentAttributes, List<int> simulationYears)
        {
            int column = 1;
            var primaryKeyField = _unitOfWork.AdminSettingsRepo.GetKeyFields();            
            worksheet.Cells[headerRow2, column++].Value = primaryKeyField[0].ToString();

            // Current
            column = AddCurrentAttributesHeaders(worksheet, currentAttributes, column);
            worksheet.Cells.Style.WrapText = false;

            // dynamic year headers
            var currentCell = new CurrentCell { Row = headerRow2, Column = column };
            AddDynamicHeadersCells(worksheet, currentCell, simulationYears);
            column = currentCell.Column;
            var currentAttributesCount = currentAttributes.Count;
            ExcelHelper.ApplyColor(worksheet.Cells[headerRow2, 2, headerRow2, column - 1], Color.FromArgb(255, 242, 204));            
            ExcelHelper.ApplyBorder(worksheet.Cells[headerRow1, 1, headerRow2 + 1, worksheet.Dimension.Columns]);
            ExcelHelper.ApplyStyleNoWrap(worksheet.Cells[headerRow2, 3, headerRow2, 3 + currentAttributesCount - 1]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 1, headerRow2, 2]);
            ExcelHelper.ApplyStyle(worksheet.Cells[headerRow2, 2 + currentAttributesCount, headerRow2 + 1, worksheet.Dimension.Columns]);


            using (ExcelRange autoFilterCells = worksheet.Cells[headerRow2 + 1, 1, headerRow2 + 1, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }
            
            return new CurrentCell { Row = headerRow2 + 2, Column = worksheet.Dimension.Columns + 1 };
        }
                
        private static int AddCurrentAttributesHeaders(ExcelWorksheet worksheet, HashSet<string> currentAttributes, int column)
        {
            var currentAttributesColumn = column;
            worksheet.Cells[headerRow1, currentAttributesColumn].Value = "Attributes";
            // Dynamic headers in row 2
            foreach (var currentAttribute in currentAttributes)
            {
                worksheet.Cells[headerRow2, column++].Value = currentAttribute;
            }
            // Merge cells for "Current"
            ExcelHelper.MergeCells(worksheet, headerRow1, currentAttributesColumn, headerRow1, column - 1);

            return column;
        }

        public static void PerformPostAutofitAdjustments(ExcelWorksheet worksheet)
        {
            worksheet.Column(2).SetTrueWidth(12);
        }

        private void setColor(int parallelBridge, string treatment, string previousYearTreatment, TreatmentCause previousYearCause,
           TreatmentCause treatmentCause, int year, int index, ExcelWorksheet worksheet, int row, int column)
        {
            _highlightWorkDoneCells.CheckConditions(parallelBridge, treatment, previousYearTreatment, previousYearCause, treatmentCause, year, index, worksheet, row, column);
        }

        private void AddDynamicHeadersCells(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            const string HeaderConstText = "Work Done\r\n";
            var column = currentCell.Column;
            var row = currentCell.Row;            
            workDoneStartColumn = column;
            foreach (var year in simulationYears)
            {
                ExcelHelper.ApplyColor(worksheet.Cells[row - 1, column, row, column + 1], ColorTranslator.FromHtml("#D9E1F2"));
                ExcelHelper.MergeCells(worksheet, row - 1, column, row, column + 1);
                worksheet.Cells[row - 1, column].Value = HeaderConstText + year;
                worksheet.Cells[row + 1, column].Value = BAMSConstants.Work;
                worksheet.Cells[row + 1, column + 1].Value = "Cost";
                ExcelHelper.ApplyStyle(worksheet.Cells[row, column, row, column + 1]);
                column += 2;                
            }            

            ExcelHelper.ApplyBorder(worksheet.Cells[row - 1, workDoneStartColumn, row, worksheet.Dimension.Columns]);
            currentCell.Row++;
            currentCell.Column = column - 1;
        }

        private CurrentCell FillDataInWorksheet(ExcelWorksheet decisionsWorksheet, string primaryKeyValue, List<double> currentAttributesValues, HashSet<string> currentAttributes, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            int column = 1;

            column = FillInitialDataInWorksheet(decisionsWorksheet, primaryKeyValue, currentAttributesValues, currentAttributes, row, column);
            ExcelHelper.ApplyBorder(decisionsWorksheet.Cells[row, 1, row, column - 1]);

            return new CurrentCell { Row = row + 1, Column = column - 1 };
        }

        private int FillInitialDataInWorksheet(ExcelWorksheet decisionsWorksheet, string primaryKeyValue, List<double> currentAttributesValues, HashSet<string> currentAttributes, int row, int column)
        {
            ExcelHelper.HorizontalCenterAlign(decisionsWorksheet.Cells[row, column]);
            decisionsWorksheet.Cells[row, column++].Value = primaryKeyValue;
            
            for (int index = 0; index < currentAttributesValues.Count; index++)
            {
                SetDecimalFormat(decisionsWorksheet.Cells[row, column]);
                var attribute = currentAttributes.ElementAt(index);
                var currentAttributesValue = currentAttributesValues[index];         
                decisionsWorksheet.Cells[row, column++].Value = currentAttributesValue;
            }

            return column;
        }
        
        private static void SetDecimalFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.DecimalPrecision3);

        private static void SetAccountingFormat(ExcelRange cell) => ExcelHelper.SetCustomFormat(cell, ExcelHelperCellFormat.Accounting);
    }
}
