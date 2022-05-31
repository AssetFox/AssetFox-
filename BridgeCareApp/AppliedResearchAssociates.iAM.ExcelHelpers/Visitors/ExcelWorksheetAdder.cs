using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class ExcelWorksheetAdder
    {
        public static ExcelWorksheet AddWorksheet(ExcelWorkbook workbook, ExcelWorksheetModel model)
        {
            var writer = new ExcelWorksheetWriter();
            var returnValue = workbook.Worksheets.Add(model.TabName);
            foreach (var content in model.Content)
            {
                content.Accept(writer, returnValue);
            }
            returnValue.Cells.Calculate();
            return returnValue;
        }
    }
}
