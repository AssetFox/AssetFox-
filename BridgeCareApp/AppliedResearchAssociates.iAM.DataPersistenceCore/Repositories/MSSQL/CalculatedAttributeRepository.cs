using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

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
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Equations)
                .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CalculatedAttributes)
                .ThenInclude(_ => _.Equations)
                .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library)
        {
            // Does the library have a provided ID?
            AssignIdWhenNull(library);

            // Update the library
            _unitOfDataPersistanceWork.Context.Upsert(library.ToLibraryEntity(), library.Id, _unitOfDataPersistanceWork.UserEntity?.Id);

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
            if (!_unitOfDataPersistanceWork.Context.CalculatedAttributeLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified calculated attribute library was not found.");
            }

            ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());

            // Assign IDs as needed
            

            var entities = calculatedAttributes
                .Select(calc =>
                {
                    AssignIdWhenNull(calc);
                    return calc.ToLibraryEntity(_unitOfDataPersistanceWork.Context.Attribute.First(attr => attr.Name == calc.Attribute).Id);
                })
                .ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistanceWork.Context.CalculatedAttribute.AsNoTracking()
                .Where(_ => _.CalculatedAttributeLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeEntity>(_ =>
                _.CalculatedAttributeLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfDataPersistanceWork.Context.UpdateAll(entities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistanceWork.UserEntity?.Id);

            _unitOfDataPersistanceWork.Context.AddAll(entities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistanceWork.UserEntity?.Id);

            DeleteLibraryPairs(libraryId);

            var calculatedAttributeEquationCriteriaPairs = new List<CalculatedAttributeEquationCriteriaPairEntity>();
            var equations = new List<EquationEntity>();
            var equationPairJoins = new List<EquationCalculatedAttributePairEntity>();
            var criteria = new List<CriterionLibraryEntity>();
            var criteriaPairJoins = new List<CriterionLibraryCalculatedAttributePairEntity>();

            calculatedAttributes.ForEach(calcAttr =>
            {
                calculatedAttributeEquationCriteriaPairs.AddRange(
                    calcAttr.Equations.Select(pair =>
                    {
                        AssignIdWhenNull(pair);
                        return pair.ToLibraryEntity(calcAttr.Id);
                    }));
                equations.AddRange(calcAttr.Equations.Select(pair =>
                {
                    AssignIdWhenNull(pair.Equation);
                    return pair.Equation.ToEntity();
                }));
                equationPairJoins.AddRange(
                    calcAttr.Equations.Select(pair => pair.Equation.ToLibraryEntity(pair.Id)));
                criteria.AddRange(calcAttr.Equations.Where(_ => _.CriteriaLibrary != null).Select(pair =>
                {
                    AssignIdWhenNull(pair.CriteriaLibrary);
                    var entity = pair.CriteriaLibrary.ToEntity();
                    entity.IsSingleUse = true;
                    return entity;
                }));
                criteriaPairJoins.AddRange(calcAttr.Equations
                    .Where(pair => pair.CriteriaLibrary != null)
                    .Select(pair => pair.CriteriaLibrary.ToLibraryEntity(pair.Id)));
            });

            AddAllWithUser(calculatedAttributeEquationCriteriaPairs);
            AddAllWithUser(equations);
            AddAllWithUser(equationPairJoins);
            AddAllWithUser(criteria);
            AddAllWithUser(criteriaPairJoins);
        }

        public void DeleteCalculatedAttributeLibrary(Guid libraryId)
        {
            var library = _unitOfDataPersistanceWork.Context.CalculatedAttributeLibrary.SingleOrDefault(_ => _.Id == libraryId);
            if (library == null) return;

            foreach (var attribute in library.CalculatedAttributes)
            {
                DeleteLibraryPairs(libraryId);
            }

            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeLibraryEntity>(_ => _.Id == libraryId);
        }
            

        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) =>
            _unitOfDataPersistanceWork.Context.ScenarioCalculatedAttribute
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.Attribute)
                .Include(_ => _.Equations)
                .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Equations)
                .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
                .ThenInclude(_ => _.Equation)
                .Select(_ => _.ToDto())
                .ToList();

        public void UpsertScenarioCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId)
        {
            // This will throw an error if no simulation is found.  That is the desired action here.
            // Let the API worry about the existence of the simulation
            _unitOfDataPersistanceWork.SimulationRepo.GetSimulation(scenarioId);

            ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());

            var entities = calculatedAttributes
                .Select(calc =>
                {
                    AssignIdWhenNull(calc);
                    return calc.ToScenarioEntity(scenarioId, _unitOfDataPersistanceWork.Context.Attribute.First(attr => attr.Name == calc.Attribute).Id);
                });

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistanceWork.Context.ScenarioCalculatedAttribute.AsNoTracking()
                .Where(_ => _.SimulationId == scenarioId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCalculatedAttributeEntity>(_ =>
                _.SimulationId == scenarioId && !entityIds.Contains(_.Id));

            _unitOfDataPersistanceWork.Context.UpdateAll(entities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistanceWork.UserEntity?.Id);

            _unitOfDataPersistanceWork.Context.AddAll(entities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistanceWork.UserEntity?.Id);

            DeleteScenarioPairs(scenarioId);

            var calculatedAttributeEquationCriteriaPairs = new List<ScenarioCalculatedAttributeEquationCriteriaPairEntity>();
            var equations = new List<EquationEntity>();
            var equationPairJoins = new List<ScenarioEquationCalculatedAttributePairEntity>();
            var criteria = new List<CriterionLibraryEntity>();
            var criteriaPairJoins = new List<ScenarioCriterionLibraryCalculatedAttributePairEntity>();

            calculatedAttributes.ForEach(calcAttr =>
            {
                calculatedAttributeEquationCriteriaPairs.AddRange(calcAttr.Equations.Select(p =>
                {
                    AssignIdWhenNull(p);
                    return p.ToScenarioEntity(calcAttr.Id);
                }));
                equations.AddRange(calcAttr.Equations.Select(p =>
                {
                    AssignIdWhenNull(p.Equation);
                    return p.Equation.ToEntity();
                }));
                equationPairJoins.AddRange(calcAttr.Equations.Select(p => p.Equation.ToScenarioEntity(p.Id)));
                criteria.AddRange(calcAttr.Equations.Where(_ => _.CriteriaLibrary != null).Select(p =>
                {
                    AssignIdWhenNull(p.CriteriaLibrary);
                    var entity = p.CriteriaLibrary.ToEntity();
                    entity.IsSingleUse = true;
                    return entity;
                }));
                criteriaPairJoins.AddRange(calcAttr.Equations
                    .Where(p => p.CriteriaLibrary != null)
                    .Select(p => p.CriteriaLibrary.ToScenarioEntity(p.Id)));
            });

            AddAllWithUser(calculatedAttributeEquationCriteriaPairs);
            AddAllWithUser(equations);
            AddAllWithUser(equationPairJoins);
            AddAllWithUser(criteria);
            AddAllWithUser(criteriaPairJoins);
        }



        private void ValidateCalculatedAttributes(IQueryable<CalculatedAttributeDTO> calculatedAttributes)
        {
            var missingAttributes = calculatedAttributes.Where(_ =>
                !_unitOfDataPersistanceWork.Context.Attribute.Any(attr => attr.Name == _.Attribute));
            if (missingAttributes.Any())
            {
                throw new ArgumentException(
                    $"The following calculated attributes have no matching attribute objects in the network: {JoinAttributesIntoCommaSeparatedString(missingAttributes)}");
            }

            var nullEquationAttributes = calculatedAttributes.Where(_ => _.Equations == null);
            if (nullEquationAttributes.Any())
            {
                throw new ArgumentException(
                    $"The following calculated attributes are invalid: {JoinAttributesIntoCommaSeparatedString(nullEquationAttributes)}");
            }

            var noEquationAttributes = calculatedAttributes.Where(_ => _.Equations.Count < 1);
            if (noEquationAttributes.Any())
            {
                throw new ArgumentException(
                    $"The following calculated attributes have no equations: {JoinAttributesIntoCommaSeparatedString(noEquationAttributes)}");
            }

            var noEquationExpressionAttributes = calculatedAttributes
                .Where(_ => _.Equations.Any(equation => string.IsNullOrEmpty(equation.Equation.Expression)));
            if (noEquationExpressionAttributes.Any())
            {
                throw new ArgumentException(
                    $"The following calculated attributes have empty equation expressions: {JoinAttributesIntoCommaSeparatedString(noEquationExpressionAttributes)}");
            }

            var attributesWithErrors = new List<string>();
            foreach (var calcAttr in calculatedAttributes)
            {
                var nullCriteria = calcAttr.Equations.Where(equation => equation.CriteriaLibrary == null);
                var noCriteriaExpressionAttributes = calcAttr.Equations
                    .Where(_ => _.CriteriaLibrary != null)
                    .Where(_ => string.IsNullOrEmpty(_.CriteriaLibrary.MergedCriteriaExpression));
                if (nullCriteria.Count() + noCriteriaExpressionAttributes.Count() != 1) attributesWithErrors.Add(calcAttr.Attribute);
            }

            if (attributesWithErrors.Count > 0)
            {
                throw new ArgumentException(
                    $"The following calculated attributes do not have the proper number of default equations: {string.Join(", ", attributesWithErrors)}");
            }
        }

        private void DeleteLibraryPairs(Guid libraryId)
        {
            _unitOfDataPersistanceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.CalculatedAttributePairJoin.CalculatedAttributePair.CalculatedAttribute
                    .CalculatedAttributeLibraryId == libraryId);

            _unitOfDataPersistanceWork.Context.DeleteAll<CriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.CalculatedAttributePair.CalculatedAttribute
                    .CalculatedAttributeLibraryId == libraryId);

            _unitOfDataPersistanceWork.Context.DeleteAll<CalculatedAttributeEquationCriteriaPairEntity>(_ =>
                _.CalculatedAttribute.CalculatedAttributeLibraryId == libraryId);
        }

        private void DeleteScenarioPairs(Guid simulationId)
        {
            _unitOfDataPersistanceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.ScenarioCalculatedAttributePairJoin.ScenarioCalculatedAttributePair.ScenarioCalculatedAttribute
                    .SimulationId == simulationId);

            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.ScenarioCalculatedAttributePair.ScenarioCalculatedAttribute.SimulationId == simulationId);

            _unitOfDataPersistanceWork.Context.DeleteAll<ScenarioCalculatedAttributeEquationCriteriaPairEntity>(_ =>
                _.ScenarioCalculatedAttribute.SimulationId == simulationId);
        }

        private void AddAllWithUser<T>(List<T> entity) where T : class =>
            _unitOfDataPersistanceWork.Context.AddAll(entity, _unitOfDataPersistanceWork.UserEntity?.Id);

        private string JoinAttributesIntoCommaSeparatedString(IQueryable<CalculatedAttributeDTO> calculatedAttributes) =>
            string.Join(", ", calculatedAttributes.Select(_ => _.Attribute).ToList());

        private void AssignIdWhenNull(AppliedResearchAssociates.iAM.DTOs.Abstract.BaseDTO dto)
        {
            if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
        }
    }
}
