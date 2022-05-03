<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/Worksheets/AnchoredExcelRegionModel.cs
﻿
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;

using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Regions;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets
========
﻿using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/Worksheets/AnchoredExcelRegionModel.cs
{
    /// <summary>A region model, together with its location in the worksheet.</summary>
    public class AnchoredExcelRegionModel: IExcelWorksheetContentModel
    {
        public RowBasedExcelRegionModel Region { get; set; }
        public int StartRow { get; set; } = 1;
        public int StartColumn { get; set; } = 1;
        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
