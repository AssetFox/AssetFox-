
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public static class RelativeExcelRangeModels
    {
        public static RelativeExcelRangeModel OneByOne(IExcelModel content)
            => new RelativeExcelRangeModel
            {
                Content = content,
                Size = new ExcelRangeSize(1, 1),
            };

        public static RelativeExcelRangeModel Empty(int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = ExcelValueModels.Nothing,
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel Text(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = ExcelValueModels.String(text),
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel BoldText(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = (IExcelModel)StackedExcelModels.BoldText(text),
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel BoldCenteredText(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = StackedExcelModels.Stacked(
                    ExcelValueModels.String(text),
                    ExcelStyleModels.CenteredHeader,
                    ExcelStyleModels.Bold,
                    ExcelStyleModels.MediumBorder
                ),
                Size = new ExcelRangeSize(width, height),
            };

        public static RelativeExcelRangeModel BoldRightText(string text, int width = 1, int height = 1)
            => new RelativeExcelRangeModel
            {
                Content = StackedExcelModels.Stacked(
                    ExcelValueModels.String(text),
                    ExcelStyleModels.RightHeader,
                    ExcelStyleModels.Bold,
                    ExcelStyleModels.MediumBorder
                ),
                Size = new ExcelRangeSize(width, height),
            };


    }
}
