using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.ExcelHelpers.Visitors
{
    public static class ExcelWorksheetAdder
    {
        public static ExcelWorksheet AddWorksheet(ExcelWorkbook workbook, ExcelWorksheetModel model)
        {
            var writer = new ExcelWorksheetWriter();
            var r = workbook.Worksheets.Add(model.TabName);
            foreach (var content in model.Content)
            {
                content.Accept(writer, r);
            }
            r.Cells.Calculate();
            return r;
        }
    }
}
