using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Visitors;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.ShortNameGlossary
{
    public class SummaryReportGlossary
    {
        private readonly IExcelHelper _excelHelper;

        public SummaryReportGlossary(IExcelHelper excelHelper)
        {
            _excelHelper = excelHelper;
        }

        public void FillModelBasedContent(ExcelWorksheet worksheet)
        {
            var regionModel = new RowBasedExcelWorksheetModel
            {
                Region = ShortNameGlossaryColumns.GlossaryColumn(),
                StartRow = 1,
                StartColumn = 8,
            };
            var writer = new ExcelWorksheetWriter();
            regionModel.Accept(writer, worksheet);
        }

        public void Fill(ExcelWorksheet worksheet)
        {
            FillBridgeCareWorkType(worksheet);
            FillModelBasedContent(worksheet);
        }

        public void FillBridgeCareWorkType(ExcelWorksheet worksheet)
        {
            
            var initialRow = 1;
            worksheet.Cells["A1"].Value = "Bridge Care Work Type";
            worksheet.Cells["B1"].Value = "Short Bridge Care Work Type";
            _excelHelper.ApplyStyle(worksheet.Cells["A1:B1"]);

            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();
            var row = 2;
            var column = 1;

            foreach (var treatment in abbreviatedTreatmentNames)
            {
                worksheet.Cells[row, column++].Value = treatment.Key;
                worksheet.Cells[row, column].Value = treatment.Value;
                column = 1;
                row++;
            }
            _excelHelper.ApplyBorder(worksheet.Cells[initialRow, 1, row, 2]);
            row += 2;

            worksheet.Cells[row, 1].Value = "Color Key";
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row += 2;

            worksheet.Cells[row, 1].Value = "Work Done Columns";
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - Project came from BAMS";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - project is being cash flowed";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 0, 0));
            row++;

            worksheet.Cells[row, 1].Value = "Bridge being worked on has a parallel bridge - project came from MPMS";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 204, 255));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);
            row++;

            worksheet.Cells[row, 1].Value = "Bridge project is being cashed flowed";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 255, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Red);
            row++;

            worksheet.Cells[row, 1].Value = "MPMS Project selected for consecutive years";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 153, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);
            row += 2;

            worksheet.Cells[row, 1].Value = "Details Colums";
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            row++;

            worksheet.Cells[row, 1].Value = "Project is being cash flowed";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(0, 255, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "P3 Bridge where minimum condition is less than 5";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(255, 255, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.Black);
            row++;

            worksheet.Cells[row, 1].Value = "Min Condition is less than or equal to 3.5";
            _excelHelper.ApplyBorder(worksheet.Cells[row, 1, row, 2]);
            _excelHelper.MergeCells(worksheet, row, 1, row, 2);
            _excelHelper.ApplyColor(worksheet.Cells[row, 1, row, 2], Color.FromArgb(112, 48, 160));
            _excelHelper.SetTextColor(worksheet.Cells[row, 1, row, 2], Color.White);

            row += 3;

            worksheet.Cells[row, 1].Value = "Example: ";

            worksheet.Cells[row, 2].Value = "2021";
            worksheet.Cells[row, 3].Value = "2022";
            worksheet.Cells[row, 4].Value = "2023";
            row++;

            worksheet.Cells[row, 2].Value = "Brdg_Repl";
            _excelHelper.ApplyColor(worksheet.Cells[row, 2], Color.FromArgb(0, 204, 255));
            _excelHelper.SetTextColor(worksheet.Cells[row, 2], Color.FromArgb(255, 0, 0));

            worksheet.Cells[row, 3].Value = "--";
            _excelHelper.ApplyColor(worksheet.Cells[row, 3], Color.FromArgb(0, 255, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 3], Color.Red);

            worksheet.Cells[row, 4].Value = "--";
            _excelHelper.ApplyColor(worksheet.Cells[row, 4], Color.FromArgb(0, 255, 0));
            _excelHelper.SetTextColor(worksheet.Cells[row, 4], Color.Red);
            row++;
            _excelHelper.ApplyBorder(worksheet.Cells[row - 1, 2, row - 1, 4]);

            worksheet.Cells.AutoFitColumns(70);
            worksheet.Cells[row, 2].Value = "(Bridge being replaced also has a parallel bridge.  Bridge replacement is cash flowed over 3 years.)";
        }
    }
}
