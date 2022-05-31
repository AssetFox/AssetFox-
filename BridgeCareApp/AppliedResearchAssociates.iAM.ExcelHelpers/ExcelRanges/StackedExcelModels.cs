using System.Linq;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelRanges/StackedExcelModels.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/StackedExcelModels.cs
{
    /// <summary>
    /// All of the models will get applied in the same place. If two conflict,
    /// the later one wins.
    /// </summary>
    public static class StackedExcelModels
    {
        public static StackedExcelModel Stacked(params IExcelModel[] stack)
            => new StackedExcelModel
            {
                Content = stack.ToList(),
            };

        public static StackedExcelModel Empty => Stacked();

        public static StackedExcelModel BoldText(string text)
            => Stacked(
                ExcelValueModels.String(text),
                ExcelStyleModels.Bold);

        public static StackedExcelModel LeftHeader(string text)
            => Stacked(
                ExcelValueModels.String(text),
                ExcelStyleModels.LeftHeader);

        public static StackedExcelModel LeftHeaderWrap(string text)
            => Stacked(
                ExcelValueModels.String(text),
                ExcelStyleModels.LeftHeaderWrap);

        public static StackedExcelModel CenteredHeader(string text)
            => Stacked(
                ExcelValueModels.String(text),
                ExcelStyleModels.CenteredHeader);
    }
}
