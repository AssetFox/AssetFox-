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
    public class EquationRepository : MSSQLRepository, IEquationRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public EquationRepository(IAMContext context) : base(context) { }

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerEntityId, string joinEntity)
        {
            if (IsRunningFromXUnit)
            {
                Context.Equation.AddRange(equationEntityPerEntityId.Values.ToList());
            }
            else
            {
                Context.BulkInsert(equationEntityPerEntityId.Values.ToList());
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

            Context.SaveChanges();
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            if (IsRunningFromXUnit)
            {
                equationEntities.ForEach(entity => Context.AddOrUpdate(entity, entity.Id));
            }
            else
            {
                Context.BulkInsertOrUpdate(equationEntities);
            }

            Context.SaveChanges();
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.PerformanceCurveEquation.AddRange(performanceCurveEquationJoinEntities);
            }
            else
            {
                Context.BulkInsert(performanceCurveEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.TreatmentConsequenceEquation.AddRange(treatmentConsequenceEquationJoinEntities);
            }
            else
            {
                Context.BulkInsert(treatmentConsequenceEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.TreatmentCostEquation.AddRange(treatmentCostEquationJoinEntities);
            }
            else
            {
                Context.BulkInsert(treatmentCostEquationJoinEntities);
            }
        }
    }
}
