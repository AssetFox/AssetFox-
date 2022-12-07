﻿using System;
using System.Collections.Generic;
using System.Linq;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsRegions
    {
        public static List<int> NumberedDistricts
            => new List<int> { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 }; // Seven is indeed skipped.

        public class TableWithTotalModel
        {
            public RowBasedExcelRegionModel rowBasedExcelRegionModel { get; set; }
            public int startRowDelta;
            public int totalRowDelta;
        }

        public static TableWithTotalModel DistrictSubtable(
            SimulationOutput simulationOutput,
            int districtNumber,
            Func<SimulationOutput, int, List<ExcelRowModel>> districtCountyFunction,
            int initialDelta)
        {
            var tableRows = new List<ExcelRowModel>();

            // Subheader for District
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, $"District: {districtNumber}", simulationOutput.Years.Count + 2, 1),
            };

            var districtCountyRows = districtCountyFunction(simulationOutput, districtNumber);

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TableBottomSumRow(simulationOutput, districtNumber, districtCountyRows.Count)
            };

            tableRows.AddRange(headerRows);
            int startDelta = initialDelta + headerRows.Count;

            tableRows.AddRange(districtCountyRows);

            int totalDelta = startDelta + tableRows.Count;
            tableRows.AddRange(bottomRows);

            return new TableWithTotalModel
            {
                rowBasedExcelRegionModel = RowBasedExcelRegionModels.WithRows(tableRows),
                startRowDelta = startDelta,
                totalRowDelta = totalDelta
            };                
        }


        public static RowBasedExcelRegionModel MpmsTable(SimulationOutput simulationOutput, ref int initialRowIndex)
        {
            // MPMS Projects By County global section header
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Dollars Spent on MPMS Projects by County", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };
            int tableRowStartIndex = initialRowIndex + headerRows.Count;

            var districtSubTables = new List<TableWithTotalModel>();
            foreach (var district in NumberedDistricts)
            {
                var districtSubTable = DistrictSubtable(simulationOutput, district, DistrictTotalsRowModels.MpmsTableDistrict, tableRowStartIndex);
                districtSubTables.Add(districtSubTable);
                tableRowStartIndex = districtSubTable.totalRowDelta;
            }

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.MpmsTableTurnpike(simulationOutput),
                DistrictTotalsRowModels.TableStateTotal(simulationOutput, districtSubTables.Select(_ => _.totalRowDelta).ToList())
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables.Select(_ => _.rowBasedExcelRegionModel).ToList()),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            initialRowIndex += tableModels.Rows.Count + 1;

            return tableModels;
        }


        public static RowBasedExcelRegionModel BamsTable(SimulationOutput simulationOutput, ref int initialRowIndex)
        {
            // BAMS Projects By County global section header
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Dollars Spent on BAMS Projects by County", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };
            int tableRowStartIndex = initialRowIndex + headerRows.Count;

            var districtSubTables = new List<TableWithTotalModel>();
            foreach (var district in NumberedDistricts)
            {
                var districtSubTable = DistrictSubtable(simulationOutput, district, DistrictTotalsRowModels.BamsTableDistrict, tableRowStartIndex);
                districtSubTables.Add(districtSubTable);
                tableRowStartIndex = districtSubTable.totalRowDelta;
            }

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.BamsTableTurnpike(simulationOutput),
                DistrictTotalsRowModels.TableStateTotal(simulationOutput, districtSubTables.Select(_ => _.totalRowDelta).ToList())
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables.Select(_ => _.rowBasedExcelRegionModel).ToList()),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            initialRowIndex += tableModels.Rows.Count + 1;

            return tableModels;
        }

        public static RowBasedExcelRegionModel OverallDollarsTable(SimulationOutput simulationOutput, ref int initialRowIndex)
        {
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "Overall Dollars Spent on Projects by District", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput),
            };
            int tableRowStartIndex = initialRowIndex + headerRows.Count;

            var districtSubTables = new List<TableWithTotalModel>();
            foreach (var district in NumberedDistricts)
            {
                var districtSubTable = DistrictSubtable(simulationOutput, district, DistrictTotalsRowModels.TotalsTableDistrict, tableRowStartIndex);
                districtSubTables.Add(districtSubTable);
                tableRowStartIndex = districtSubTable.totalRowDelta;
            }

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.TotalsTableTurnpike(simulationOutput),
                DistrictTotalsRowModels.TableStateTotal(simulationOutput, districtSubTables.Select(_ => _.totalRowDelta).ToList())
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.Concat(districtSubTables.Select(_ => _.rowBasedExcelRegionModel).ToList()),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            initialRowIndex += tableModels.Rows.Count + 1;

            return tableModels;
        }

        internal static RowBasedExcelRegionModel PercentOverallDollarsTable(SimulationOutput simulationOutput, ref int initialRowIndex)
        {
            var headerRows = new List<ExcelRowModel>
            {
                ExcelRowModels.CenteredHeader(0, "% of Overall Dollars Spent on Projects by District", simulationOutput.Years.Count + 2, 1),
                DistrictTotalsRowModels.DistrictCountyAndYearsHeaders(simulationOutput)
            };

            var districtSubTables = new List<ExcelRowModel>();
            int tableRowStartIndex = initialRowIndex + headerRows.Count;

            foreach (var district in NumberedDistricts)
            {
                // Subheader for District
                var subHeaderRows = new List<ExcelRowModel>
                {
                    ExcelRowModels.CenteredHeader(0, $"District: {district}", simulationOutput.Years.Count + 2, 1),
                };
                districtSubTables.AddRange(subHeaderRows);

                var stateTotalsRowOffset = tableRowStartIndex - initialRowIndex + 3;

                var districtSubTable = DistrictTotalsRowModels.PercentOverallDollarsDistrictSubtable(simulationOutput, district, stateTotalsRowOffset);
                districtSubTables.AddRange(districtSubTable);

                tableRowStartIndex += districtSubTable.Count + 1; // account for header and total lines
            }

            var bottomRows = new List<ExcelRowModel>
            {
                DistrictTotalsRowModels.PercentOverallDollarsTurnpike(simulationOutput, tableRowStartIndex - initialRowIndex + 2)
            };

            var tableModels = RowBasedExcelRegionModels.Concat(
                RowBasedExcelRegionModels.WithRows(headerRows),
                RowBasedExcelRegionModels.WithRows(districtSubTables),
                RowBasedExcelRegionModels.WithRows(bottomRows)
            );

            return tableModels;
        }

    }
}