using System;
using System.Collections.Generic;
using System.Linq;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsRegions
    {
        public static RowBasedExcelRegionModel DistrictSubtable(List<DistrictTotalsRowModels.CountyRow> districtCountyValueRows)
        {
            var districtNumber = districtCountyValueRows.First().District;
            var yearColumnCount = districtCountyValueRows.First().rowEntries.Count;
            
            // Subheader for District
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, $"District: {districtNumber}", yearColumnCount + 2, 1),
            };

            var districtCountyRows = DistrictTotalsRowModels.DistrictCountyRowsToExcelRowModels(districtCountyValueRows);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TableSumRowModel(districtCountyValueRows)
            };

            var tableRows =
                headerRows
                .Concat(districtCountyRows)
                .Concat(bottomRows)
                .ToList();

            return RowBasedExcelRegionModels.WithRows(tableRows);
        }

        public static RowBasedExcelRegionModel PercentageDistrictSubtable(List<DistrictTotalsRowModels.CountyRow> districtCountyValueRows)
        {
            var districtNumber = districtCountyValueRows.First().District;
            var yearColumnCount = districtCountyValueRows.First().rowEntries.Count;

            // Subheader for District
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, $"District: {districtNumber}", yearColumnCount + 2, 1),
            };

            var districtCountyRows = DistrictTotalsRowModels.DistrictCountyRowsToPercentageExcelRowModels(districtCountyValueRows);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TableSumRowPercentageModel(districtCountyValueRows)
            };

            var tableRows =
                headerRows
                .Concat(districtCountyRows)
                .Concat(bottomRows)
                .ToList();

            return RowBasedExcelRegionModels.WithRows(tableRows);
        }


        public static RowBasedExcelRegionModel MpmsTable(SimulationOutput simulationOutput, List<int> districtList)
        {
            // MPMS Projects By County global section header
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Dollars Spent on MPMS Projects by County", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };

            var allRows = new List<DistrictTotalsRowModels.CountyRow>();

            var districtSubTables = new List<RowBasedExcelRegionModel>();

            foreach (var district in districtList)
            {
                var rows = DistrictTotalsRowModels.MpmsTableDistrictValues(simulationOutput, district);
                allRows.AddRange(rows);

                var districtSubTable = DistrictSubtable(rows);
                districtSubTables.Add(districtSubTable);
            }

            var turnpikeRowValues = DistrictTotalsRowModels.MpmsTurnpikeRowValue(simulationOutput);
            allRows.Add(turnpikeRowValues);
            var stateTotalValues = DistrictTotalsRowModels.TableSumRowValue(allRows);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TurnpikeRowToExcelRowModel(turnpikeRowValues),
                DistrictTotalsRowModels.TableStateTotal(stateTotalValues)
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            return tableModels;
        }


        public static RowBasedExcelRegionModel BamsTable(SimulationOutput simulationOutput, List<int> districtList)
        {
            // BAMS Projects By County global section header
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Dollars Spent on BAMS Projects by County", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };

            var allRows = new List<DistrictTotalsRowModels.CountyRow>();

            var districtSubTables = new List<RowBasedExcelRegionModel>();

            foreach (var district in districtList)
            {
                var rows = DistrictTotalsRowModels.BamsTableDistrictValues(simulationOutput, district);
                allRows.AddRange(rows);

                var districtSubTable = DistrictSubtable(rows);
                districtSubTables.Add(districtSubTable);
            }

            var turnpikeRowValues = DistrictTotalsRowModels.BamsTurnpikeRowValue(simulationOutput);
            allRows.Add(turnpikeRowValues);
            var stateTotalValues = DistrictTotalsRowModels.TableSumRowValue(allRows);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TurnpikeRowToExcelRowModel(turnpikeRowValues),
                DistrictTotalsRowModels.TableStateTotal(stateTotalValues)
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            return tableModels;
        }

        public static RowBasedExcelRegionModel OverallDollarsTable(SimulationOutput simulationOutput, List<int> districtList)
        {
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Overall Dollars Spent on Projects by District", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput),
            };

            var allRows = new List<DistrictTotalsRowModels.CountyRow>();

            var districtSubTables = new List<RowBasedExcelRegionModel>();

            foreach (var district in districtList)
            {
                var rows = DistrictTotalsRowModels.OverallDollarsTableDistrictValues(simulationOutput, district);
                allRows.AddRange(rows);

                var districtSubTable = DistrictSubtable(rows);
                districtSubTables.Add(districtSubTable);
            }

            var turnpikeRowValues = DistrictTotalsRowModels.OverallDollarsTableTurnpike(simulationOutput);
            allRows.Add(turnpikeRowValues);
            var stateTotalValues = DistrictTotalsRowModels.TableSumRowValue(allRows);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TurnpikeRowToExcelRowModel(turnpikeRowValues),
                DistrictTotalsRowModels.TableStateTotal(stateTotalValues)
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            return tableModels;
        }


        internal static RowBasedExcelRegionModel PercentageOverallDollarsTable(SimulationOutput simulationOutput, List<int> districtList)
        { 
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "% of Overall Dollars Spent on Projects by District", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };

            var allRows = DistrictTotalsRowModels.PercentageOverallDollarsTableValues(simulationOutput, districtList);

            var districtSubTables = new List<RowBasedExcelRegionModel>();
            foreach (var district in districtList)
            {
                var districtPercentageRows = allRows.Where(countyRow => countyRow.District == district).ToList();
                var districtSubTable = PercentageDistrictSubtable(districtPercentageRows);
                districtSubTables.Add(districtSubTable);
            }

            var percentageTurnpikeRow = allRows.Single(countyRow => countyRow.County.ToUpper() == "TURNPIKE");
            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TurnpikeRowToPercentageExcelRowModel(percentageTurnpikeRow)
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            return tableModels;
        }

    }
}
