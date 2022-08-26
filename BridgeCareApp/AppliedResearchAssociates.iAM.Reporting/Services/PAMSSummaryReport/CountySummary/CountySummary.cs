using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.ExcelHelpers;
using System.Reflection.PortableExecutable;
using OfficeOpenXml.Style;
using System.Drawing;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.PAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;

using CurrentCell = AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport.CurrentCell;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.CountySummary
{
    public class DistrictCounty
    {
        public int District { get; set; }
        public string County { get; set; }
    }

    public class DistrictCountyCost: DistrictCounty
    {
        public int Year { get; set; }
        public int Cost { get; set; }
    }

    public class DistrictCountyPercent: DistrictCounty
    {
        public int Year { get; set; }
        public int Cost { get; set; }
    }

    public class CountySummary : ICountySummary
    {
        private ISummaryReportHelper _summaryReportHelper;
        private IList<DistrictCounty> _uniqueDistrictCountyList = null;
        private CurrentCell currentCell = null;

        public CountySummary()
        {
            _summaryReportHelper = new SummaryReportHelper();
            if (_summaryReportHelper == null) { throw new ArgumentNullException(nameof(_summaryReportHelper)); }

            //create list object
            _uniqueDistrictCountyList = new List<DistrictCounty>();
        }

        public void Fill(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears, Simulation simulation)
        {
            FillDataToUseInExcel(worksheet, reportOutputData, simulationYears, simulation);
        }

        #region Private methods

        private void FillDataToUseInExcel(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears, Simulation simulation)
        {
            //create unique district county list
            BuildUniqueDistrictCountyList(reportOutputData);

            //Fill Budget By County
            FillBudgetByCountyInExcel(worksheet, reportOutputData, simulationYears, simulation);

            ////Fill Budget Percent By County
            FillBudgetPercentageByCountyInExcel(worksheet, reportOutputData, simulationYears, simulation);
        }

        private List<string> GetHeaders(List<int> simulationYears)
        {
            //static headers
            var headers = new List<string> {
                "District",
                "County"
            };

            //add years
            foreach (var year in simulationYears) { headers.Add(year.ToString()); }

            //return object
            return headers;
        }

        private void BuildUniqueDistrictCountyList(SimulationOutput reportOutputData)
        {
            if (reportOutputData?.InitialAssetSummaries?.Any() == true)
            {
                //create object
                IList<DistrictCounty> districtCountyList = new List<DistrictCounty>();

                foreach (var sectionSummary in reportOutputData.InitialAssetSummaries)
                {
                    //get and configure field values
                    var strDistrict = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "DISTRICT");
                    var district = 0; if (!string.IsNullOrEmpty(strDistrict) && !string.IsNullOrWhiteSpace(strDistrict)) { district = Convert.ToInt16(strDistrict); }
                    var county = _summaryReportHelper.checkAndGetValue<string>(sectionSummary.ValuePerTextAttribute, "COUNTY");

                    //check item in the list
                    var checkResultObject = districtCountyList
                                                .Where(w => w.District == district)
                                                .Where(w => w.County == county).FirstOrDefault();
                    if (checkResultObject == null)
                    {
                        //add object to list
                        districtCountyList.Add(new DistrictCounty() {
                            District = district,
                            County = county,
                        });
                    }
                }

                //sort district county list
                _uniqueDistrictCountyList = districtCountyList.OrderBy(o => o.District).ThenBy(t => t.County).ToList();
            }
        }

        private void FillBudgetByCountyInExcel(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears, Simulation simulation)
        {
            //Build Budget By County Headers
            var headers = GetHeaders(simulationYears);

            var rowNo = 1; var columnNo = 3;
            worksheet.Cells[rowNo, columnNo].Value = "Overall Dollars Recommended on Treatments by County";
            ExcelHelper.MergeCells(worksheet, rowNo, columnNo, rowNo, columnNo + (simulationYears.Count - 1));

            //build header cells
            rowNo++; columnNo = 0;
            for (int column = 0; column < headers.Count; column++)
            {
                columnNo = column + 1;
                worksheet.Cells[rowNo, columnNo].Value = headers[column];
            }

            ////build yearly budget list
            //foreach (var sectionData in reportOutputData.Years)
            //{
            //    var year = sectionData.Year;

            //    //get cost
            //    foreach (var section in sectionData.Assets)
            //    {
            //        var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
            //    }
            //}

            //fill data in cells
            currentCell = new CurrentCell { Row = rowNo, Column = columnNo }; var currentRowIndex = 0;
            foreach (var districtCountyObject in _uniqueDistrictCountyList)
            {
                rowNo++; columnNo = 1; currentRowIndex++;
                worksheet.Cells[rowNo, columnNo++].Value = districtCountyObject.District;
                worksheet.Cells[rowNo, columnNo++].Value = districtCountyObject.County;

                var rowsPerDistrict = _uniqueDistrictCountyList.Where(w => w.District == districtCountyObject.District).Count();                
                if (currentRowIndex > rowsPerDistrict-1)
                {
                    //reset index
                    rowNo++; columnNo = 1; currentRowIndex = 0;

                    //Add Summary Row
                    ExcelHelper.MergeCells(worksheet, rowNo, columnNo, rowNo, columnNo + 1);
                    worksheet.Cells[rowNo, columnNo].Value = "District " + districtCountyObject.District.ToString() + " Total";

                    //add total
                }
            }

            //add state total
            rowNo++; columnNo = 1; 

            //Add Summary Row
            ExcelHelper.MergeCells(worksheet, rowNo, columnNo, rowNo, columnNo + 1);
            worksheet.Cells[rowNo, columnNo].Value = "State Total";

            currentCell.Row = rowNo; currentCell.Column = columnNo;
        }


        private void FillBudgetPercentageByCountyInExcel(ExcelWorksheet worksheet, SimulationOutput reportOutputData, List<int> simulationYears, Simulation simulation)
        {
            //Build Budget By County Headers
            var headers = GetHeaders(simulationYears);

            var rowNo = currentCell.Row + 1; var columnNo = 3;
            worksheet.Cells[rowNo, columnNo].Value = "% of Overall Dollars Recommended on Treatments by County";
            ExcelHelper.MergeCells(worksheet, rowNo, columnNo, rowNo, columnNo + (simulationYears.Count - 1));

            //build header cells
            rowNo++; columnNo = 0;
            for (int column = 0; column < headers.Count; column++)
            {
                columnNo = column + 1;
                worksheet.Cells[rowNo, columnNo].Value = headers[column];
            }

            ////build yearly budget list
            //foreach (var sectionData in reportOutputData.Years)
            //{
            //    var year = sectionData.Year;

            //    //get cost
            //    foreach (var section in sectionData.Assets)
            //    {
            //        var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
            //    }
            //}

            //fill data in cells
            currentCell = new CurrentCell { Row = rowNo, Column = columnNo }; var currentRowIndex = 0;
            foreach (var districtCountyObject in _uniqueDistrictCountyList)
            {
                rowNo++; columnNo = 1; currentRowIndex++;
                worksheet.Cells[rowNo, columnNo++].Value = districtCountyObject.District;
                worksheet.Cells[rowNo, columnNo++].Value = districtCountyObject.County;

                var rowsPerDistrict = _uniqueDistrictCountyList.Where(w => w.District == districtCountyObject.District).Count();
                if (currentRowIndex > rowsPerDistrict - 1)
                {
                    //reset index
                    rowNo++; columnNo = 1; currentRowIndex = 0;

                    //Add Summary Row
                    ExcelHelper.MergeCells(worksheet, rowNo, columnNo, rowNo, columnNo + 1);
                    worksheet.Cells[rowNo, columnNo].Value = "District " + districtCountyObject.District.ToString() + " Total";

                    //add total
                }
            }
            currentCell.Row = rowNo; currentCell.Column = columnNo;


        }

        #endregion Private methods
    }
}
