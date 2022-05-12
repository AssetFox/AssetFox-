﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Parameters
{
    public class SummaryReportParameters
    {
        private readonly ISummaryReportHelper _summaryReportHelper;
        public SummaryReportParameters(ISummaryReportHelper summaryReportHelper)
        {
            _summaryReportHelper = summaryReportHelper ?? throw new ArgumentNullException(nameof(summaryReportHelper));
        }
        internal void Fill(ExcelWorksheet worksheet, int simulationYearsCount, ParametersModel parametersModel, Simulation simulation)
        {
            var currentCell = new CurrentCell{Row = 1, Column = 1 };
            // Simulation Name format
            ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1);
            ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Row + 9);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1], Color.White);

            worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1].Value = "Simulation Name";
            worksheet.Cells[currentCell.Row, currentCell.Column + 2, currentCell.Row, currentCell.Row + 9].Value = simulation.Name;
            ExcelHelper.ApplyColor(worksheet.Cells[1, 3, 1, 10], Color.FromArgb(142, 169, 219));
            ExcelHelper.ApplyBorder(worksheet.Cells[1, 1, 1, 10]);
            // End of Simulation Name format

            // Simulation Comment
            ExcelHelper.MergeCells(worksheet, 2, 1, 2, 2);
            ExcelHelper.ApplyColor(worksheet.Cells[2, 1, 2, 2], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[2, 1, 2, 2], Color.White);
            worksheet.Cells["A2:B2"].Value = "Simulation Comment";

            ExcelHelper.MergeCells(worksheet, 2, 3, 2, 10);
            worksheet.Cells["C2:J2"].Value = simulation.AnalysisMethod.Description;
            ExcelHelper.ApplyBorder(worksheet.Cells[2, 1, 2, 10]);

            currentCell = FillData(worksheet, parametersModel, simulation.LastRun, currentCell, simulation.LastModifiedDate, simulation.AnalysisMethod.Filter.Expression);

            currentCell = FillSimulationDetails(worksheet, simulationYearsCount, simulation, currentCell);
            currentCell = FillAnalysisDetails(worksheet, simulation, currentCell);
            currentCell = FillJurisdictionCriteria(worksheet, simulation, currentCell);
            currentCell = FillPriorities(worksheet, simulation, currentCell);
            FillBudgetSplitCriteria(worksheet, currentCell, simulation);
            FillInvestmentAndBudgetCriteria(worksheet, simulation);
            worksheet.Cells.AutoFitColumns(50);
        }

        #region

        private CurrentCell FillData(ExcelWorksheet worksheet, ParametersModel parametersModel, DateTime lastRun, CurrentCell currentCell, DateTime lastModifiedDate,
            string jurisdictionExpression)
        {
            var bpnValueCellTracker = new Dictionary<string, (int row, int col)>();
            var statusValueCellTracker = new Dictionary<string, (int row, int col)>();

            worksheet.Cells[currentCell.Row + 2, currentCell.Column].Value = "BridgeCare Rules Creator:";
            worksheet.Cells[currentCell.Row + 2, currentCell.Column + 1].Value = "Central Office";
            worksheet.Cells[currentCell.Row + 3, currentCell.Column].Value = "BridgeCare Rules Date:";
            worksheet.Cells[currentCell.Row + 3, currentCell.Column + 1].Value = lastModifiedDate.ToShortDateString();
            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row + 2, currentCell.Column, currentCell.Row + 3, currentCell.Column + 1]);

            worksheet.Cells[currentCell.Row + 2, currentCell.Column + 3].Value = "Simulation Last Run:";
            worksheet.Cells[currentCell.Row + 2, currentCell.Column + 4].Value = lastRun.ToShortDateString();
            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row + 2, currentCell.Column + 3, currentCell.Row + 2, currentCell.Column + 4]);

            currentCell.Row += 5; // moving on to the "NHS" block

            ExcelHelper.MergeCells(worksheet, currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1);
            ExcelHelper.ApplyColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1], Color.White);
            worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row, currentCell.Column + 1].Value = "NHS";
            worksheet.Cells[currentCell.Row + 1, currentCell.Column].Value = "NHS";
            worksheet.Cells[currentCell.Row + 2, currentCell.Column].Value = "Non-NHS";

            worksheet.Cells[currentCell.Row + 1, currentCell.Column + 1].Value = parametersModel.nHSModel.NHS;
            worksheet.Cells[currentCell.Row + 2, currentCell.Column + 1].Value = parametersModel.nHSModel.NonNHS;

            ExcelHelper.ApplyBorder(worksheet.Cells[currentCell.Row, currentCell.Column, currentCell.Row + 2, currentCell.Column + 1]);
            ExcelHelper.HorizontalCenterAlign(
                worksheet.Cells[currentCell.Row + 1, currentCell.Column + 1, currentCell.Row + 2, currentCell.Column + 1]);

            var rowNo = currentCell.Row + 4;
            ExcelHelper.MergeCells(worksheet, rowNo, currentCell.Column, rowNo, currentCell.Column + 1);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1], Color.White);
            worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1].Value = "6A19 BPN";
            worksheet.Cells[rowNo + 1, currentCell.Column].Value = "1";
            worksheet.Cells[rowNo + 2, currentCell.Column].Value = "2";
            worksheet.Cells[rowNo + 3, currentCell.Column].Value = "3";
            worksheet.Cells[rowNo + 4, currentCell.Column].Value = "4";
            worksheet.Cells[rowNo + 5, currentCell.Column].Value = "H";

            bpnValueCellTracker.Add("1", (rowNo + 1, currentCell.Column + 1));
            bpnValueCellTracker.Add("2", (rowNo + 2, currentCell.Column + 1));
            bpnValueCellTracker.Add("3", (rowNo + 3, currentCell.Column + 1));
            bpnValueCellTracker.Add("4", (rowNo + 4, currentCell.Column + 1));
            bpnValueCellTracker.Add("H", (rowNo + 5, currentCell.Column + 1));

            worksheet.Cells[rowNo + 6, currentCell.Column].Value = "L";
            worksheet.Cells[rowNo + 7, currentCell.Column].Value = "T";
            worksheet.Cells[rowNo + 8, currentCell.Column].Value = "D";
            worksheet.Cells[rowNo + 9, currentCell.Column].Value = "N";
            worksheet.Cells[rowNo + 10, currentCell.Column].Value = "Blank";

            bpnValueCellTracker.Add("L", (rowNo + 6, currentCell.Column + 1));
            bpnValueCellTracker.Add("T", (rowNo + 7, currentCell.Column + 1));
            bpnValueCellTracker.Add("D", (rowNo + 8, currentCell.Column + 1));
            bpnValueCellTracker.Add("N", (rowNo + 9, currentCell.Column + 1));
            bpnValueCellTracker.Add("Blank", (rowNo + 10, currentCell.Column + 1));

            foreach (var item in bpnValueCellTracker)
            {
                if (parametersModel.BPNValues.Contains(item.Key))
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "N";
                }
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[item.Value.row, item.Value.col]);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, currentCell.Column, rowNo + 10, currentCell.Column + 1]);

            rowNo = currentCell.Row + 17;
            ExcelHelper.MergeCells(worksheet, rowNo, currentCell.Column, rowNo, currentCell.Column + 1);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1], Color.White);
            worksheet.Cells[rowNo, currentCell.Column, rowNo, currentCell.Column + 1].Value = "Bridge Length";
            worksheet.Cells[rowNo + 1, currentCell.Column].Value = "8-20";
            worksheet.Cells[rowNo + 2, currentCell.Column].Value = "NBIS Length";

            worksheet.Cells[rowNo + 1, currentCell.Column + 1].Value = parametersModel.LengthBetween8and20;
            worksheet.Cells[rowNo + 2, currentCell.Column + 1].Value = parametersModel.LengthGreaterThan20;
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo + 1, currentCell.Column + 1, rowNo + 2, currentCell.Column + 1]);

            ExcelHelper.MergeCells(worksheet, rowNo + 4, currentCell.Column, rowNo + 4, currentCell.Column + 1);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo + 4, currentCell.Column, rowNo + 4, currentCell.Column + 1], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo + 4, currentCell.Column, rowNo + 4, currentCell.Column + 1], Color.White);
            worksheet.Cells[rowNo + 4, currentCell.Column, rowNo + 4, currentCell.Column + 1].Value = "Status";
            worksheet.Cells[rowNo + 5, currentCell.Column].Value = "Open";
            worksheet.Cells[rowNo + 6, currentCell.Column].Value = "Closed";
            worksheet.Cells[rowNo + 7, currentCell.Column].Value = "P3";
            worksheet.Cells[rowNo + 8, currentCell.Column].Value = "Posted";

            //[TODO]: setting up value based on a substring is a bad idea. It can slow down the app. Jake and PennDot has decided to this way
            worksheet.Cells[rowNo + 5, currentCell.Column + 1].Value = jurisdictionExpression.Contains("[POST_STATUS]<>'OPEN'") ? "N" : "Y"; // open
            worksheet.Cells[rowNo + 6, currentCell.Column + 1].Value = jurisdictionExpression.Contains("[POST_STATUS]<>'CLOSED'") ? "N" : "Y"; // closed
            worksheet.Cells[rowNo + 7, currentCell.Column + 1].Value = jurisdictionExpression.Contains("[P3]='0'") ? "N" : "Y"; // P3
            worksheet.Cells[rowNo + 8, currentCell.Column + 1].Value = jurisdictionExpression.Contains("[POST_STATUS]<>'POSTED'") ? "N" : "Y"; // P3
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo + 5, currentCell.Column + 1, rowNo + 8, currentCell.Column + 1]);

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, currentCell.Column, rowNo + 8, currentCell.Column + 1]);
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowNo + 7, currentCell.Column + 1]);

            rowNo = currentCell.Row + 8; // currentCell.Row is equal to 6
            ExcelHelper.MergeCells(worksheet, rowNo, currentCell.Column + 3, rowNo, currentCell.Column + 5);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, currentCell.Column + 3, rowNo, currentCell.Column + 5], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, currentCell.Column + 3, rowNo, currentCell.Column + 5], Color.White);
            worksheet.Cells[rowNo, currentCell.Column + 3, rowNo, currentCell.Column + 5].Value = "5A21 Owner";

            var rowNoForMap = currentCell.Row + 8;
            var ownerCodeValueTracker = new Dictionary<string, (int row, int col)> { {"01", (++rowNoForMap, currentCell.Column + 5)},
                {"02", (++rowNoForMap, currentCell.Column + 5)},{"03",(++rowNoForMap, currentCell.Column + 5)},{"04",(++rowNoForMap, currentCell.Column + 5)},
                {"11",(++rowNoForMap, currentCell.Column + 5)},{"12",(++rowNoForMap, currentCell.Column + 5)},
                {"21",(++rowNoForMap, currentCell.Column + 5)},{"25",(++rowNoForMap, currentCell.Column + 5)},
                {"26",(++rowNoForMap, currentCell.Column + 5)},{"27",(++rowNoForMap, currentCell.Column + 5)},{"31",(++rowNoForMap, currentCell.Column + 5)},
                {"32",(++rowNoForMap, currentCell.Column + 5)},{"60",(++rowNoForMap, currentCell.Column + 5)},{"62",(++rowNoForMap, currentCell.Column + 5)},
                {"64",(++rowNoForMap, currentCell.Column + 5)},{"66",(++rowNoForMap, currentCell.Column + 5)},{"68",(++rowNoForMap, currentCell.Column + 5)},
                {"69",(++rowNoForMap, currentCell.Column + 5)},{"70",(++rowNoForMap, currentCell.Column + 5)},
                {"80",(++rowNoForMap, currentCell.Column + 5)},{"XX",(++rowNoForMap, currentCell.Column + 5)} };

            foreach (var item in ownerCodeValueTracker)
            {
                ExcelHelper.MergeCells(worksheet, item.Value.row, item.Value.col - 2, item.Value.row, item.Value.col - 1, false);
                worksheet.Cells[item.Value.row, item.Value.col - 2, item.Value.row, item.Value.col - 1].Value = MappingContent.OwnerCodeForReport(item.Key);
            }

            foreach (var item in ownerCodeValueTracker)
            {
                if (parametersModel.OwnerCode.Contains(item.Key))
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "N";
                }
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[item.Value.row, item.Value.col]);
            }

            var rowForStyle = currentCell.Row + 8; // currentCell.Row is equal to 6
            var colForStyle = currentCell.Column + 3;
            ExcelHelper.ApplyBorder(worksheet.Cells[rowForStyle, colForStyle, rowForStyle + 21, colForStyle + 2]);

            ExcelHelper.MergeCells(worksheet, rowForStyle, colForStyle + 4, rowForStyle, colForStyle + 6);
            ExcelHelper.ApplyColor(worksheet.Cells[rowForStyle, colForStyle + 4, rowForStyle, colForStyle + 6], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowForStyle, colForStyle + 4, rowForStyle, colForStyle + 6], Color.White);
            worksheet.Cells[rowForStyle, colForStyle + 4, rowForStyle, colForStyle + 6].Value = "5C22 Functional Class";

            ExcelHelper.MergeCells(worksheet, rowForStyle + 1, colForStyle + 4, rowForStyle + 1, colForStyle + 6);
            ExcelHelper.ApplyColor(worksheet.Cells[rowForStyle + 1, colForStyle + 4, rowForStyle + 1, colForStyle + 6], Color.DimGray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowForStyle + 1, colForStyle + 4, rowForStyle + 1, colForStyle + 6], Color.White);
            worksheet.Cells[rowForStyle + 1, colForStyle + 4, rowForStyle + 1, colForStyle + 6].Value = "Rural";

            var functionalClassValueTracker = new Dictionary<string, (int row, int col)> { {"01",(rowForStyle + 2, colForStyle + 6)},
                {"02",(rowForStyle + 3, colForStyle + 6)}, {"03",(rowForStyle + 4, colForStyle + 6)},{"06",(rowForStyle + 5, colForStyle + 6)},
                {"07",(rowForStyle + 6, colForStyle + 6)},{"08",(rowForStyle + 7, colForStyle + 6)},{"09",(rowForStyle + 8, colForStyle + 6)},
                {"NN",(rowForStyle + 9, colForStyle + 6)},{"11",(rowForStyle + 11, colForStyle + 6)},
                {"12",(rowForStyle + 12, colForStyle + 6)},{"14",(rowForStyle + 13, colForStyle + 6)},{"16",(rowForStyle + 14, colForStyle + 6)},
                {"17",(rowForStyle + 15, colForStyle + 6)},{"19",(rowForStyle + 16, colForStyle + 6)},
                {"99",(rowForStyle + 18, colForStyle + 6)} };

            foreach (var item in functionalClassValueTracker)
            {
                ExcelHelper.MergeCells(worksheet, item.Value.row, item.Value.col - 2, item.Value.row, item.Value.col - 1, false);
                worksheet.Cells[item.Value.row, item.Value.col - 2, item.Value.row, item.Value.col - 1].Value = _summaryReportHelper.FullFunctionalClassDescription(item.Key);
            }

            ExcelHelper.MergeCells(worksheet, rowForStyle + 10, colForStyle + 4, rowForStyle + 10, colForStyle + 6);
            ExcelHelper.ApplyColor(worksheet.Cells[rowForStyle + 10, colForStyle + 4, rowForStyle + 10, colForStyle + 6], Color.DimGray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowForStyle + 10, colForStyle + 4, rowForStyle + 10, colForStyle + 6], Color.White);
            worksheet.Cells[rowForStyle + 10, colForStyle + 4, rowForStyle + 10, colForStyle + 6].Value = "Urban";

            ExcelHelper.MergeCells(worksheet, rowForStyle + 17, colForStyle + 4, rowForStyle + 17, colForStyle + 5, false);
            worksheet.Cells[rowForStyle + 17, colForStyle + 4, rowForStyle + 17, colForStyle + 5].Value = _summaryReportHelper.FullFunctionalClassDescription("NN");

            foreach (var item in functionalClassValueTracker)
            {
                if (parametersModel.FunctionalClass.Contains(item.Key))
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "Y";
                }
                else
                {
                    worksheet.Cells[item.Value.row, item.Value.col].Value = "N";
                }
                ExcelHelper.HorizontalCenterAlign(worksheet.Cells[item.Value.row, item.Value.col]);
            }
            worksheet.Cells[rowForStyle + 17, colForStyle + 6].Value = worksheet.Cells[functionalClassValueTracker["NN"].row, functionalClassValueTracker["NN"].col].Value;
            ExcelHelper.ApplyBorder(worksheet.Cells[rowForStyle, colForStyle + 4, rowForStyle + 18, colForStyle + 6]);
            ExcelHelper.HorizontalCenterAlign(worksheet.Cells[rowForStyle + 17, colForStyle + 6]);

            return currentCell;
        }

        private CurrentCell FillSimulationDetails(ExcelWorksheet worksheet, int yearCount, Simulation simulation, CurrentCell currentCell)
        {
            currentCell.Column += 5; // curr col is now 6
            var rowNo = currentCell.Row; // 6
            var colNo = currentCell.Column; // 6
            ExcelHelper.MergeCells(worksheet, rowNo, colNo, rowNo, colNo + 2);
            ExcelHelper.MergeCells(worksheet, rowNo + 2, colNo, rowNo + 2, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo + 4, colNo, rowNo + 4, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo + 6, colNo, rowNo + 6, colNo + 1);

            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, colNo, rowNo, colNo + 2], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, colNo, rowNo, colNo + 2], Color.White);

            worksheet.Cells[rowNo, colNo, rowNo, colNo + 2].Value = "Investment:";
            worksheet.Cells[rowNo + 2, colNo, rowNo + 2, colNo + 1].Value = "Start Year:";
            worksheet.Cells[rowNo + 4, colNo, rowNo + 4, colNo + 1].Value = "Analysis Period:";
            worksheet.Cells[rowNo + 6, colNo, rowNo + 6, colNo + 1].Value = "Inflation Rate:";

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, colNo, rowNo + 6, colNo + 2]);

            worksheet.Cells[rowNo + 2, colNo + 2].Value = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod; //StartYear;
            worksheet.Cells[rowNo + 4, colNo + 2].Value = yearCount;
            worksheet.Cells[rowNo + 6, colNo + 2].Value = simulation.InvestmentPlan.InflationRatePercentage; //inflationRate;

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo + 2, colNo + 2, rowNo + 6, colNo + 2]);
            currentCell.Column += 6; // col = 12

            return currentCell;
        }

        private CurrentCell FillAnalysisDetails(ExcelWorksheet worksheet, Simulation simulation, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row; // row no = 6
            var colNo = currentCell.Column; // col no = 12

            ExcelHelper.MergeCells(worksheet, rowNo, colNo, rowNo, colNo + 3);
            ExcelHelper.MergeCells(worksheet, rowNo + 2, colNo, rowNo + 2, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo + 4, colNo, rowNo + 4, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo + 6, colNo, rowNo + 6, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo + 8, colNo, rowNo + 8, colNo + 1);

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, colNo, rowNo + 8, colNo + 3]);

            ExcelHelper.MergeCells(worksheet, rowNo + 2, colNo + 2, rowNo + 2, colNo + 3, false);
            ExcelHelper.MergeCells(worksheet, rowNo + 4, colNo + 2, rowNo + 4, colNo + 3, false);
            ExcelHelper.MergeCells(worksheet, rowNo + 6, colNo + 2, rowNo + 6, colNo + 3, false);
            ExcelHelper.MergeCells(worksheet, rowNo + 8, colNo + 2, rowNo + 8, colNo + 3, false);

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo + 2, colNo + 2, rowNo + 8, colNo + 3]);

            worksheet.Cells[rowNo, colNo, rowNo, colNo + 3].Value = "Analysis:";
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, colNo, rowNo, colNo + 3], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, colNo, rowNo, colNo + 3], Color.White);
            worksheet.Cells[rowNo + 2, colNo, rowNo + 2, colNo + 1].Value = "Optimization:";
            worksheet.Cells[rowNo + 4, colNo, rowNo + 4, colNo + 1].Value = "Budget:";
            worksheet.Cells[rowNo + 6, colNo, rowNo + 6, colNo + 1].Value = "Weighting:";
            worksheet.Cells[rowNo + 8, colNo, rowNo + 8, colNo + 1].Value = "Benefit:";

            worksheet.Cells[rowNo + 2, colNo + 2, rowNo + 2, colNo + 3].Value = simulation.AnalysisMethod.OptimizationStrategy;

            worksheet.Cells[rowNo + 4, colNo + 2, rowNo + 4, colNo + 3].Value = simulation.AnalysisMethod.SpendingStrategy; //BudgetType;
            worksheet.Cells[rowNo + 6, colNo + 2, rowNo + 6, colNo + 3].Value = simulation.AnalysisMethod.Weighting.Name; //WeightingAttribute;
            worksheet.Cells[rowNo + 8, colNo + 2, rowNo + 8, colNo + 3].Value = simulation.AnalysisMethod.Benefit.Attribute.Name; //BenefitAttribute;

            currentCell.Row += 10; // row = 16, col = 12

            return currentCell;
        }

        private CurrentCell FillPriorities(ExcelWorksheet worksheet, Simulation simulation, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row; // row no = 19
            var colNo = currentCell.Column; // col no = 12
            ExcelHelper.MergeCells(worksheet, rowNo, colNo, rowNo, worksheet.Dimension.End.Column);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNo, colNo, rowNo, worksheet.Dimension.End.Column], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNo, colNo, rowNo, worksheet.Dimension.End.Column], Color.White);
            ExcelHelper.MergeCells(worksheet, rowNo + 1, colNo + 1, rowNo + 1, worksheet.Dimension.End.Column);

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo + 1, colNo, rowNo + 1, worksheet.Dimension.End.Column]);

            worksheet.Cells[rowNo, colNo, rowNo, colNo + 14].Value = "Analysis Priorites:";

            worksheet.Cells[rowNo, colNo].Value = "Number";
            worksheet.Cells[rowNo, colNo + 1].Value = "Criteria:";

            var cells = worksheet.Cells[rowNo, colNo];
            ExcelHelper.ApplyStyle(cells);
            var startingRow = rowNo + 2;

            var priorites = simulation.AnalysisMethod.BudgetPriorities.OrderBy(_ => _.PriorityLevel);
            foreach (var item in priorites)
            {
                ExcelHelper.MergeCells(worksheet, startingRow, colNo + 1, startingRow, worksheet.Dimension.End.Column, false);
                worksheet.Cells[startingRow, colNo].Value = item.PriorityLevel;
                worksheet.Cells[startingRow, colNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[startingRow, colNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[startingRow, colNo + 1].Value = item.Criterion.Expression;
                worksheet.Row(startingRow).Height = 33;
                startingRow++;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo + 2, colNo, startingRow, worksheet.Dimension.End.Column]);

            currentCell.Row = startingRow;
            return currentCell;
        }

        private CurrentCell FillJurisdictionCriteria(ExcelWorksheet worksheet, Simulation simulation, CurrentCell currentCell)
        {
            var rowNo = currentCell.Row; // row no = 16
            var colNo = currentCell.Column; // col no = 12
            ExcelHelper.MergeCells(worksheet, rowNo, colNo, rowNo + 1, colNo + 1);
            ExcelHelper.MergeCells(worksheet, rowNo, colNo + 2, rowNo + 1, colNo + 14, false);

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNo, colNo, rowNo + 1, colNo + 14]);

            worksheet.Cells[rowNo, colNo, rowNo, colNo + 1].Value = "Jurisdiction Criteria:";
            worksheet.Cells[rowNo, colNo + 2, rowNo, colNo + 14].Value = simulation.AnalysisMethod.Filter.Expression; //criteria;

            currentCell.Row += 3; // row no = 19, col no = 12

            return currentCell;
        }

        private void FillBudgetSplitCriteria(ExcelWorksheet worksheet, CurrentCell currentCell, Simulation simulation)
        {
            var rowNum = currentCell.Row;
            var colNum = currentCell.Column; // 12
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            rowNum++;
            var startingRow = rowNum;
            ExcelHelper.MergeCells(worksheet, rowNum, colNum, rowNum, colNum + 2);
            ExcelHelper.ApplyColor(worksheet.Cells[rowNum, colNum, rowNum, colNum + 2], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[rowNum, colNum, rowNum, worksheet.Dimension.End.Column], Color.White);

            worksheet.Cells[rowNum, colNum].Value = "Budget Split Criteria";
            worksheet.Cells[++rowNum, colNum].Value = "Rank";
            worksheet.Cells[rowNum, colNum + 1].Value = "Amount";
            worksheet.Cells[rowNum, colNum + 2].Value = "Percentage";

            ExcelHelper.ApplyBorder(worksheet.Cells[rowNum, colNum, rowNum, colNum + 2]);
            var cells = worksheet.Cells[rowNum, colNum, rowNum, colNum + 2];
            ExcelHelper.ApplyStyle(cells);

            foreach (var item in simulation.InvestmentPlan.CashFlowRules)
            {
                var i = 0;
                foreach(var rule in item.DistributionRules)
                {
                    i++;
                    worksheet.Cells[++rowNum, colNum].Value = i;
                    worksheet.Cells[rowNum, colNum + 1].Style.Numberformat.Format = currencyFormat;
                    worksheet.Cells[rowNum, colNum + 1].Value = rule.CostCeiling;
                    worksheet.Cells[rowNum, colNum + 2].Value = rule.Expression;
                }
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startingRow, colNum, rowNum, colNum + 2]);
        }

        private void FillInvestmentAndBudgetCriteria(ExcelWorksheet worksheet, Simulation simulation)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            worksheet.Cells[38, 1].Value = "Years";
            worksheet.Cells[38, 2].Value = "Total Funding";
            ExcelHelper.ApplyColor(worksheet.Cells[38, 2], Color.DimGray);
            ExcelHelper.SetTextColor(worksheet.Cells[38, 2], Color.White);
            ExcelHelper.ApplyColor(worksheet.Cells[38, 1], Color.DimGray);
            ExcelHelper.SetTextColor(worksheet.Cells[38, 1], Color.White);

            var startingRowInvestment = 40;
            var startingBudgetHeaderColumn = 2;
            var nextBudget = 0;
            var investmentGrid = new SortedDictionary<int, Dictionary<string, decimal?>>();
            var startYear = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
            foreach (var budgets in simulation.InvestmentPlan.Budgets)
            {
                var i = 0;
                foreach (var item in budgets.YearlyAmounts)
                {
                    if (!investmentGrid.ContainsKey(startYear + i))
                    {
                        investmentGrid.Add(startYear + i, new Dictionary<string, decimal?>() {
                            {budgets.Name, item.Value }
                        });
                    }
                    else
                    {
                        if (!investmentGrid[startYear + i].ContainsKey(budgets.Name))
                        {
                            investmentGrid[startYear + i].Add(budgets.Name, item.Value);
                        }
                        else
                        {
                            investmentGrid[startYear + i][budgets.Name] += item.Value;
                        }
                    }
                    i++;
                }
            }

            var firstRow = true;
            foreach (var item in investmentGrid)
            {
                worksheet.Cells[startingRowInvestment, 1].Value = item.Key;
                foreach (var budget in item.Value)
                {
                    if (firstRow == true)
                    {
                        worksheet.Cells[39, startingBudgetHeaderColumn + nextBudget].Value = budget.Key;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Value = budget.Value.Value;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Style.Numberformat.Format = currencyFormat;
                        nextBudget++;
                        continue;
                    }
                    for (var column = startingBudgetHeaderColumn; column <= item.Value.Count + 1; column++)
                    {
                        if (worksheet.Cells[39, column].Value.ToString() == budget.Key)
                        {
                            worksheet.Cells[startingRowInvestment, column].Style.Numberformat.Format = currencyFormat;
                            worksheet.Cells[startingRowInvestment, column].Value = budget.Value.Value;
                            break;
                        }
                    }
                }
                startingRowInvestment++;
                firstRow = false;
                nextBudget = 0;
            }
            ExcelHelper.MergeCells(worksheet, 38, 1, 39, 1);
            if(simulation.InvestmentPlan.Budgets.Count > 0)
            {
                ExcelHelper.MergeCells(worksheet, 38, 2, 38, simulation.InvestmentPlan.Budgets.Count + 1);
                ExcelHelper.ApplyBorder(worksheet.Cells[38, 1, startingRowInvestment - 1, simulation.InvestmentPlan.Budgets.Count + 1]);
            }
            FillBudgetCriteria(worksheet, startingRowInvestment, simulation);
        }

        private void FillBudgetCriteria(ExcelWorksheet worksheet, int startingRowInvestment, Simulation simulation)
        {
            var rowToApplyBorder = startingRowInvestment + 2;
            worksheet.Cells[startingRowInvestment + 2, 1].Value = "Budget Criteria";
            ExcelHelper.MergeCells(worksheet, startingRowInvestment + 2, 1, startingRowInvestment + 2, 5);
            ExcelHelper.ApplyColor(worksheet.Cells[startingRowInvestment + 2, 1, startingRowInvestment + 2, 5], Color.Gray);
            ExcelHelper.SetTextColor(worksheet.Cells[startingRowInvestment + 2, 1, startingRowInvestment + 2, 5], Color.White);

            worksheet.Cells[startingRowInvestment + 3, 1].Value = "Budget Name";
            worksheet.Cells[startingRowInvestment + 3, 2].Value = "Criteria";
            ExcelHelper.MergeCells(worksheet, startingRowInvestment + 3, 2, startingRowInvestment + 3, 5);
            var cells = worksheet.Cells[startingRowInvestment + 3, 1, startingRowInvestment + 3, 2];
            ExcelHelper.ApplyStyle(cells);
            foreach (var item in simulation.InvestmentPlan.BudgetConditions)
            {
                worksheet.Cells[startingRowInvestment + 4, 1].Value = item.Budget.Name;
                worksheet.Cells[startingRowInvestment + 4, 2].Value = item.Criterion.Expression;
                ExcelHelper.MergeCells(worksheet, startingRowInvestment + 4, 2, startingRowInvestment + 4, 5, false);
                startingRowInvestment++;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[rowToApplyBorder, 1, startingRowInvestment + 3, 5]);
        }

        #endregion
    }
}
