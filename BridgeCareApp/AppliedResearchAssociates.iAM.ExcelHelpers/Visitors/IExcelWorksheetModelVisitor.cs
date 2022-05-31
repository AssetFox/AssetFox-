<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Visitors/IExcelWorksheetModelVisitor.cs
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

﻿namespace AppliedResearchAssociates.iAM.ExcelHelpers.Visitors
========
﻿namespace BridgeCareCore.Helpers.Excel.Visitors
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/IExcelWorksheetModelVisitor.cs
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Visitors/IExcelWorksheetModelVisitor.cs
        TOutput Visit(SpecifiedColumnWidthExcelWorksheetModel specifiedColumnWidthExcelWorksheetModel, THelper helper);
========
        TOutput Visit(SpecifiedColumnWidthExcelWorksheetModel model, THelper helper);
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/IExcelWorksheetModelVisitor.cs
    }
}
