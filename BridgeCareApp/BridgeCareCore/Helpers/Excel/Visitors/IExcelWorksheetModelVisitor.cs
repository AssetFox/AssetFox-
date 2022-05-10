namespace BridgeCareCore.Helpers.Excel.Visitors
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
        TOutput Visit(SpecifiedColumnWidthExcelWorksheetModel model, THelper helper);
    }
}
