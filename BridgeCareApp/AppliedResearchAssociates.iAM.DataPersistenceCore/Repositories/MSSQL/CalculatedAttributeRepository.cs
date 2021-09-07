using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;

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

        public void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId)
        {
            var attributeList = _unitOfDataPersistanceWork.Context.Attribute;
            foreach (var calculatedAttribute in calculatedAttributes)
            {
                var attributeObject = attributeList.FirstOrDefault(_ => _.Name == calculatedAttribute.Attribute);
                if (attributeObject != null)
                {
                    _unitOfDataPersistanceWork.Context.Upsert(calculatedAttribute.ToLibraryEntity(attributeObject),
                        libraryId, _unitOfDataPersistanceWork.UserEntity?.Id);
                }
            }
        }

        public void DeleteCalculatedAttributeLibrary(Guid libraryId) =>
            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeLibraryEntity>(_ => _.Id == libraryId);

        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) =>
            _unitOfDataPersistanceWork.Context.ScenarioCalculatedAttribute
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId)
        {
            var attributeList = _unitOfDataPersistanceWork.Context.Attribute;

            // This will throw an error if no simulation is found.  That is the desired action here - let the API worry about the existence of the simulation
            var networkId = _unitOfDataPersistanceWork.NetworkRepo.GetPennDotNetwork().Id;
            var scenario = _unitOfDataPersistanceWork.SimulationRepo.GetSimulation(scenarioId).ToEntity(networkId);

            foreach (var calculatedAttribute in calculatedAttributes)
            {
                var attributeObject = attributeList.FirstOrDefault(_ => _.Name == calculatedAttribute.Attribute);
                if (attributeObject != null)
                {
                    _unitOfDataPersistanceWork.Context.Upsert(calculatedAttribute.ToScenarioEntity(scenario, attributeObject),
                        scenarioId, _unitOfDataPersistanceWork.UserEntity?.Id);
                }
            }
        }

        private void ClearCalculatedAttributes(Guid scenarioId) =>
            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCalculatedAttributeEntity>(_ => _.SimulationId == scenarioId);

        private void DeleteCalculatedAttributeFromLibrary(Guid libraryId, Guid calculatedAttributeId) =>
            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeEntity>(_ => _.CalculatedAttributeLibraryId == libraryId && _.Id == calculatedAttributeId);


    }
}
