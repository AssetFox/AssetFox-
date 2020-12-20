using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class EquationRepository : IEquationRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly IAMContext _context;

        public EquationRepository(IAMContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerEntityId, string joinEntity)
        {
            if (IsRunningFromXUnit)
            {
                _context.Equation.AddRange(equationEntityPerEntityId.Values.ToList());
            }
            else
            {
                _context.BulkInsert(equationEntityPerEntityId.Values.ToList());
            }

            switch (joinEntity)
            {
            case "PerformanceCurveEntity":
                JoinEquationsWithPerformanceCurves(equationEntityPerEntityId);
                break;
            case "TreatmentConsequenceEntity":
                JoinEquationsWithTreatmentConsequences(equationEntityPerEntityId);
                break;
            case "TreatmentCostEntity":
                JoinEquationsWithTreatmentCosts(equationEntityPerEntityId);
                break;
            default:
                throw new InvalidOperationException("Unable to determine equation join entity type.");
            }
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            if (IsRunningFromXUnit)
            {
                equationEntities.ForEach(entity => _context.AddOrUpdate(entity, entity.Id));
            }
            else
            {
                _context.BulkInsertOrUpdate(equationEntities);
            }
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.PerformanceCurveEquation.AddRange(performanceCurveEquationJoinEntities);
            }
            else
            {
                _context.BulkInsert(performanceCurveEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.TreatmentConsequenceEquation.AddRange(treatmentConsequenceEquationJoinEntities);
            }
            else
            {
                _context.BulkInsert(treatmentConsequenceEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.TreatmentCostEquation.AddRange(treatmentCostEquationJoinEntities);
            }
            else
            {
                _context.BulkInsert(treatmentCostEquationJoinEntities);
            }
        }
    }
}
