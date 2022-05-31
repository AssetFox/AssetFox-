<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelRanges/ExcelIntegerValueModel.cs
﻿using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
﻿using BridgeCareCore.Helpers.Excel.Visitors;
using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/ExcelIntegerValueModel.cs
{
    public class ExcelIntegerValueModel: IExcelModel
    {
        public int Value { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
