using Antlr4.Runtime.Misc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.GraphTabs
{
    public class GraphData
    {

        public int Fill(ExcelWorksheet graphDataSheet, ExcelWorksheet pamsWorkSummaryWorksheet, int sourceStartRow, int destStartCol, int simulationYearsCount)
        {
            var sourceRow = sourceStartRow - 1; var sourceCol = 1;
            int destRow = 1; int destCol = destStartCol; var summaryRow = sourceRow - 1;

            // Summary title; could merge 1-5
            graphDataSheet.Cells[destRow, destCol].Formula = $"{pamsWorkSummaryWorksheet.Cells[summaryRow, sourceCol].FullAddress}";
            destRow++; 

            // subtitle row = "Year", sourceRow + 3, sourceRow + 4, sourceRow + 5, sourceRow + 6            
            graphDataSheet.Cells[destRow, destCol++].Value = "Year";


            // subtitle rows

            //Excellent
            sourceRow++;
            graphDataSheet.Cells[destRow, destCol++].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";

            //Good
            sourceRow++;
            graphDataSheet.Cells[destRow, destCol++].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";

            //Fair
            sourceRow++;
            graphDataSheet.Cells[destRow, destCol++].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";

            //Poor
            sourceRow++;
            graphDataSheet.Cells[destRow, destCol++].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                        

            simulationYearsCount++; // add one for current year
            sourceCol++;


            // Data rows
            var yearRow = sourceStartRow - 1;
            for (var i = 1; i < simulationYearsCount; i++)
            { 
                sourceRow = sourceStartRow; sourceCol++;
                destRow++; destCol = destStartCol;
                
                // Year
                graphDataSheet.Cells[destRow, destCol++].Formula = $"{pamsWorkSummaryWorksheet.Cells[yearRow, sourceCol].FullAddress}";

                // Excellent
                graphDataSheet.Cells[destRow, destCol].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0";

                //Good
                sourceRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0";


                //Fair
                sourceRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol++].Style.Numberformat.Format = @"#0";

                //Poor
                sourceRow++;
                graphDataSheet.Cells[destRow, destCol].Formula = $"{pamsWorkSummaryWorksheet.Cells[sourceRow, sourceCol].FullAddress}";
                graphDataSheet.Cells[destRow, destCol].Style.Numberformat.Format = @"#0";
            }

            destCol += 2;

            return destCol;
        }

    }
}
