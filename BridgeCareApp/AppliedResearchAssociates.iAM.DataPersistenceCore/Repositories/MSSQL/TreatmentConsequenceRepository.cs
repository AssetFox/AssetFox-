using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentConsequenceRepository : ITreatmentConsequenceRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public TreatmentConsequenceRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId, string simulationName)
        {
            var consequenceEntities = new List<ConditionalTreatmentConsequenceEntity>();
            var equationEntityPerConsequenceEntityId = new Dictionary<Guid, EquationEntity>();
            var consequenceEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var attributeNames = consequencesPerTreatmentId.Values
                .SelectMany(_ => _.Select(__ => __.Attribute.Name).Distinct()).ToList();
            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute
                .Where(_ => attributeNames.Contains(_.Name)).ToList();

            if (!attributeEntities.Any())
            {
                throw new RowNotInTableException("No attributes found for treatment consequences.");
            }

            var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
            if (!attributeNames.All(attributeName => attributeNamesFromDataSource.Contains(attributeName)))
            {
                var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                if (attributeNamesNotFound.Count() == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                }

                throw new RowNotInTableException($"No attributes found having names: {string.Join(", ", attributeNamesNotFound)}.");
            }

            consequencesPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var entities = consequencesPerTreatmentId[treatmentId].Select(_ =>
                    {
                        var consequenceEntity = _.ToEntity(treatmentId,
                            attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id);

                        if (!_.Equation.ExpressionIsBlank)
                        {
                            equationEntityPerConsequenceEntityId.Add(consequenceEntity.Id, _.Equation.ToEntity());
                        }

                        if (!_.Criterion.ExpressionIsBlank)
                        {
                            if (consequenceEntityIdsPerExpression.ContainsKey(_.Criterion.Expression))
                            {
                                consequenceEntityIdsPerExpression[_.Criterion.Expression].Add(consequenceEntity.Id);
                            }
                            else
                            {
                                consequenceEntityIdsPerExpression.Add(_.Criterion.Expression, new List<Guid>
                                {
                                    consequenceEntity.Id
                                });
                            }
                        }

                        return consequenceEntity;
                    })
                    .ToList();

                consequenceEntities.AddRange(entities);
            });

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentConsequence.AddRange(consequenceEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(consequenceEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (equationEntityPerConsequenceEntityId.Values.Any())
            {
                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntityPerConsequenceEntityId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentConsequence);
            }

            if (consequenceEntityIdsPerExpression.Values.Any())
            {
                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(consequenceEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.ConditionalTreatmentConsequence, simulationName);
            }
        }

        public void AddOrUpdateOrDeleteTreatmentConsequences(Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId,
            Guid libraryId)
        {
            var treatmentConsequences = treatmentConsequencePerTreatmentId.SelectMany(_ => _.Value.ToList()).ToList();

            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!treatmentConsequences.All(_ => attributeNames.Contains(_.Attribute)))
            {
                var missingAttributes = treatmentConsequences.Select(_ => _.Attribute)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
            }

            var entities = treatmentConsequencePerTreatmentId.SelectMany(_ =>
                    _.Value.Select(__ =>
                        __.ToEntity(_.Key, attributeEntities.Single(___ => ___.Name == __.Attribute).Id)))
                .ToList();

            var entityIds = entities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfDataPersistenceWork.Context.TreatmentConsequence
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<ConditionalTreatmentConsequenceEntity, bool>>>
            {
                {"delete", _ => _.SelectableTreatment.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkAddOrUpdateOrDelete(entities, predicatesPerCrudOperation);
            }

            if (treatmentConsequences.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntitiesPerJoinEntityId = treatmentConsequences
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntitiesPerJoinEntityId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentConsequence);
            }

            if (treatmentConsequences.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionLibraryJoinsToAdd = treatmentConsequences
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)).Select(_ =>
                        new CriterionLibraryConditionalTreatmentConsequenceEntity
                        {
                            CriterionLibraryId = _.CriterionLibrary.Id,
                            ConditionalTreatmentConsequenceId = _.Id
                        }).ToList();

                if (IsRunningFromXUnit)
                {
                    _unitOfDataPersistenceWork.Context.CriterionLibraryTreatmentConsequence.AddRange(criterionLibraryJoinsToAdd);
                }
                else
                {
                    _unitOfDataPersistenceWork.Context.BulkInsert(criterionLibraryJoinsToAdd);
                }
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
