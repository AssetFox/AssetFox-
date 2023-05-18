using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

public interface IAdminDataRepository
{
    IList<string> GetKeyFields();
    void SetKeyFields(string keyFields);

    void SetPrimaryNetwork(string name);
    string GetPrimaryNetwork();

    IList<string> GetSimulationReportNames();
}


