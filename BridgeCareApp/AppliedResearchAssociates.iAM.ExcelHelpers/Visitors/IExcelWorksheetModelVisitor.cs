
namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
        TOutput Visit(SpecifiedColumnWidthExcelWorksheetModel specifiedColumnWidthExcelWorksheetModel, THelper helper);
    }
}
