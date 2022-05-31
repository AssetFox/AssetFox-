<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Visitors/IExcelModelVisitor.cs
﻿namespace AppliedResearchAssociates.iAM.ExcelHelpers.Visitors
========
﻿namespace BridgeCareCore.Helpers.Excel.Visitors
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/IExcelModelVisitor.cs
{
    public interface IExcelModelVisitor<THelper, TOutput>
    {
        TOutput Visit(ExcelStringValueModel model, THelper helper);
        TOutput Visit(ExcelMoneyValueModel model, THelper helper);
        TOutput Visit(ExcelFormulaModel model, THelper helper);
        TOutput Visit(ExcelBoldModel excelBoldModel, THelper helper);
        TOutput Visit(ExcelSingleBorderModel excelSingleBorderModel, THelper helper);
        TOutput Visit(ExcelItalicModel excelItalicModel, THelper helper);
        TOutput Visit(ExcelIntegerValueModel model, THelper helper);
        TOutput Visit(ExcelNothingModel nothing, THelper helper);
        TOutput Visit(StackedExcelModel model, THelper helper);
        TOutput Visit(ExcelBorderModel model, THelper helper);
        TOutput Visit(ExcelHorizontalAlignmentModel model, THelper helper);
        TOutput Visit(ExcelVerticalAlignmentModel model, THelper helper);
        TOutput Visit(ExcelFillModel model, THelper helper);
        TOutput Visit(ExcelFontColorModel model, THelper helper);
        TOutput Visit(ExcelFontSizeModel model, THelper helper);
        TOutput Visit(ExcelNumberFormatModel model, THelper helper);
        TOutput Visit(ExcelRichTextModel model, THelper helper);
        TOutput Visit(ExcelDecimalValueModel model, THelper helper);

        TOutput Visit(ExcelWrapTextModel model, THelper helper);

        TOutput Visit(ExcelFontSizeModel model, THelper helper);
        TOutput Visit(ExcelSingleBorderModel model, THelper helper);
    }
}
