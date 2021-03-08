using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : IAttributeRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public AttributeRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            if (IsRunningFromXUnit)
            {
                attributes.ForEach(_ =>
                {
                    var entity = _.ToEntity();
                    _unitOfDataPersistenceWork.Context.Upsert(entity, entity.Id);
                });
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsertOrUpdate(attributes.Select(_ => _.ToEntity()).ToList());
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void JoinAttributesWithEquationsAndCriteria(Explorer explorer)
        {
            var calculatedFieldNames = explorer.CalculatedFields.Select(_ => _.Name).ToList();

            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute
                .Where(_ => calculatedFieldNames.Contains(_.Name)).ToList();

            var existingJoins = _unitOfDataPersistenceWork.Context.AttributeEquationCriterionLibrary
                .Include(_ => _.Attribute)
                .Include(_ => _.Equation)
                .Include(_ => _.CriterionLibrary)
                .ToList()
                .Select(_ => (attributeName: _.Attribute.Name, equationExpression: _.Equation.Expression, criterionExpression: _.CriterionLibrary?.MergedCriteriaExpression ?? string.Empty))
                .ToList();

            var joinEntities = new List<AttributeEquationCriterionLibraryEntity>();
            var equationEntities = new List<EquationEntity>();
            var criterionLibraryEntities = new List<CriterionLibraryEntity>();

            attributeEntities.ForEach(attributeEntity =>
            {
                var calculatedField = explorer.CalculatedFields.Single(_ => _.Name == attributeEntity.Name);
                calculatedField.ValueSources.ForEach(valueSource =>
                {
                    var joinTuple = (attributeName: attributeEntity.Name, equationExpression: valueSource.Equation.Expression, criterionExpression: valueSource.Criterion.Expression ?? string.Empty);

                    if (!existingJoins.Contains(joinTuple))
                    {
                        var newJoinEntity = new AttributeEquationCriterionLibraryEntity { AttributeId = attributeEntity.Id, };

                        var equationEntity =
                            _unitOfDataPersistenceWork.Context.Equation.SingleOrDefault(_ =>
                                _.Expression == valueSource.Equation.Expression);

                        if (equationEntity == null)
                        {
                            equationEntity = valueSource.Equation.ToEntity();
                            equationEntities.Add(equationEntity);
                        }

                        newJoinEntity.EquationId = equationEntity.Id;

                        if (!valueSource.Criterion.ExpressionIsBlank)
                        {
                            var criterionLibraryEntity = _unitOfDataPersistenceWork.Context.CriterionLibrary.Any(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression)
                                ? _unitOfDataPersistenceWork.Context.CriterionLibrary.Single(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression)
                                : GetCriterionLibraryEntity(valueSource.Criterion.Expression, criterionLibraryEntities);

                            newJoinEntity.CriterionLibraryId = criterionLibraryEntity.Id;
                        }

                        joinEntities.Add(newJoinEntity);
                    }
                });
            });

            if (equationEntities.Any())
            {
                _unitOfDataPersistenceWork.EquationRepo.CreateEquations(equationEntities);
            }

            if (criterionLibraryEntities.Any())
            {
                _unitOfDataPersistenceWork.CriterionLibraryRepo.CreateCriterionLibraries(criterionLibraryEntities);
            }

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.AttributeEquationCriterionLibrary.AddRange(joinEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(joinEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        private CriterionLibraryEntity GetCriterionLibraryEntity(string expression, List<CriterionLibraryEntity> criterionLibraryEntities)
        {
            var criterionLibraryEntity = _unitOfDataPersistenceWork.Context.CriterionLibrary
                .SingleOrDefault(_ => _.MergedCriteriaExpression == expression && _.Name.Contains("Explorer Attribute"));

            if (criterionLibraryEntity == null)
            {
                var criterionLibraryNames = _unitOfDataPersistenceWork.Context.CriterionLibrary
                    .Where(_ => _.Name.Contains("Explorer Attribute"))
                    .Select(_ => _.Name).ToList();

                var newCriterionLibraryName = $"Explorer Attribute Criterion Library";
                if (criterionLibraryNames.Contains(newCriterionLibraryName))
                {
                    var version = 2;
                    while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                    {
                        version++;
                    }
                    newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
                }

                criterionLibraryEntity = new CriterionLibraryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = newCriterionLibraryName,
                    MergedCriteriaExpression = expression
                };
            }

            criterionLibraryEntities.Add(criterionLibraryEntity);

            return criterionLibraryEntity;
        }

        public Explorer GetExplorer()
        {
            if (!_unitOfDataPersistenceWork.Context.Attribute.Any())
            {
                throw new RowNotInTableException("Found no attributes.");
            }

            var attributes = _unitOfDataPersistenceWork.Context.Attribute
                .Include(_ => _.AttributeEquationCriterionLibraryJoins)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.AttributeEquationCriterionLibraryJoins)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.Name != "AGE")
                .ToList();

            var explorer = new Explorer();

            attributes.ForEach(entity =>
            {
                var simulationAnalysisDomainAttribute = entity.ToSimulationAnalysisDomain();
                if (simulationAnalysisDomainAttribute is NumberAttribute numberAttribute)
                {
                    if (entity.IsCalculated)
                    {
                        var calculatedField = explorer.AddCalculatedField(entity.Name);
                        calculatedField.IsDecreasingWithDeterioration = entity.IsAscending;

                        if (entity.AttributeEquationCriterionLibraryJoins.Any())
                        {
                            entity.AttributeEquationCriterionLibraryJoins.ForEach(join =>
                            {
                                var source = calculatedField.AddValueSource();
                                source.Equation.Expression = join.Equation.Expression;
                                source.Criterion.Expression = join.CriterionLibrary?.MergedCriteriaExpression ?? string.Empty;
                            });
                        }
                    }
                    else
                    {
                        var attribute = explorer.AddNumberAttribute(numberAttribute.Name);
                        attribute.IsDecreasingWithDeterioration = numberAttribute.IsDecreasingWithDeterioration;
                        attribute.DefaultValue = numberAttribute.DefaultValue;
                        attribute.Minimum = numberAttribute.Minimum;
                        attribute.Maximum = numberAttribute.Maximum;
                    }
                }
                else if (simulationAnalysisDomainAttribute is TextAttribute textAttribute)
                {
                    var attribute = explorer.AddTextAttribute(textAttribute.Name);
                    attribute.DefaultValue = textAttribute.DefaultValue;
                }
            });

            return explorer;
        }
    }
}
