using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Drawing;
using System;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class TreatmentsWorkSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;
        private WorkSummaryModel _workSummaryModel;
        //private HashSet<string> MPMSTreatments = new HashSet<string>();

        //private Dictionary<int, decimal> TotalAsphaltSpent = new Dictionary<int, decimal>();
        //private int AsphaltTotalRow = 0;

        public TreatmentsWorkSummary(WorkSummaryModel workSummaryModel)
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        public void FillTreatmentsWorkSummarySections(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int count)>> costAndLengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )

        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillFullDepthAsphaltTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillCompositeTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillConcreteTreatments(worksheet, currentCell, simulationYears, costAndLengthPerTreatmentPerYear, simulationTreatments);
            FillTreatmentGroups(worksheet, currentCell, simulationYears);
            FillWorkTypes(worksheet, currentCell, simulationYears);
        }

        private void FillFullDepthAsphaltTreatments(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Full Depth Asphalt Pavement Treatments", "PAMS Full Depth Asphalt Treatments");

            var categoryHTreatments = simulationTreatments.Where(treatment => treatment.Name.ToLower().StartsWith("h")).ToList();
            categoryHTreatments.AddRange(simulationTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)));


            AddFullDepthAsphaltTreatmentSegmentMiles(worksheet, currentCell,
                lengthPerTreatmentPerYear,
                categoryHTreatments
                );
        }


        private void AddFullDepthAsphaltTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = PAMSConstants.AsphaltTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in lengthPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double asphaltTotalCount = 0;
                foreach (var treatment in simulationTreatments)
                {
                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                    worksheet.Cells[row, column].Value = costAndLength.length / 5280;
                    //projectRowNumberModel.TreatmentsCount.Add(treatment.Name + "_" + yearlyValues.Key, row);
                    row++;
                }
                worksheet.Cells[row, column].Value = asphaltTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }
 


    private void FillCompositeTreatments(
        ExcelWorksheet worksheet,
        CurrentCell currentCell,
        List<int> simulationYears,
        Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
        List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
        )
    {
        _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Composite Pavement Treatments", "PAMS Composite Treatments");

            var categoryHTreatments = simulationTreatments.Where(treatment => treatment.Name.ToLower().StartsWith("h")).ToList();
            categoryHTreatments.AddRange(simulationTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)));

            AddCompositeTreatmentSegmentMiles(worksheet, currentCell,
            lengthPerTreatmentPerYear,
            categoryHTreatments
            );
    }


    private void AddCompositeTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
        Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
        List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
        )
    {
        int startRow, startColumn, row, column;
        _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
        _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
        worksheet.Cells[row++, column].Value = PAMSConstants.CompositeTotal;
        column++;
        var fromColumn = column + 1;
        foreach (var yearlyValues in lengthPerTreatmentPerYear)
        {
            row = startRow;
            column = ++column;
            double CompositeTotalCount = 0;
            foreach (var treatment in simulationTreatments)
            {
                yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                worksheet.Cells[row, column].Value = costAndLength.length / 5280;
                //projectRowNumberModel.TreatmentsCount.Add(treatment.Name + "_" + yearlyValues.Key, row);
                row++;
            }
            worksheet.Cells[row, column].Value = CompositeTotalCount;
        }
        ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
        ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176));
            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
    }



        private void FillConcreteTreatments(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var categoryJTreatments = simulationTreatments.Where(treatment => treatment.Name.ToLower().StartsWith("j")).ToList();
            categoryJTreatments.AddRange(simulationTreatments.Where(treatment => treatment.Name.ToLower().Equals(PAMSConstants.NoTreatment)));

            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Segment Miles of Concrete Pavement Treatments", "PAMS Concrete Treatments");
            AddConcreteTreatmentSegmentMiles(worksheet, currentCell,
                lengthPerTreatmentPerYear,
                categoryJTreatments
                );
        }


        private void AddConcreteTreatmentSegmentMiles(ExcelWorksheet worksheet, CurrentCell currentCell,
            Dictionary<int, Dictionary<string, (decimal treatmentCost, int length)>> lengthPerTreatmentPerYear,
            List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, simulationTreatments, ref row, ref column);
            worksheet.Cells[row++, column].Value = PAMSConstants.ConcreteTotal;
            column++;
            var fromColumn = column + 1;
            foreach (var yearlyValues in lengthPerTreatmentPerYear)
            {
                row = startRow;
                column = ++column;
                double ConcreteTotalCount = 0;
                foreach (var treatment in simulationTreatments)
                {
                    yearlyValues.Value.TryGetValue(treatment.Name, out var costAndLength);
                    worksheet.Cells[row, column].Value = costAndLength.length / 5280;
                    //projectRowNumberModel.TreatmentsCount.Add(treatment.Name + "_" + yearlyValues.Key, row);
                    row++;
                }
                worksheet.Cells[row, column].Value = ConcreteTotalCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.FromArgb(180, 198, 231));
            ExcelHelper.ApplyColor(worksheet.Cells[row, fromColumn, row, column], Color.FromArgb(132, 151, 176));

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillTreatmentGroups(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears //,
                                      //Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
                                      //List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "PAMS Treatment Groups Totals");
            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);

            //return workTypeTotalCulvert;
            return null;
        }

        private Dictionary<TreatmentCategory, SortedDictionary<int, decimal>> FillWorkTypes(ExcelWorksheet worksheet, CurrentCell currentCell,
            List<int> simulationYears //,
                                      //Dictionary<int, Dictionary<string, (decimal treatmentCost, int bridgeCount)>> costPerTreatmentPerYear,
                                      //List<(string Name, AssetCategory AssetType, TreatmentCategory Category)> simulationTreatments
            )
        {
            var headerRange = new Range(currentCell.Row, currentCell.Row + 1);
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Total Budget", "Work Type Totals");
            worksheet.Cells[currentCell.Row, simulationYears.Count + 3].Value = "Total (all years)";
            //var workTypeComposite = AddCostsOfCompositeWork(worksheet, simulationYears, currentCell, costPerTreatmentPerYear, simulationTreatments);
            //worksheet.Cells[startRow, column + 2].Style.Numberformat.Format = "#0.00%";
            //worksheet.Cells[startRow, column + 3].Value = "Percentage spent on PRESERVATION/REHABILITATION/REPLACEMENT";
            //return workTypeTotalCulvert;
            return null;
        }

    };




}
