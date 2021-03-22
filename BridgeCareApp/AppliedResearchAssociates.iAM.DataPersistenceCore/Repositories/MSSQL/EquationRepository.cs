using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class EquationRepository : IEquationRepository
    {
        private static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public EquationRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, string joinEntity, Guid? userId = null)
        {
            _unitOfDataPersistenceWork.Context.AddAll(equationEntityPerJoinEntityId.Values.ToList(), userId);

            switch (joinEntity)
            {
            case DataPersistenceConstants.EquationJoinEntities.PerformanceCurve:
                JoinEquationsWithPerformanceCurves(equationEntityPerJoinEntityId, userId);
                break;

            case DataPersistenceConstants.EquationJoinEntities.TreatmentConsequence:
                JoinEquationsWithTreatmentConsequences(equationEntityPerJoinEntityId, userId);
                break;

            case DataPersistenceConstants.EquationJoinEntities.TreatmentCost:
                JoinEquationsWithTreatmentCosts(equationEntityPerJoinEntityId, userId);
                break;

            default:
                throw new InvalidOperationException("Unable to determine equation join entity type.");
            }
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            if (IsRunningFromXUnit)
            {
                equationEntities.ForEach(entity => _unitOfDataPersistenceWork.Context.Upsert(entity, entity.Id));
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsertOrUpdate(equationEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, Guid? userId = null)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            _unitOfDataPersistenceWork.Context.AddAll(performanceCurveEquationJoinEntities, userId);
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, Guid? userId = null)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            _unitOfDataPersistenceWork.Context.AddAll(treatmentConsequenceEquationJoinEntities, userId);
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, Guid? userId = null)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            _unitOfDataPersistenceWork.Context.AddAll(treatmentCostEquationJoinEntities, userId);
        }
    }
}
