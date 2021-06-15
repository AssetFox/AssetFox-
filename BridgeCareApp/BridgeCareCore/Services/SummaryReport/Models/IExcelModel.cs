using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public interface IExcelModel
    {
        T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper);
    }
}
