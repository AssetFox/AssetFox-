using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentConsequenceRepository : ITreatmentConsequenceRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentConsequenceRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public List<TreatmentConsequenceDTO> GetScenarioTreatmentConsequencesByTreatmentId(Guid treatmentId)
        {
            if (!_unitOfWork.Context.ScenarioConditionalTreatmentConsequences.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException("The specified scenario treamtment was not found");
            }

            return  _unitOfWork.Context.ScenarioConditionalTreatmentConsequences
                .Include(_ => _.Attribute)
                .Include(_ => _.ScenarioConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary).Select(_ => _.ToDto()).ToList();
        }

        public List<TreatmentConsequenceDTO> GetTreatmentConsequencesByTreatmentId(Guid treatmentId)
        {
            if (!_unitOfWork.Context.TreatmentConsequence.Any(_ => _.Id == treatmentId))
            {
                throw new RowNotInTableException("The specified treamtment was not found");
            }

            return _unitOfWork.Context.TreatmentConsequence
                .Include(_ => _.Attribute)
                .Include(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary).Select(_ => _.ToDto()).ToList();
        }

        public List<CommittedProjectConsequenceDTO> GetCommittedProjectConsequencesByProjectId(Guid projectId)
        {
            if (!_unitOfWork.Context.CommittedProject.Any(_ => _.Id == projectId))
            {
                throw new RowNotInTableException("The specified committed project was not found");
            }

            return _unitOfWork.Context.CommittedProjectConsequence
                .Include(_ => _.Attribute).Select(_ => _.ToDTO()).ToList();
        }

        public void CreateScenarioConditionalTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId)
        {
            var consequenceEntities = new List<ScenarioConditionalTreatmentConsequenceEntity>();
            var equationEntities = new List<EquationEntity>();
            var equationJoinEntities = new List<ScenarioConditionalTreatmentConsequenceEquationEntity>();
            var criterionEntities = new List<CriterionLibraryEntity>();
            var criterionJoinEntities = new List<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>();

            var allConsequences = consequencesPerTreatmentId.Values.SelectMany(_ => _).ToList();
            var attributeEntities = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
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
                consequenceEntities.AddRange(consequencesPerTreatmentId[treatmentId].Select(_ =>
                    {
                        var consequenceEntity = _.ToScenarioEntity(treatmentId,
                            attributeEntities.Single(__ => __.Name == _.Attribute.Name).Id);

                        if (!_.Equation.ExpressionIsBlank)
                        {
                            var equationEntity = _.Equation.ToEntity();
                            equationEntities.Add(equationEntity);
                            equationJoinEntities.Add(new ScenarioConditionalTreatmentConsequenceEquationEntity
                            {
                                ScenarioConditionalTreatmentConsequenceId = _.Id, EquationId = equationEntity.Id
                            });
                        }

                        if (!_.Criterion.ExpressionIsBlank)
                        {
                            var criterionEntity = _.Criterion.ToEntity("");
                            criterionEntity.IsSingleUse = true;
                            criterionEntities.Add(criterionEntity);
                            criterionJoinEntities.Add(new CriterionLibraryScenarioConditionalTreatmentConsequenceEntity
                            {
                                CriterionLibraryId = criterionEntity.Id, ScenarioConditionalTreatmentConsequenceId = _.Id
                            });
                        }

                        return consequenceEntity;
                    }).ToList());
            });

            _unitOfWork.Context.AddAll(consequenceEntities);
            _unitOfWork.Context.AddAll(equationEntities);
            _unitOfWork.Context.AddAll(equationJoinEntities);
            _unitOfWork.Context.AddAll(criterionEntities);
            _unitOfWork.Context.AddAll(criterionJoinEntities);
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

            var conditionalTreatmentConsequenceEntities = treatmentConsequencePerTreatmentId
                .SelectMany(_ => _.Value.Select(consequence => consequence
                    .ToLibraryEntity(_.Key, attributeEntities
                        .Single(attribute => attribute.Name == consequence.Attribute).Id)))
                .ToList();

            var entityIds = conditionalTreatmentConsequenceEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentConsequence.AsNoTracking()
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
                var equationJoins = new List<ConditionalTreatmentConsequenceEquationEntity>();

                var equations = treatmentConsequences
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .Select(consequence =>
                    {
                        var equation = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = consequence.Equation.Expression,
                        };
                        equationJoins.Add(new ConditionalTreatmentConsequenceEquationEntity
                        {
                            EquationId = equation.Id,
                            ConditionalTreatmentConsequenceId = consequence.Id
                        });
                        return equation;
                    }).ToList();

                // Delete any existing entries for related treatment consequences               
                _unitOfWork.Context.DeleteAll<ConditionalTreatmentConsequenceEquationEntity>(_ => existingEntityIds.Contains(_.ConditionalTreatmentConsequenceId));

                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoins, _unitOfWork.UserEntity?.Id);
            }

            if (treatmentConsequences.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryConditionalTreatmentConsequenceEntity>();

                var criteria = treatmentConsequences
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryConditionalTreatmentConsequenceEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            ConditionalTreatmentConsequenceId = treatment.Id
                        });
                        return criterion;
                    }).ToList();

                // Delete any existing entries for related treatment consequences               
                _unitOfWork.Context.DeleteAll<CriterionLibraryConditionalTreatmentConsequenceEntity>(_ => existingEntityIds.Contains(_.ConditionalTreatmentConsequenceId));

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void UpsertOrDeleteScenarioTreatmentConsequences(Dictionary<Guid, List<TreatmentConsequenceDTO>> treatmentConsequencePerTreatmentId,
            Guid simulationId)
        {
            var treatmentConsequences = treatmentConsequencePerTreatmentId.SelectMany(_ => _.Value.ToList()).ToList();

            var attributeEntities = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
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

            var scenarioConditionalTreatmentConsequenceEntities = treatmentConsequencePerTreatmentId
                .SelectMany(_ => _.Value.Select(consequence => consequence
                    .ToScenarioEntity(_.Key, attributeEntities
                        .Single(attribute => attribute.Name == consequence.Attribute).Id)))
                .ToList();

            var entityIds = scenarioConditionalTreatmentConsequenceEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioConditionalTreatmentConsequences
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == simulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioConditionalTreatmentConsequenceEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == simulationId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(scenarioConditionalTreatmentConsequenceEntities
                .Where(_ => existingEntityIds.Contains(_.Id)).ToList());

            _unitOfWork.Context.AddAll(scenarioConditionalTreatmentConsequenceEntities
                .Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

            if (treatmentConsequences.Any(_ =>
                _.Equation?.Id != null && _.Equation?.Id != Guid.Empty && !string.IsNullOrEmpty(_.Equation.Expression)))
            {
                var equationJoins = new List<ScenarioConditionalTreatmentConsequenceEquationEntity>();

                var equations = treatmentConsequences
                    .Where(_ => _.Equation?.Id != null && _.Equation?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.Equation.Expression))
                    .Select(consequence =>
                    {
                        var equationEntity = new EquationEntity
                        {
                            Id = Guid.NewGuid(),
                            Expression = consequence.Equation.Expression,
                        };
                        equationJoins.Add(new ScenarioConditionalTreatmentConsequenceEquationEntity
                        {
                            EquationId = equationEntity.Id,
                            ScenarioConditionalTreatmentConsequenceId = consequence.Id
                        });
                        return equationEntity;
                    }).ToList();

                // Delete any existing entries for related treatment consequences               
                _unitOfWork.Context.DeleteAll<ScenarioConditionalTreatmentConsequenceEquationEntity>(_ => existingEntityIds.Contains(_.ScenarioConditionalTreatmentConsequenceId));

                _unitOfWork.Context.AddAll(equations, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(equationJoins, _unitOfWork.UserEntity?.Id);
            }

            if (treatmentConsequences.Any(_ =>
                _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>();

                var criteria = treatmentConsequences
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(treatment =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = treatment.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{treatment} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioConditionalTreatmentConsequenceEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            ScenarioConditionalTreatmentConsequenceId = treatment.Id
                        });
                        return criterion;
                    }).ToList();

                // Delete any existing entries for related treatment consequences               
                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(_ => existingEntityIds.Contains(_.ScenarioConditionalTreatmentConsequenceId));

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public List<TreatmentConsequenceDTO> GetTreatmentConsequencesByLibraryIdAndTreatmentName(Guid treatmentLibraryId, string treatmentName)
        {
            var treatmentConsequences = _unitOfWork.Context.SelectableTreatment.AsNoTracking()
                .Include(_ => _.TreatmentConsequences)
                .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.TreatmentConsequences)
            .ThenInclude(_ => _.Attribute)
                .FirstOrDefault(_ => _.Name == treatmentName && _.TreatmentLibraryId == treatmentLibraryId)?.TreatmentConsequences?.ToList();
            if (treatmentConsequences == null)
            {
                return new List<TreatmentConsequenceDTO>();
            }
            return treatmentConsequences.Select(tc => tc.ToDto())?.ToList();
        }
    }
}
