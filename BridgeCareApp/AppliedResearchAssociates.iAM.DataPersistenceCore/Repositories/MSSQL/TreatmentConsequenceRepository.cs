﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
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

        private readonly IEquationRepository _equationRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IAMContext _context;

        public TreatmentConsequenceRepository(IEquationRepository equationRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            IAMContext context)
        {
            _equationRepo = equationRepo ?? throw new ArgumentNullException(nameof(equationRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateTreatmentConsequences(Dictionary<Guid, List<ConditionalTreatmentConsequence>> consequencesPerTreatmentId, string simulationName)
        {
            var consequenceEntities = new List<ConditionalTreatmentConsequenceEntity>();
            var equationEntityPerConsequenceEntityId = new Dictionary<Guid, EquationEntity>();
            var consequenceEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            var attributeNames = consequencesPerTreatmentId.Values
                .SelectMany(_ => _.Select(__ => __.Attribute.Name).Distinct()).ToList();
            var attributeEntities = _context.Attribute
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
                _context.TreatmentConsequence.AddRange(consequenceEntities);
            }
            else
            {
                _context.BulkInsert(consequenceEntities);
            }

            if (equationEntityPerConsequenceEntityId.Values.Any())
            {
                _equationRepo.CreateEquations(equationEntityPerConsequenceEntityId, "TreatmentConsequenceEntity");
            }

            if (consequenceEntityIdsPerExpression.Values.Any())
            {
                _criterionLibraryRepo.JoinEntitiesWithCriteria(consequenceEntityIdsPerExpression,
                    "TreatmentConsequenceEntity", simulationName);
            }
        }
    }
}
