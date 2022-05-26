using OfficeOpenXml.Style;
﻿using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
