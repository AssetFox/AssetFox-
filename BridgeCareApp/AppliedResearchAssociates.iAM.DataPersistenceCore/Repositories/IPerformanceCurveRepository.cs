﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IPerformanceCurveRepository
    {
        void CreatePerformanceCurveLibrary(string name, Guid simulationId);

        void CreatePerformanceCurves(List<PerformanceCurve> performanceCurves, Guid simulationId);

        void SimulationPerformanceCurves(Simulation simulation);

        Task<List<PerformanceCurveLibraryDTO>> PerformanceCurveLibrariesWithPerformanceCurves();

        void AddOrUpdatePerformanceCurveLibrary(PerformanceCurveLibraryDTO dto, Guid simulationId);

        void AddOrUpdateOrDeletePerformanceCurves(List<PerformanceCurveDTO> performanceCurves, Guid libraryId);

        void DeletePerformanceCurveLibrary(Guid libraryId);
    }
}
