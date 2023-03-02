using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Models;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSAuditReport
{
    public class DataTab
    {
        private BridgesUnfundedTreatments _bridgesUnfundedTreatments;
        private ReportHelper _reportHelper;

        public DataTab()
        {            
            _reportHelper = new ReportHelper();
            _bridgesUnfundedTreatments = new BridgesUnfundedTreatments();
        }

        public void Fill(ExcelWorksheet bridgesWorksheet, SimulationOutput simulationOutput)
        {
            // Add excel headers to excel.
            var currentCell = _bridgesUnfundedTreatments.AddHeadersCells(bridgesWorksheet);

            // Add row next to headers for filters and year numbers for dynamic data. Cover from
            // top, left to right, and bottom set of data.
            using (var autoFilterCells = bridgesWorksheet.Cells[3, 1, currentCell.Row, currentCell.Column - 1])
            {
                autoFilterCells.AutoFilter = true;
            }            

            bridgesWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;

            AddDynamicDataCells(bridgesWorksheet, simulationOutput, currentCell);

            bridgesWorksheet.Cells.AutoFitColumns();
            _bridgesUnfundedTreatments.PerformPostAutofitAdjustments(bridgesWorksheet);
        }

        public HashSet<string> GetRequiredAttributes() => new HashSet<string>()
        {
            $"{BAMSAuditReportConstants.DeckSeeded}",
            $"{BAMSAuditReportConstants.SupSeeded}",
            $"{BAMSAuditReportConstants.SubSeeded}",
            $"{BAMSAuditReportConstants.CulvSeeded}",
            $"{BAMSAuditReportConstants.DeckDurationN}",
            $"{BAMSAuditReportConstants.SupDurationN}",
            $"{BAMSAuditReportConstants.SubDurationN}",
            $"{BAMSAuditReportConstants.CulvDurationN}"
        };

        private void AddDynamicDataCells(ExcelWorksheet worksheet, SimulationOutput simulationOutput, CurrentCell currentCell)
        {
            // facilityId, year, section // TODO define datamodel obj for DataTab and fill that up here then pass those to FillExcel,
            // TODO bridges in data tab need to match with bridges in Decision tab
            var treatmentsPerSection = new SortedDictionary<int, Tuple<SimulationYearDetail, AssetDetail>>();
            foreach (var initialAssetSummary in simulationOutput.InitialAssetSummaries)
            {
                var brKey = CheckGetValue(initialAssetSummary.ValuePerNumericAttribute, "BRKEY_");
                foreach (var year in simulationOutput.Years.OrderBy(yr => yr.Year))
                {
                    var section = year.Assets.FirstOrDefault(_ => CheckGetValue(_.ValuePerNumericAttribute, "BRKEY_") == brKey);
                    var facilityId = Convert.ToInt32(brKey);
                    // Skip if we already have a treatment for this section
                    //if (!treatmentsPerSection.ContainsKey(facilityId))
                    //{
                    //    var treatmentOptions = section.TreatmentOptions.
                    //        Where(_ => section.TreatmentConsiderations.Exists(a => a.TreatmentName == _.TreatmentName)).ToList();
                    //    treatmentOptions.Sort((a, b) => b.Benefit.CompareTo(a.Benefit));
                    //    var chosenTreatment = treatmentOptions.FirstOrDefault();
                    //    if (chosenTreatment != null)
                    //    {
                    //        var newTuple = new Tuple<SimulationYearDetail, AssetDetail>(year, section);
                    //        if (validFacilityIds.Contains(facilityId))
                    //        {
                    //            treatmentsPerSection.Add(facilityId, newTuple);
                    //        }
                    //    }
                    //}
                }
            }
            

            currentCell.Row += 1; // Data starts here
            currentCell.Column = 1;

            //foreach (var facilityTuple in treatmentsPerSection.Values)
            //{
            //    var section = facilityTuple.Item2;
            //    var year = facilityTuple.Item1;                
            //    _bridgesUnfundedTreatments.FillDataInWorkSheet(worksheet, currentCell, section, year.Year);
                currentCell.Row++;
                currentCell.Column = 1;
            //}
        }

        private double CheckGetValue(Dictionary<string, double> valuePerNumericAttribute, string attribute) => _reportHelper.CheckAndGetValue<double>(valuePerNumericAttribute, attribute);        
    }
}
