using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Models.SummaryReport;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.UnfundedTreatmentTime
{
    public class UnfundedTreatmentTime : IUnfundedTreatmentTime
    {
        private readonly IUnfundedTreatmentCommon _unfundedTreatmentCommon;

        public UnfundedTreatmentTime(IUnfundedTreatmentCommon unfundedTreatmentCommon)
        {
            _unfundedTreatmentCommon = unfundedTreatmentCommon;
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
            AddDynamicDataCells(unfundedTreatmentTimeWorksheet, simulationOutput, currentCell);

            unfundedTreatmentTimeWorksheet.Cells.AutoFitColumns();
        }

        #region Private methods

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section, treatment
            var treatmentsPerSection = new SortedDictionary<int, List<Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>>>();
            foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
            {
                var untreatedSections =
                    year.Sections.Where(
                        sect => sect.TreatmentCause == TreatmentCause.NoSelection &&
                        sect.ValuePerNumericAttribute["RISK_SCORE"] > 15000 &&
                        sect.TreatmentOptions.Count > 0
                        ).ToList();

                foreach (var section in untreatedSections)
                {
                    var facilityId = int.Parse(section.FacilityName);

                    var treatmentOptions = section.TreatmentOptions.
                        Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                    treatmentOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));

                    var chosenTreatment = treatmentOptions.FirstOrDefault();
                    if (chosenTreatment != null)
                    {
                        var newTuple = new Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>(year, section, chosenTreatment);
                        if (!treatmentsPerSection.ContainsKey(facilityId))
                        {
                            treatmentsPerSection.Add(facilityId, new List<Tuple<SimulationYearDetail, SectionDetail, TreatmentOptionDetail>> { newTuple });
                        }
                        else
                        {
                            treatmentsPerSection[facilityId].Add(newTuple);
                        }
                    }
                }
            }

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

            foreach (var facilityList in treatmentsPerSection.Values)
            {
                foreach (var facilityTuple in facilityList)
                {
                    var section = facilityTuple.Item2;
                    var year = facilityTuple.Item1;
                    var treatment = facilityTuple.Item3;
                    _unfundedTreatmentCommon.FillDataInWorkSheet(worksheet, currentCell, section, year.Year, treatment);
                    currentCell.Row++;
                    currentCell.Column = 1;
                }
            }
        }

        #endregion Private methods
    }
}
