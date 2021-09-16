using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AttributeRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            var attributeEntities = attributes.Select(_ => _.ToEntity()).ToList();
            var existingAttributeIds = _unitOfWork.Context.Attribute.Select(_ => _.Id).ToList();

            var entitiesToUpdate = attributeEntities.Where(_ => existingAttributeIds.Contains(_.Id)).ToList();
            var entitiesToAdd = attributeEntities.Where(_ => !existingAttributeIds.Contains(_.Id)).ToList();

            _unitOfWork.Context.UpdateAll(entitiesToUpdate, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.AddAll(entitiesToAdd, _unitOfWork.UserEntity?.Id);
        }

        public void JoinAttributesWithEquationsAndCriteria(Explorer explorer)
        {
            var calculatedFieldNames = explorer.CalculatedFields.Select(_ => _.Name).ToList();

            var attributeEntities = _unitOfWork.Context.Attribute
                .Where(_ => calculatedFieldNames.Contains(_.Name)).ToList();

            var existingJoins = _unitOfWork.Context.AttributeEquationCriterionLibrary
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
                    var doesNotHaveCurrentTuple = !existingJoins.Any(_ => _.attributeName == joinTuple.attributeName &&
                                                                          _.equationExpression == joinTuple.equationExpression &&
                                                                          _.criterionExpression == joinTuple.criterionExpression);
                    if (doesNotHaveCurrentTuple)
                    {
                        var newJoinEntity = new AttributeEquationCriterionLibraryEntity { AttributeId = attributeEntity.Id, };

                        var equationEntity = valueSource.Equation.ToEntity();
                        equationEntities.Add(equationEntity);

                        newJoinEntity.EquationId = equationEntity.Id;

                        if (!valueSource.Criterion.ExpressionIsBlank)
                        {
                            var criterionLibraryEntity = _unitOfWork.Context.CriterionLibrary
                                .Any(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression && _.Name.Contains("Explorer Attribute Criterion Library"))
                                ? _unitOfWork.Context.CriterionLibrary
                                    .First(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression && _.Name.Contains("Explorer Attribute Criterion Library"))
                                : GetCriterionLibraryEntity(valueSource.Criterion.Expression, criterionLibraryEntities);

                            newJoinEntity.CriterionLibraryId = criterionLibraryEntity.Id;
                        }

                        joinEntities.Add(newJoinEntity);
                    }
                });
            });

            if (equationEntities.Any())
            {
                _unitOfWork.Context.AddAll(equationEntities, _unitOfWork.UserEntity?.Id);
            }

            if (criterionLibraryEntities.Any())
            {
                _unitOfWork.Context.AddAll(criterionLibraryEntities, _unitOfWork.UserEntity?.Id);
            }

            _unitOfWork.Context.AddAll(joinEntities, _unitOfWork.UserEntity?.Id);
        }

        private CriterionLibraryEntity GetCriterionLibraryEntity(string expression, List<CriterionLibraryEntity> criterionLibraryEntities)
        {
            var criterionLibraryNames = _unitOfWork.Context.CriterionLibrary
                .Where(_ => _.Name.Contains("Explorer Attribute"))
                .Select(_ => _.Name).ToList();

            var newCriterionLibraryName = "Explorer Attribute Criterion Library";
            if (criterionLibraryNames.Contains(newCriterionLibraryName))
            {
                var version = 2;
                while (criterionLibraryNames.Contains($"{newCriterionLibraryName} v{version}"))
                {
                    version++;
                }
                newCriterionLibraryName = $"{newCriterionLibraryName} v{version}";
            }

            var criterionLibraryEntity = new CriterionLibraryEntity
            {
                Id = Guid.NewGuid(),
                Name = newCriterionLibraryName,
                MergedCriteriaExpression = expression
            };

            criterionLibraryEntities.Add(criterionLibraryEntity);

            return criterionLibraryEntity;
        }

        public Explorer GetExplorer()
        {
            if (!_unitOfWork.Context.Attribute.Any())
            {
                throw new RowNotInTableException("Found no attributes.");
            }

            var attributes = _unitOfWork.Context.Attribute
                .Include(_ => _.AttributeEquationCriterionLibraryJoins)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.AttributeEquationCriterionLibraryJoins)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.Name != "AGE")
                .AsNoTracking()
                .ToList();

            var explorer = new Explorer();

            attributes.ForEach(entity =>
            {
                if (entity.DataType == "NUMBER")
                {
                    if (entity.IsCalculated)
                    {
                        var calculatedField = explorer.AddCalculatedField(entity.Name);
                        calculatedField.IsDecreasingWithDeterioration = entity.IsAscending;
                        calculatedField.Timing = CalculatedFieldTiming.NotSpecified;

                        // TODO:  Remove this once the simulation object is built
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
                        var numAttribute = explorer.AddNumberAttribute(entity.Name);
                        numAttribute.IsDecreasingWithDeterioration = entity.IsAscending;
                        numAttribute.DefaultValue = Convert.ToDouble(entity.DefaultValue);
                        numAttribute.Maximum = entity.Maximum;
                        numAttribute.Minimum = entity.Minimum;
                    }
                }

                else if (entity.DataType == "STRING")
                {
                    var textAttribute = explorer.AddTextAttribute(entity.Name);

                    textAttribute.DefaultValue = entity.DefaultValue;
                }
                else
                {
                    throw new InvalidOperationException("Cannot determine Attribute entity data type");
                }
            });

            return explorer;
        }

        public Task<List<AttributeDTO>> Attributes()
        {
            if (!_unitOfWork.Context.Attribute.Any())
            {
                throw new RowNotInTableException("Found no attributes.");
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.Attribute.OrderBy(_ => _.Name).Select(_ => _.ToDto()).ToList());
        }

        public Task<List<AttributeDTO>> CalculatedAttributes()
        {
            if (!_unitOfWork.Context.Attribute.Any())
            {
                throw new RowNotInTableException("Found no attributes.");
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.Attribute.Where(_ => _.IsCalculated).OrderBy(_ => _.Name).Select(_ => _.ToDto()).ToList());
        }
    }
}
