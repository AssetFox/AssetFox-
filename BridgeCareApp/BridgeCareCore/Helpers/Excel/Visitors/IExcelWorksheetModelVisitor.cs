<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Interfaces/BAMSSummaryReport/Visitors/IExcelWorksheetModelVisitor.cs
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors
========
﻿namespace BridgeCareCore.Helpers.Excel.Visitors
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/IExcelWorksheetModelVisitor.cs
{
    public interface IExcelWorksheetModelVisitor<THelper, TOutput>
    {
        TOutput Visit(AnchoredExcelRegionModel model, THelper helper);
        TOutput Visit(AutoFitColumnsExcelWorksheetContentModel model, THelper helper);
        TOutput Visit(SpecificColumnWidthChangeExcelWorksheetModel model, THelper helper);
    }
}
