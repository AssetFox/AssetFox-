using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CalculatedAttributeRepository : ICalculatedAttributesRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistanceWork;

        public CalculatedAttributeRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfDataPersistanceWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void ClearCalculatedAttributes(Guid scenarioId) => throw new NotImplementedException();
        public void DeleteCalculatedAttributeFromLibrary(Guid libraryId, Guid calculatedAttributeId) => throw new NotImplementedException();
        public void DeleteCalculatedAttributeFromScenario(Guid scenarioId, Guid calculatedAttributeId) => throw new NotImplementedException();
        public void DeleteCalculatedAttributeLibrary(Guid libraryId) => throw new NotImplementedException();
        public ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries() => throw new NotImplementedException();
        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) => throw new NotImplementedException();
        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library) => throw new NotImplementedException();
        public void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId) => throw new NotImplementedException();
        public void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId) => throw new NotImplementedException();
    }
}
