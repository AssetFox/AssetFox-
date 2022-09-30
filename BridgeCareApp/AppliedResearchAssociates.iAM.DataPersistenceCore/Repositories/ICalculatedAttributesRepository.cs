﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICalculatedAttributesRepository
    {
        // Libraries
        ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries();
        CalculatedAttributeLibraryDTO GetCalculatedAttributeLibraryByID(Guid id);

        List<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibrariesNoChildren();

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByLibraryIdNoChildren(Guid libraryid);

        void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library);

        void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId);

        void DeleteCalculatedAttributeLibrary(Guid libraryId);

        CalculatedAttributeDTO GetScenarioCalulatedAttributesByScenarioAndAttributeId(Guid scenarioId, Guid attributeId);
        // Scenarios
        ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId);

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByScenarioIdNoChildren(Guid scenarioId);

        void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid simulationId);

        void PopulateScenarioCalculatedFields(Simulation simulation);

        CalculatedAttributeDTO GetLibraryCalulatedAttributesByLibraryAndAttributeId(Guid libraryId, Guid attributeId);       
    }
}
