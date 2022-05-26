
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class StackedExcelModel: IExcelModel
    {
        public List<IExcelModel> Content { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
