﻿using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISummaryReportGenerator
    {
        void GenerateExcelReport(SimulationModel simulationModel);
        byte[] DownloadExcelReport(SimulationModel simulationModel);

        byte[] DownloadTempJsonReport(SimulationModel simulationModel);
    }
}
