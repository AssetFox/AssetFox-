using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

﻿namespace AppliedResearchAssociates.iAM.ExcelHelpers.Visitors
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
        TOutput Visit(SpecifiedColumnWidthExcelWorksheetModel specifiedColumnWidthExcelWorksheetModel, THelper helper);
    }
}
