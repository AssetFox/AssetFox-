
﻿using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelNothingModel: IExcelModel
    {
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
