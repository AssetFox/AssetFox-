using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CalculatedAttributeRepository : ICalculatedAttributesRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistanceWork;

        public CalculatedAttributeRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfDataPersistanceWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries() =>
            _unitOfDataPersistanceWork.Context.CalculatedAttributeLibrary
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library) =>
            _unitOfDataPersistanceWork.Context.Upsert(library.ToLibraryEntity(_unitOfDataPersistanceWork.Context.Attribute),
                library.Id, _unitOfDataPersistanceWork.UserEntity?.Id);

        public void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId) => throw new NotImplementedException();

        public void DeleteCalculatedAttributeFromLibrary(Guid libraryId, Guid calculatedAttributeId) => throw new NotImplementedException();

        public void DeleteCalculatedAttributeLibrary(Guid libraryId) => throw new NotImplementedException();

        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) =>
            _unitOfDataPersistanceWork.Context.ScenarioCalculatedAttribute
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId) => throw new NotImplementedException();

        public void DeleteCalculatedAttributeFromScenario(Guid scenarioId, Guid calculatedAttributeId) => throw new NotImplementedException();

        public void ClearCalculatedAttributes(Guid scenarioId) => throw new NotImplementedException();
        
        
        
        
    }
}
