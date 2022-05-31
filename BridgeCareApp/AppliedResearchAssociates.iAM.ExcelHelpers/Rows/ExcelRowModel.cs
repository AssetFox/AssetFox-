using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Rows/ExcelRowModel.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Rows/ExcelRowModel.cs
{
    public class ExcelRowModel
    {
        public List<RelativeExcelRangeModel> Values { get; set; }
        /// <summary>When writing a cell, the EveryCell models are written first.
        /// Therefore, style entries in EveryCell can be overridden by the Values.</summary>
        public IExcelModel EveryCell { get; set; } = StackedExcelModels.Empty;
    }
}
