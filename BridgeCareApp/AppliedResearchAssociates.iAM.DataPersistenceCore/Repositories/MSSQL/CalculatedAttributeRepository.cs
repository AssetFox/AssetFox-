using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
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
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public CalculatedAttributeRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfDataPersistenceWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public ICollection<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibraries() =>
            _unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary
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

        public List<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibrariesNoChildren()
        {
            return _unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByScenarioIdNoChildren(Guid scenarioId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == scenarioId))
            {
                throw new RowNotInTableException("The specified scenario was not found");
            }

            return _unitOfDataPersistenceWork.Context.ScenarioCalculatedAttribute
                .Include(_ => _.Attribute)
                .Where(_ => _.SimulationId == scenarioId)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<CalculatedAttributeDTO> GetCalcuatedAttributesByLibraryIdNoChildren(Guid libraryid)
        {
            if (!_unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary.Any(_ => _.Id == libraryid))
            {
                throw new RowNotInTableException("The specified calculated attribute library was not found");
            }

            return _unitOfDataPersistenceWork.Context.CalculatedAttribute
                .Include(_ => _.Attribute)
                .Where(_ => _.CalculatedAttributeLibraryId == libraryid)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public CalculatedAttributeDTO GetLibraryCalulatedAttributesByLibraryAndAttributeId(Guid libraryId, Guid attributeId)
        {
            return _unitOfDataPersistenceWork.Context.CalculatedAttribute.AsNoTracking()
            .Include(_ => _.Attribute)
            .Include(_ => _.Equations)
            .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
            .ThenInclude(_ => _.CriterionLibrary)
            .Include(_ => _.Equations)
            .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
            .ThenInclude(_ => _.Equation)
            .Single(_ => _.Attribute.Id == attributeId && _.CalculatedAttributeLibraryId == libraryId)
            .ToDto();
        }

        public CalculatedAttributeDTO GetScenarioCalulatedAttributesByScenarioAndAttributeId(Guid scenarioId, Guid attributeId)
        {
            return _unitOfDataPersistenceWork.Context.ScenarioCalculatedAttribute.AsNoTracking()
            .Include(_ => _.Attribute)
            .Include(_ => _.Equations)
            .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
            .ThenInclude(_ => _.CriterionLibrary)
            .Include(_ => _.Equations)
            .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
            .ThenInclude(_ => _.Equation)
            .Single(_ => _.Attribute.Id == attributeId && _.SimulationId == scenarioId)
            .ToDto();
        }

        public CalculatedAttributeLibraryDTO GetCalculatedAttributeLibraryByID(Guid id)
        {
            return _unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary
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
            .Single(_ => _.Id == id).ToDto();
        }

        public void UpsertCalculatedAttributeLibrary(CalculatedAttributeLibraryDTO library)
        {
            _unitOfDataPersistenceWork.AsTransaction(u =>
            {
                // Does the library have a provided ID?
                AssignIdWhenNull(library);

                var existingLibrary = u.Context.CalculatedAttributeLibrary
                    .Include(_ => _.CalculatedAttributes)
                    .ThenInclude(_ => _.Equations)
                    .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                    .FirstOrDefault(_ => _.Id == library.Id);

                if (existingLibrary != null)
                {
                    var criteriaIds = existingLibrary
                        .CalculatedAttributes.SelectMany(_ => _.Equations)
                        .Where(_ => _.CriterionLibraryCalculatedAttributeJoin != null)
                        .Select(_ => _.CriterionLibraryCalculatedAttributeJoin)
                        .Select(_ => _.CriterionLibraryId);

                    // Delete the criteria
                    u.Context.DeleteAll<CriterionLibraryEntity>(_ => criteriaIds.Contains(_.Id));
                }

                // Update the library
                u.Context.Upsert(library.ToLibraryEntity(), library.Id, u.UserEntity?.Id);

                // Delete the entities attached to the library that are no longer there
                var entityIds = library.CalculatedAttributes.Select(_ => _.Id).ToList();
                // This SHOULD cascade all deletes except for equations and criteria
                u.Context.DeleteAll<CalculatedAttributeEntity>(_ => _.CalculatedAttributeLibraryId == library.Id && !entityIds.Contains(_.Id));
                // Deleteing all equations and criteria are fine as they will be deleted as part of the upsert anyways.
                u.Context.DeleteAll<EquationEntity>(_ =>
                    _.CalculatedAttributePairJoin.CalculatedAttributePair.CalculatedAttribute.CalculatedAttributeLibraryId == library.Id);
                u.Context.DeleteAll<CriterionLibraryCalculatedAttributePairEntity>(_ =>
                    _.CalculatedAttributePair.CalculatedAttribute.CalculatedAttributeLibraryId == library.Id);


                // Insert the new entities into the library
                UpsertCalculatedAttributes(library.CalculatedAttributes, library.Id);
            });
        }

        public void UpsertCalculatedAttributes(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid libraryId)
        {
            if (!_unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified calculated attribute library was not found.");
            }

            ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());

            // Assign IDs as needed
            

            var entities = calculatedAttributes
                .Select(calc =>
                {
                    AssignIdWhenNull(calc);
                    return calc.ToLibraryEntity(libraryId, _unitOfDataPersistenceWork.Context.Attribute.First(attr => attr.Name == calc.Attribute).Id);
                })
                .ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.CalculatedAttribute.AsNoTracking()
                .Where(_ => _.CalculatedAttributeLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            DeleteLibraryPairs(calculatedAttributes.Select(_ => _.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<CalculatedAttributeEntity>(_ =>
                _.CalculatedAttributeLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.UpdateAll(entities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistenceWork.UserEntity?.Id);

            _unitOfDataPersistenceWork.Context.AddAll(entities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistenceWork.UserEntity?.Id);

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
            var library = _unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary.SingleOrDefault(_ => _.Id == libraryId);
            if (library == null) return;

            DeleteLibraryPairs(library.CalculatedAttributes.Select(_ => _.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<CalculatedAttributeLibraryEntity>(_ => _.Id == libraryId);
        }
            

        public ICollection<CalculatedAttributeDTO> GetScenarioCalculatedAttributes(Guid simulationId) =>
            _unitOfDataPersistenceWork.Context.ScenarioCalculatedAttribute
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

        public void UpsertScenarioCalculatedAttributesAtomically(ICollection<CalculatedAttributeDTO> calculatedAttributes, Guid scenarioId)
        {
            // This will throw an error if no simulation is found.  That is the desired action here.
            // Let the API worry about the existence of the simulation
            _unitOfDataPersistenceWork.AsTransaction(u =>
            {
                u.SimulationRepo.GetSimulation(scenarioId);

                ValidateCalculatedAttributes(calculatedAttributes.AsQueryable());

                var entities = calculatedAttributes
                    .Select(calc =>
                    {
                        AssignIdWhenNull(calc);
                        return calc.ToScenarioEntity(scenarioId, u.Context.Attribute.First(attr => attr.Name == calc.Attribute).Id);
                    });

                var entityIds = entities.Select(_ => _.Id).ToList();

                var existingEntityIds = _unitOfDataPersistenceWork.Context.ScenarioCalculatedAttribute.AsNoTracking()
                    .Where(_ => _.SimulationId == scenarioId && entityIds.Contains(_.Id))
                    .Select(_ => _.Id).ToList();

                DeleteScenarioPairs(scenarioId);

                u.Context.DeleteAll<ScenarioCalculatedAttributeEntity>(_ =>
                    _.SimulationId == scenarioId && !entityIds.Contains(_.Id));

                u.Context.UpdateAll(entities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistenceWork.UserEntity?.Id);

                _unitOfDataPersistenceWork.Context.AddAll(entities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(), _unitOfDataPersistenceWork.UserEntity?.Id);

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
            });
        }



        private void ValidateCalculatedAttributes(IQueryable<CalculatedAttributeDTO> calculatedAttributes)
        {
            var missingAttributes = calculatedAttributes.Where(_ =>
                !_unitOfDataPersistenceWork.Context.Attribute.Any(attr => attr.Name == _.Attribute));
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

        private void DeleteLibraryPairs(IEnumerable<Guid> attributeIds)
        {
            _unitOfDataPersistenceWork.Context.DeleteAll<EquationEntity>(_ => attributeIds.Contains(
                _.CalculatedAttributePairJoin.CalculatedAttributePair.CalculatedAttribute.Id));

            var pairsToDelete = _unitOfDataPersistenceWork.Context.CalculatedAttribute
                .Include(_ => _.Equations)
                .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .Where(_ => attributeIds.Contains(_.Id))
                .SelectMany(_ => _.Equations);

            var criteriaIds = pairsToDelete
                .Where(_ => _.CriterionLibraryCalculatedAttributeJoin != null)
                .Select(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .Select(_ => _.CriterionLibraryId);

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryEntity>(_ => criteriaIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryCalculatedAttributePairEntity>(_ => attributeIds.Contains(
                _.CalculatedAttributePair.CalculatedAttribute.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<CalculatedAttributeEquationCriteriaPairEntity>(_ => pairsToDelete.Select(pair => pair.Id).Contains(
                _.Id));

        }

        private void DeleteScenarioPairs(Guid simulationId)
        {
            _unitOfDataPersistenceWork.Context.DeleteAll<EquationEntity>(_ =>
                _.ScenarioCalculatedAttributePairJoin.ScenarioCalculatedAttributePair.ScenarioCalculatedAttribute
                    .SimulationId == simulationId);

            var criteriaIds = _unitOfDataPersistenceWork.Context.ScenarioCalculatedAttribute
                .Include(_ => _.Equations)
                .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .Where(_ => _.SimulationId == simulationId)
                .SelectMany(_ => _.Equations)
                .Where(_ => _.CriterionLibraryCalculatedAttributeJoin != null)
                .Select(_ => _.CriterionLibraryCalculatedAttributeJoin)
                .Select(_ => _.CriterionLibraryId);

            _unitOfDataPersistenceWork.Context.DeleteAll<CriterionLibraryEntity>(_ => criteriaIds.Contains(_.Id));

            _unitOfDataPersistenceWork.Context.DeleteAll<ScenarioCriterionLibraryCalculatedAttributePairEntity>(_ =>
                _.ScenarioCalculatedAttributePair.ScenarioCalculatedAttribute.SimulationId == simulationId);

            _unitOfDataPersistenceWork.Context.DeleteAll<ScenarioCalculatedAttributeEquationCriteriaPairEntity>(_ =>
                _.ScenarioCalculatedAttribute.SimulationId == simulationId);
        }

        private void AddAllWithUser<T>(List<T> entity) where T : class =>
            _unitOfDataPersistenceWork.Context.AddAll(entity, _unitOfDataPersistenceWork.UserEntity?.Id);

        private string JoinAttributesIntoCommaSeparatedString(IQueryable<CalculatedAttributeDTO> calculatedAttributes) =>
            string.Join(", ", calculatedAttributes.Select(_ => _.Attribute).ToList());

        private void AssignIdWhenNull(AppliedResearchAssociates.iAM.DTOs.Abstract.BaseDTO dto)
        {
            if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
        }

        public void PopulateScenarioCalculatedFields(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var explorer = simulation.Network.Explorer;

            var calculatedFieldName = explorer.CalculatedFields.Select(_ => _.Name);
            var calculatedAttributesToPopulate = GetScenarioCalculatedAttributes(simulation.Id)
                .ToList()
                .Where(_ => calculatedFieldName.Contains(_.Attribute))
                .ToList();
            if (calculatedAttributesToPopulate.Any(_ => !Enum.IsDefined(typeof(CalculatedFieldTiming), _.CalculationTiming)))
            {
                var badTimingList = calculatedAttributesToPopulate
                    .Where(_ => !Enum.IsDefined(typeof(CalculatedFieldTiming), _.CalculationTiming))
                    .Select(_ => _.Attribute).ToList();
                throw new RowNotInTableException($"Calculations have invalid timings ({string.Join(",", badTimingList)}");
            }

            foreach (var calcAttr in calculatedAttributesToPopulate)
            {
                var calculatedField = explorer.CalculatedFields.First(_ => _.Name == calcAttr.Attribute);
                calculatedField.Timing = (CalculatedFieldTiming)calcAttr.CalculationTiming;
                foreach (var pair in calcAttr.Equations)
                {
                    var source = calculatedField.AddValueSource();
                    source.Equation.Expression = pair.Equation.Expression;
                    source.Criterion.Expression = pair.CriteriaLibrary?.MergedCriteriaExpression ?? string.Empty;
                }
            }
        }
        public List<CalculatedAttributeLibraryDTO> GetCalculatedAttributeLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfDataPersistenceWork.Context.CalculatedAttributeLibraryUser
                .AsNoTracking()
                .Include(u => u.CalculatedAttributeLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.CalculatedAttributeLibrary.ToDto())
                .ToList();
        }
        public void UpsertOrDeleteUsers(Guid calculatedAttributeLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfDataPersistenceWork.Context.CalculatedAttributeLibraryUser.Where(u => u.LibraryId == calculatedAttributeLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToCalculatedAttributeLibraryUserEntity(u, calculatedAttributeLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<CalculatedAttributeLibraryUserEntity>();
            foreach (var dto in dtosToUpdate)
            {
                var entityToUpdate = entitiesToMaybeUpdate.FirstOrDefault(e => e.UserId == dto.UserId);
                if (entityToUpdate != null && entityToUpdate.AccessLevel != (int)dto.AccessLevel)
                {
                    entityToUpdate.AccessLevel = (int)dto.AccessLevel;
                    entitiesToUpdate.Add(entityToUpdate);
                }
            }
            _unitOfDataPersistenceWork.Context.AddRange(entitiesToAdd);
            _unitOfDataPersistenceWork.Context.UpdateRange(entitiesToUpdate);
            var entitiesToDelete = existingEntities.Where(u => userIdsToDelete.Contains(u.UserId)).ToList();
            _unitOfDataPersistenceWork.Context.RemoveRange(entitiesToDelete);
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        private List<LibraryUserDTO> GetAccessForUser(Guid calculatedAttributeLibraryId, Guid userId)
        {
            var dtos = _unitOfDataPersistenceWork.Context.CalculatedAttributeLibraryUser
                .Where(u => u.LibraryId == calculatedAttributeLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public List<LibraryUserDTO> GetLibraryUsers(Guid calculatedAttributeLibraryId)
        {
            var dtos = _unitOfDataPersistenceWork.Context.CalculatedAttributeLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == calculatedAttributeLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfDataPersistenceWork.Context.CalculatedAttributeLibrary.Any(bl => bl.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }
    }
}
