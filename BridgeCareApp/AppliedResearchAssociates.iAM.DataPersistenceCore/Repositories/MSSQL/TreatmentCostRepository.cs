using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentCostRepository : MSSQLRepository, ITreatmentCostRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly IEquationRepository _equationRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;

        public TreatmentCostRepository(IEquationRepository equationRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            IAMContext context) : base(context)
        {
            _equationRepo = equationRepo ?? throw new ArgumentNullException(nameof(equationRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
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
                Context.TreatmentCost.AddRange(costEntities);
            }
            else
            {
                Context.BulkInsert(costEntities);
            }

            if (equationEntityPerCostEntityId.Values.Any())
            {
                _equationRepo.CreateEquations(equationEntityPerCostEntityId, "TreatmentCostEntity");
            }

            if (costEntityIdsPerExpression.Values.Any())
            {
                _criterionLibraryRepo.JoinEntitiesWithCriteria(costEntityIdsPerExpression,
                    "TreatmentCostEntity", simulationName);
            }
        }
    }
}
