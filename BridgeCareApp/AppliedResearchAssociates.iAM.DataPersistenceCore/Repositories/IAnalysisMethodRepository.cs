﻿using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnalysisMethodRepository
    {
        void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId);

        void GetSimulationAnalysisMethod(Simulation simulation);

        AnalysisMethodDTO GetPermittedAnalysisMethod(Guid simulationId);

        AnalysisMethodDTO GetAnalysisMethod(Guid simulationId);

        void UpsertPermittedAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto);

        void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto);
    }
}
