using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Models.DefaultData;

namespace BridgeCareCore.Interfaces.DefaultData
{
    public interface IAnalysisDefaultDataService
    {
        Task<AnalysisDefaultData> GetAnalysisDefaultData();
    }
}
