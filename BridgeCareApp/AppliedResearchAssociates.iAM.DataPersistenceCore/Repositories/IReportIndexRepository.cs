﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IReportIndexRepository
    {
        bool Add(ReportIndexEntity report);
        ReportIndexDTO Get(Guid reportId);
        List<ReportIndexDTO> GetAllForScenario(Guid scenarioId);
        bool DeleteReport(Guid reportId);
        bool DeleteAllSimulationReports(Guid scenarioId);
        /// <summary>
        /// Deletes any ReportIndex with an ExpirationDate before the current date
        /// </summary>
        bool DeleteExpiredReports();
    }
}
