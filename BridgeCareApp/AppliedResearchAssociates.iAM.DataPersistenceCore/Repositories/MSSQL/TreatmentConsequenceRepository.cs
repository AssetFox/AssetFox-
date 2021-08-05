﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentConsequenceRepository : ITreatmentConsequenceRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentConsequenceRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId, string simulationName)
        {
            var consequenceEntities = new List<ConditionalTreatmentConsequenceEntity>();
            var equationEntityPerConsequenceEntityId = new Dictionary<Guid, EquationEntity>();
            var consequenceEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var allConsequences = consequencesPerTreatmentId.Values.SelectMany(_ => _).ToList();
            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
            var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
            if (!allConsequences.All(_ => attributeNames.Contains(_.Attribute.Name)))
            {
                var missingAttributes = allConsequences.Select(_ => _.Attribute.Name)
                    .Except(attributeNames).ToList();
                if (missingAttributes.Count == 1)
                {
                    throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                }

                throw new RowNotInTableException(
                    $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
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

            _unitOfWork.Context.AddAll(consequenceEntities);

            if (equationEntityPerConsequenceEntityId.Values.Any())
            {
                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerConsequenceEntityId,
                    DataPersistenceConstants.EquationJoinEntities.TreatmentConsequence);
            }

            if (consequenceEntityIdsPerExpression.Values.Any())
            {
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(consequenceEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.ConditionalTreatmentConsequence, simulationName);
            }
        }

        public void UpsertOrDeleteTreatmentConsequences(Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId,
            Guid libraryId)
        {
            var treatmentConsequences = treatmentConsequencePerTreatmentId.SelectMany(_ => _.Value.ToList()).ToList();

            var attributeEntities = _unitOfWork.Context.Attribute.ToList();
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

            var conditionalTreatmentConsequenceEntities = treatmentConsequencePerTreatmentId.SelectMany(_ =>
                    _.Value.Select(__ =>
                        __.ToEntity(_.Key, attributeEntities.Single(___ => ___.Name == __.Attribute).Id)))
                .ToList();

            var entityIds = conditionalTreatmentConsequenceEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentConsequence
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ConditionalTreatmentConsequenceEntity>(_ =>
                _.SelectableTreatment.TreatmentLibraryId == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(conditionalTreatmentConsequenceEntities
                .Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(conditionalTreatmentConsequenceEntities
                .Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            if (treatmentConsequences.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationEntitiesPerJoinEntityId = treatmentConsequences
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .ToDictionary(_ => _.Id, _ => _.Equation.ToEntity());

                _unitOfWork.EquationRepo.CreateEquations(equationEntitiesPerJoinEntityId,
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

                _unitOfWork.Context.AddAll(criterionLibraryJoinsToAdd, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}
