﻿using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs
{
    public class GraphData
    {

        public int Fill(ExcelWorksheet graphDataSheet, ExcelWorksheet pamsWorkSummaryWorksheet, int sourceStartRow, int destStartCol, int simulationYearsCount)
        {
            var sourceRow = sourceStartRow - 1;
            var sourceCol = 1;

            int destRow = 1;
            int destCol = destStartCol;

            // Summary title; could merge 1-5
            graphDataSheet.Cells[destRow, destCol].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
            destRow++;
            sourceRow++;

            // subtitle row = "Year", sourceRow + 3, sourceRow + 4, sourceRow + 5, sourceRow + 6
            graphDataSheet.Cells[destRow, destCol++].Value = "Year";
            sourceRow++;

            // subtitle rows Poor, Fair, Poor, Excellent
            graphDataSheet.Cells[destRow, destCol++].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
            sourceRow++;

            graphDataSheet.Cells[destRow, destCol++].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
            sourceRow++;

            graphDataSheet.Cells[destRow, destCol++].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";

            sourceRow++;
            graphDataSheet.Cells[destRow, destCol++].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";

            simulationYearsCount++; // add one for current year

            // Data rows
            for (var i = 1; i <= simulationYearsCount; i++)
            { 
                sourceRow = sourceStartRow; destRow++;
                sourceCol++;  destCol = destStartCol;

                // Year
                graphDataSheet.Cells[destRow, destCol++].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                sourceRow++;

                // Good, Fair, Poor
                graphDataSheet.Cells[destRow, destCol].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0%";
                sourceRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0%";
                sourceRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0%";
                sourceRow++;

                // Closed
                destRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"={pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol].Style.Numberformat.Format = @"#0%";

                // skip row
                destRow++;
            }

            destCol += 2;
            return destCol;
        }

    }
}
