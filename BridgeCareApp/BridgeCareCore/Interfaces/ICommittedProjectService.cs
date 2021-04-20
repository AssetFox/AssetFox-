using System;
using System.Collections.Generic;
using System.Net.Http;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        (string, byte[]) ExportCommittedProjectsFile(HttpRequest request, Guid simulationId);

        void ImportCommittedProjectFiles(HttpRequest request);

        void DeleteCommittedProjects(HttpRequest request, Guid simulationId);
    }
}
