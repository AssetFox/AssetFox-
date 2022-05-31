using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Regions/RowBasedExcelRegionModel.cs
using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
using BridgeCareCore.Helpers.Excel.Visitors;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Regions/RowBasedExcelRegionModel.cs
{
    public class RowBasedExcelRegionModel
    {
        public List<ExcelRowModel> Rows { get; set; }
    }
}
