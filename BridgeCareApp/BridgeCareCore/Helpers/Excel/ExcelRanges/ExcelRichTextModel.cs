<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelRanges/ExcelRichTextModel.cs
﻿using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelRanges
========
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelRanges/ExcelRichTextModel.cs
{
    public class ExcelRichTextModel: IExcelModel
    {
        public string Text { get; set; }
        public bool Bold { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
