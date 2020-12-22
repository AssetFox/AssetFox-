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

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public AttributeRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void UpsertAttributes(List<DataMinerAttribute> attributes)
        {
            if (IsRunningFromXUnit)
            {
                attributes.ForEach(_ =>
                {
                    var entity = _.ToEntity();
                    _unitOfWork.Context.AddOrUpdate(entity, entity.Id);
                });
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(attributes.Select(_ => _.ToEntity()).ToList());
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void JoinAttributesWithEquationsAndCriteria(Explorer explorer)
        {
            var calculatedFieldNames = explorer.CalculatedFields.Select(_ => _.Name).ToList();

            var attributeEntities = _unitOfWork.Context.Attribute
                .Where(_ => calculatedFieldNames.Contains(_.Name)).ToList();

            var joinEntities = new List<AttributeEquationCriterionLibraryEntity>();
            var equationEntities = new List<EquationEntity>();
            var criterionLibraryEntities = new List<CriterionLibraryEntity>();

            attributeEntities.ForEach(attributeEntity =>
            {
                var calculatedField = explorer.CalculatedFields.Single(_ => _.Name == attributeEntity.Name);
                calculatedField.ValueSources.ForEach(valueSource =>
                {
                    var joinEntity = new AttributeEquationCriterionLibraryEntity { AttributeId = attributeEntity.Id };

                    var equationEntity = valueSource.Equation.ToEntity();
                    equationEntities.Add(equationEntity);

                    joinEntity.EquationId = equationEntity.Id;

                    if (!valueSource.Criterion.ExpressionIsBlank)
                    {
                        var criterionLibraryEntity = criterionLibraryEntities.Any(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression)
                            ? criterionLibraryEntities.Single(_ => _.MergedCriteriaExpression == valueSource.Criterion.Expression)
                            : GetCriterionLibraryEntity(valueSource.Criterion.Expression, criterionLibraryEntities);

                        joinEntity.CriterionLibraryId = criterionLibraryEntity.Id;
                    }

                    joinEntities.Add(joinEntity);
                });
            });

            if (equationEntities.Any())
            {
                _unitOfWork.EquationRepo.CreateEquations(equationEntities);
            }

            if (criterionLibraryEntities.Any())
            {
                _unitOfWork.CriterionLibraryRepo.CreateCriterionLibraries(criterionLibraryEntities);
            }

            if (IsRunningFromXUnit)
            {
                joinEntities.ForEach(entity => _unitOfWork.Context.AddOrUpdate(entity));
                _unitOfWork.Context.AttributeEquationCriterionLibrary.AddRange(joinEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(joinEntities);
            }

            _unitOfWork.Context.SaveChanges();
        }

        private CriterionLibraryEntity GetCriterionLibraryEntity(string expression, List<CriterionLibraryEntity> criterionLibraryEntities)
        {
            var criterionLibraryEntity = _unitOfWork.Context.CriterionLibrary
                .SingleOrDefault(_ => _.MergedCriteriaExpression == expression && _.Name.Contains("Attribute"));

            if (criterionLibraryEntity == null)
            {
                var criterionLibraryNames = _unitOfWork.Context.CriterionLibrary
                    .Where(_ => _.Name.Contains("Attribute"))
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
                                source.Criterion.Expression = join.CriterionLibrary?.MergedCriteriaExpression;
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
