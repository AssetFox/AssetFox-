<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Models/BAMSSummaryReport/ExcelStyles/ExcelBorderModel.cs
﻿using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Visitors;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.ExcelStyles
========
﻿using BridgeCareCore.Helpers.Excel.Visitors;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelStyles/ExcelBorderModel.cs
{
    public class ExcelBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
