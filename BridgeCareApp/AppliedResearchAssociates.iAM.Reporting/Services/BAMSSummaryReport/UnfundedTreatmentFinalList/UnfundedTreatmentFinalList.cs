using System;
using System.Collections.Generic;
using System.Linq;

using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.UnfundedTreatmentFinalList
{
    public class UnfundedTreatmentFinalList : IUnfundedTreatmentFinalList
    {
        private IUnfundedTreatmentCommon _unfundedTreatmentCommon;

        public UnfundedTreatmentFinalList()
        {
            _unfundedTreatmentCommon = new UnfundedTreatmentCommon.UnfundedTreatmentCommon();
            if (_unfundedTreatmentCommon == null) { throw new ArgumentNullException(nameof(_unfundedTreatmentCommon)); }

        }

        public void Fill(ExcelWorksheet unfundedTreatmentTimeWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = _unfundedTreatmentCommon.AddHeadersCells(unfundedTreatmentTimeWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = unfundedTreatmentTimeWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }

            // Add "Total Unfunded Amount" header & calculated cell
            var costColumn = unfundedTreatmentTimeWorksheet.Dimension.Columns;
            var costColumnLetter = OfficeOpenXml.ExcelCellAddress.GetColumnLetter(costColumn);
            var totalColumn = costColumn + 2;
            unfundedTreatmentTimeWorksheet.Cells[1, totalColumn].Value = "Total Unfunded Amount:";
            unfundedTreatmentTimeWorksheet.Cells[2, totalColumn].Formula = $"=SUM({costColumnLetter}:{costColumnLetter})";
            unfundedTreatmentTimeWorksheet.Cells[2, totalColumn].Style.Numberformat.Format = @"_($* #,##0_);_($*  #,##0);_($* "" - ""??_);(@_)";
            ExcelHelper.ApplyBorder(unfundedTreatmentTimeWorksheet.Cells[1, totalColumn, 2, totalColumn]);

            unfundedTreatmentTimeWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(unfundedTreatmentTimeWorksheet, simulationOutput, currentCell);
            unfundedTreatmentTimeWorksheet.Calculate();  // calculation is set to manual, so force calculation of the total now

            unfundedTreatmentTimeWorksheet.Cells.AutoFitColumns();
        }

        #region Private methods

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>>();
            var validFacilityIds = new List<int>(); // It will keep the Ids which has gone unfunded for all the years
            var firstYear = true;
            foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
            {
                var untreatedSections = _unfundedTreatmentCommon.GetUntreatedSections(year);
                if (firstYear)
                {
                    validFacilityIds.AddRange(untreatedSections.Select(_ => int.Parse(_.FacilityName.Split('-')[0])));
                    firstYear = false;
                    if(simulationOutput.Years.Count > 1)
                    {
                        continue;
                    }
                }
                else
                {
                    validFacilityIds = validFacilityIds.Intersect(untreatedSections.Select(_ => int.Parse(_.FacilityName.Split('-')[0]))).ToList();
                }

                foreach (var section in untreatedSections)
                {
                    var facilityId = int.Parse(section.FacilityName.Split('-')[0]);
                    if (!treatmentsPerSection.ContainsKey(facilityId)) // skip if we already have a treatment for this section
                    {
                        var treatmentOptions = section.TreatmentOptions.
                            Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                        treatmentOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));

                        var chosenTreatment = treatmentOptions.FirstOrDefault();
                        if (chosenTreatment != null)
                        {
                            var newTuple = new Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>(year, section, chosenTreatment);

                            if (!validFacilityIds.Contains(facilityId))
                            {
                                if (treatmentsPerSection.ContainsKey(facilityId))
                                {
                                    treatmentsPerSection.Remove(facilityId);
                                }
                            }
                            else
                            {
                                if (!treatmentsPerSection.ContainsKey(facilityId))
                                {
                                    treatmentsPerSection.Add(facilityId, newTuple);
                                }
                            }
                        }
                    }
                }
            }

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

            foreach (var facilityTuple in treatmentsPerSection.Values)
            {
                var section = facilityTuple.Item2;
                var year = facilityTuple.Item1;
                var treatment = facilityTuple.Item3;
                _unfundedTreatmentCommon.FillDataInWorkSheet(worksheet, currentCell, section, year.Year, treatment);
                currentCell.Row++;
                currentCell.Column = 1;
            }
        }

        #endregion Private methods
    }
}
