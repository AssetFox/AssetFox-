﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISelectableTreatmentRepository
    {
        void CreateTreatmentLibrary(string name, Guid simulationId);

        void CreateSelectableTreatments(List<SelectableTreatment> selectableTreatments, Guid simulationId);

        void GetSimulationTreatments(Simulation simulation);

        Task<List<TreatmentLibraryDTO>> TreatmentLibrariesWithTreatments();

        void AddOrUpdateTreatmentLibrary(TreatmentLibraryDTO dto, Guid simulationId);

        void AddOrUpdateOrDeleteTreatments(List<TreatmentDTO> treatments, Guid libraryId);

        void DeleteTreatmentLibrary(Guid libraryId);
    }
}
