using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

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

        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library)
        {
            // Update the library
            _unitOfDataPersistanceWork.Context.Upsert(library.ToLibraryEntity(_unitOfDataPersistanceWork.Context.Attribute),
                library.Id, _unitOfDataPersistanceWork.UserEntity?.Id);

            // Delete the entities attached to the library that are no longer there
            var entityIds = library.CalculatedAttributes.Select(_ => _.Id).ToList();
            // This SHOULD cascade all deletes except for equations and criteria
            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeEntity>(_ => _.CalculatedAttributeLibraryId == library.Id && !entityIds.Contains(_.Id));
            // Deleteing all equations and criteria are fine as they will be deleted as part of the upsert anyways.
            _unitOfDataPersistanceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.CalculatedAttributePairJoin.CalculatedAttributePair.CalculatedAttribute.CalculatedAttributeLibraryId == library.Id);
            _unitOfDataPersistanceWork.Context.DeleteAll<CriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.CalculatedAttributePair.CalculatedAttribute.CalculatedAttributeLibraryId == library.Id);

            // Insert the new entities into the library
            UpsertCalculatedAttributes(library.CalculatedAttributes, library.Id);
        }

        public void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId)
        {
            ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());

            var pairEntities = new List<CalculatedAttributeEquationCriteriaPairEntity>();
            var equationEntities = new List<EquationEntity>();
            var equationJoins = new List<EquationCalculatedAttributePairEntity>();
            var criteriaJoins = new List<CriterionLibraryCalculatedAttributePairEntity>();
            var criterion = new List<CriterionLibraryEntity>();
            foreach (var calculatedAttribute in calculatedAttributes)
            {
                var entity = calculatedAttribute.ToLibraryEntity(_unitOfDataPersistanceWork.Context.Attribute.First(_ => _.Name == calculatedAttribute.Attribute));
                DeletePairs(entity);
                _unitOfDataPersistanceWork.Context.Upsert(entity, libraryId, _unitOfDataPersistanceWork.UserEntity?.Id);
                foreach (var pair in calculatedAttribute.Equations)
                {
                    var pairEntity = pair.ToLibraryEntity();
                    var equationEntity = pair.Equation.ToEntity();
                    var equationJoin = new EquationCalculatedAttributePairEntity()
                    {
                        CalculatedAttributePairId = pairEntity.Id,
                        CalculatedAttributePair = pairEntity,
                        EquationId = equationEntity.Id,
                        Equation = equationEntity
                    };
                    pairEntities.Add(pairEntity);
                    equationEntities.Add(equationEntity);
                    equationJoins.Add(equationJoin);

                    if (string.IsNullOrEmpty(pair.CriteriaLibrary?.MergedCriteriaExpression)) {
                        var criteriaEntity = pair.CriteriaLibrary.ToEntity();
                        var criteriaJoin = new CriterionLibraryCalculatedAttributePairEntity()
                        {
                            CalculatedAttributePairId = pairEntity.Id,
                            CalculatedAttributePair = pairEntity,
                            CriterionLibraryId = criteriaEntity.Id,
                            CriterionLibrary = criteriaEntity
                        };
                        criteriaJoins.Add(criteriaJoin);
                        criterion.Add(criteriaEntity);
                    }
                }
            }

            AddAllWithUser(pairEntities);
            AddAllWithUser(equationJoins);
            AddAllWithUser(equationEntities);
            if (criterion.Count > 0)
            {
                AddAllWithUser(criteriaJoins);
                AddAllWithUser(criterion);
            }
        }

        public void DeleteCalculatedAttributeLibrary(Guid libraryId)
        {
            var library = _unitOfDataPersistanceWork.Context.CalculatedAttributeLibrary.SingleOrDefault(_ => _.Id == libraryId);
            if (library == null) return;

            foreach (var attribute in library.CalculatedAttributes)
            {
                DeletePairs(attribute);
            }

            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeLibraryEntity>(_ => _.Id == libraryId);
        }
            

        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) =>
            _unitOfDataPersistanceWork.Context.ScenarioCalculatedAttribute
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId)
        {
            ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());
            // This will throw an error if no simulation is found.  That is the desired action here - let the API worry about the existence of the simulation
            var networkId = _unitOfDataPersistanceWork.NetworkRepo.GetPennDotNetwork().Id;
            var scenario = _unitOfDataPersistanceWork.SimulationRepo.GetSimulation(scenarioId).ToEntity(networkId);


            foreach (var calculatedAttribute in calculatedAttributes)
            {
                    _unitOfDataPersistanceWork.Context.Upsert(calculatedAttribute.ToScenarioEntity(scenario,
                        _unitOfDataPersistanceWork.Context.Attribute.First(_ => _.Name == calculatedAttribute.Attribute)),
                        scenarioId, _unitOfDataPersistanceWork.UserEntity?.Id);
            }
        }

        private void ValidateCalculatedAttributes(IQueryable<CalculatedAttributeDTO> calculatedAttributes)
        {
            var missingAttributeList = _unitOfDataPersistanceWork.Context.Attribute.Where(a => !calculatedAttributes.Any(c => c.Attribute == a.Name));
            if (missingAttributeList.Count() > 0)
            {
                var listOfAttributes = string.Join(",", missingAttributeList.Select(_ => _.Name).ToList());
                throw new ArgumentException($"The following calculated attributes have no matching attribute objects in the network: {listOfAttributes}");
            }

            if (calculatedAttributes.Any(_ => _.Equations == null))
            {
                var nullEquations = calculatedAttributes.Where(_ => _.Equations == null).Select(_ => _.Attribute).ToList();
                var listOfAttributes = string.Join(",", nullEquations);
                throw new ArgumentException($"The following calculated atttributes are invalid: {listOfAttributes}");
            }

            foreach (var attribute in calculatedAttributes)
            {
                if (attribute.Equations.Count < 1) throw new ArgumentException($"The calculated attribute for {attribute.Attribute} has no equations");
                if (attribute.Equations.Any(_ => string.IsNullOrEmpty(_.Equation.Expression))) throw new ArgumentException($"The calculated attribute for {attribute.Attribute} has equations that are null");

                var nullCriteria = attribute.Equations.Where(_ => _.CriteriaLibrary == null).Count();
                if (nullCriteria > 1) throw new ArgumentException($"The calculated attribute for {attribute.Attribute} has more than 1 null criteria");
                var emptyCriteria = attribute.Equations.Where(_ => string.IsNullOrEmpty(_.CriteriaLibrary.MergedCriteriaExpression)).Count();
                if (emptyCriteria > 1) throw new ArgumentException($"The calculated attribute for {attribute.Attribute} has more than 1 empty criteria");
                if (emptyCriteria + nullCriteria != 1) throw new ArgumentException($"There are multiple default equations for {attribute.Attribute}");
            }
        }

        private void DeletePairs(CalculatedAttributeEntity attribute)
        {
            _unitOfDataPersistanceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.CalculatedAttributePairJoin.CalculatedAttributePair.CalculatedAttributeId == attribute.Id);
            _unitOfDataPersistanceWork.Context.DeleteAll<CriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.CalculatedAttributePair.CalculatedAttributeId == attribute.Id);

            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeEquationCriteriaPairEntity>(_ => _.CalculatedAttributeId == attribute.Id);
        }

        private void DeletePairs(ScenarioCalculatedAttributeEntity attribute)
        {
            _unitOfDataPersistanceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.ScenarioCalculatedAttributePairJoin.ScenarioCalculatedAttributePairId == attribute.Id);
            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.ScenarioCalculatedAttributePairId == attribute.Id);

            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.ScenarioCalculatedAttributePairId == attribute.Id);
        }

        private void AddAllWithUser<T>(List<T> entity) where T : class => _unitOfDataPersistanceWork.Context.AddAll(entity, _unitOfDataPersistanceWork.UserEntity?.Id);
    }
}
