﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentCostRepository : ITreatmentCostRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public TreatmentCostRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateTreatmentCosts(Dictionary<Guid, List<TreatmentCost>> treatmentCostsPerTreatmentId, string simulationName)
        {
            var costEntities = new List<TreatmentCostEntity>();
            var equationEntityPerCostEntityId = new Dictionary<Guid, EquationEntity>();
            var costEntityIdsPerExpression = new Dictionary<string, List<Guid>>();

            treatmentCostsPerTreatmentId.Keys.ForEach(treatmentId =>
            {
                var entities = treatmentCostsPerTreatmentId[treatmentId].Select(_ =>
                    {
                        var costEntity = _.ToEntity(treatmentId);

                        if (!_.Equation.ExpressionIsBlank)
                        {
                            equationEntityPerCostEntityId.Add(costEntity.Id, _.Equation.ToEntity());
                        }

                        if (!_.Criterion.ExpressionIsBlank)
                        {
                            if (costEntityIdsPerExpression.ContainsKey(_.Criterion.Expression))
                            {
                                costEntityIdsPerExpression[_.Criterion.Expression].Add(costEntity.Id);
                            }
                            else
                            {
                                costEntityIdsPerExpression.Add(_.Criterion.Expression, new List<Guid>
                                {
                                    costEntity.Id
                                });
                            }
                        }

                        return costEntity;
                    })
                    .ToList();

                costEntities.AddRange(entities);
            });

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.TreatmentCost.AddRange(costEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(costEntities);
            }

            _unitOfWork.Context.SaveChanges();

            if (equationEntityPerCostEntityId.Values.Any())
            {
                _unitOfWork.EquationRepo.CreateEquations(equationEntityPerCostEntityId, "TreatmentCostEntity");
            }

            if (costEntityIdsPerExpression.Values.Any())
            {
                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(costEntityIdsPerExpression,
                    "TreatmentCostEntity", simulationName);
            }
        }
    }
}