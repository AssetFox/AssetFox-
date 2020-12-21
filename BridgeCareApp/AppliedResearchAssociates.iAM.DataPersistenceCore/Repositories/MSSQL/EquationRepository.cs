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

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public EquationRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerEntityId, string joinEntity)
        {
            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Equation.AddRange(equationEntityPerEntityId.Values.ToList());
            }
            else
            {
                _unitOfWork.Context.BulkInsert(equationEntityPerEntityId.Values.ToList());
            }

            _unitOfWork.Context.SaveChanges();

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

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            if (IsRunningFromXUnit)
            {
                equationEntities.ForEach(entity => _unitOfWork.Context.AddOrUpdate(entity, entity.Id));
            }
            else
            {
                _unitOfWork.Context.BulkInsertOrUpdate(equationEntities);
            }

            _unitOfWork.Context.SaveChanges();
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.PerformanceCurveEquation.AddRange(performanceCurveEquationJoinEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(performanceCurveEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.TreatmentConsequenceEquation.AddRange(treatmentConsequenceEquationJoinEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(treatmentConsequenceEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerEntityId)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.TreatmentCostEquation.AddRange(treatmentCostEquationJoinEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(treatmentCostEquationJoinEntities);
            }
        }
    }
}
