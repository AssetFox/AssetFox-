using System;
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

        DateTime GetLibraryModifiedDate(Guid calculatedLibraryId);

        List<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibrariesNoChildren();

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByLibraryIdNoChildren(Guid libraryid);

        void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library);

        void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId);

        void DeleteCalculatedAttributeLibrary(Guid libraryId);

        CalculatedAttributeDTO GetScenarioCalulatedAttributesByScenarioAndAttributeId(Guid scenarioId, Guid attributeId);
        // Scenarios
        ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId);

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByScenarioIdNoChildren(Guid scenarioId);

        void UpsertScenarioCalculatedAttributesNonAtomic(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid simulationId);

        void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid simulationId);

        void PopulateScenarioCalculatedFields(Simulation simulation);

        CalculatedAttributeDTO GetLibraryCalulatedAttributesByLibraryAndAttributeId(Guid libraryId, Guid attributeId);

        List<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibrariesNoChildrenAccessibleToUser(Guid userId);

        LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId);

        void UpsertOrDeleteUsers(Guid calculatedAttribueLibraryId, IList<LibraryUserDTO> libraryUsers);

        List<LibraryUserDTO> GetLibraryUsers(Guid calculatedAttributeLibraryId);
    }
}
