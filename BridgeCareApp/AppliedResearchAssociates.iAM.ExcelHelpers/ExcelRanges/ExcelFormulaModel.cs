using System;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelFormulaModel: IExcelModel
    {
        public Func<ExcelRange, string> Formula { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
