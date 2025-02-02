﻿using AppliedResearchAssociates.iAM.Data.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelWriter : IExcelModelVisitor<ExcelRange, Unit>
    {
        public Unit Visit(ExcelStringValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
        public Unit Visit(ExcelMoneyValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }
        public Unit Visit(ExcelFormulaModel model, ExcelRange cells)
        {
            var formula = model.Formula(cells);
            cells.Formula = formula;
            return Unit.Default;
        }
        public Unit Visit(ExcelIntegerValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }

        public Unit Visit(ExcelNothingModel nothing, ExcelRange cells)
            => Unit.Default;

        public Unit Visit(ExcelBoldModel bold, ExcelRange cells)
        {
            cells.Style.Font.Bold = bold.Bold;
            return Unit.Default;
        }


        public Unit Visit(ExcelItalicModel italic, ExcelRange cells)
        {
            cells.Style.Font.Italic = italic.Italic;
            return Unit.Default;
        }

        public Unit Visit(StackedExcelModel model, ExcelRange cells)
        {
            foreach (var child in model.Content)
            {
                child.Accept(this, cells);
            }
            return Unit.Default;
        }

        public Unit Visit(ExcelBorderModel model, ExcelRange cells)
        {
            cells.Style.Border.Top.Style = model.BorderStyle;
            cells.Style.Border.Right.Style = model.BorderStyle;
            cells.Style.Border.Bottom.Style = model.BorderStyle;
            cells.Style.Border.Left.Style = model.BorderStyle;
            return Unit.Default;
        }
        public Unit Visit(ExcelSingleBorderModel model, ExcelRange cells)
        {
            switch(model.Edge)
            {
            case RectangleEdge.Top:
                cells.Style.Border.Top.Style = model.BorderStyle;
                break;
            case RectangleEdge.Left:
                cells.Style.Border.Left.Style = model.BorderStyle;
                break;
            case RectangleEdge.Right:
                cells.Style.Border.Right.Style = model.BorderStyle;
                break;
            case RectangleEdge.Bottom:
                cells.Style.Border.Bottom.Style = model.BorderStyle;
                break;
            }
            return Unit.Default;
        }

        public Unit Visit(ExcelHorizontalAlignmentModel model, ExcelRange cells)
        {
            cells.Style.HorizontalAlignment = model.Alignment;
            return Unit.Default;
        }

        public Unit Visit(ExcelVerticalAlignmentModel model, ExcelRange cells)
        {
            cells.Style.VerticalAlignment = model.Alignment;
            return Unit.Default;
        }
        public Unit Visit(ExcelFillModel model, ExcelRange cells)
        {
            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cells.Style.Fill.BackgroundColor.SetColor(model.Color);
            return Unit.Default;
        }

        public Unit Visit(ExcelFontColorModel model, ExcelRange cells)
        {
            cells.Style.Font.Color.SetColor(model.Color);
            return Unit.Default;
        }

        public Unit Visit(ExcelFontSizeModel model, ExcelRange cells)
        {
            cells.Style.Font.Size = model.FontSize;
            return Unit.Default;
        }

        public Unit Visit(ExcelNumberFormatModel model, ExcelRange cells)
        {
            cells.Style.Numberformat.Format = model.Format;
            return Unit.Default;
        }

        public Unit Visit(ExcelRichTextModel model, ExcelRange cells)
        {
            var richText = cells.RichText.Add(model.Text);
            richText.Bold = model.Bold;
            if (model.FontSize.HasValue)
            {
                richText.Size = model.FontSize.Value;
            }
            return Unit.Default;
        }

        public Unit Visit(ExcelDecimalValueModel model, ExcelRange cells)
        {
            cells.Value = model.Value;
            return Unit.Default;
        }

        public Unit Visit(ExcelWrapTextModel model, ExcelRange cells)
        {
            cells.Style.WrapText = model.Wrap;
            return Unit.Default;
        }
    }
}
