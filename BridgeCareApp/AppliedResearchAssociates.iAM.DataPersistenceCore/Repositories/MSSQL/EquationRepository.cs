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

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public EquationRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, string joinEntity)
        {
            _unitOfWork.Context.AddAll(equationEntityPerJoinEntityId.Values.ToList(), _unitOfWork.UserEntity?.Id);

            switch (joinEntity)
            {
            case DataPersistenceConstants.EquationJoinEntities.PerformanceCurve:
                JoinEquationsWithPerformanceCurves(equationEntityPerJoinEntityId);
                break;

            case DataPersistenceConstants.EquationJoinEntities.TreatmentConsequence:
                JoinEquationsWithTreatmentConsequences(equationEntityPerJoinEntityId);
                break;

            case DataPersistenceConstants.EquationJoinEntities.TreatmentCost:
                JoinEquationsWithTreatmentCosts(equationEntityPerJoinEntityId);
                break;

            default:
                throw new InvalidOperationException("Unable to determine equation join entity type.");
            }
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            _unitOfWork.Context.AddAll(equationEntities);

            _unitOfWork.Context.SaveChanges();
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            _unitOfWork.Context.AddAll(performanceCurveEquationJoinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            _unitOfWork.Context.AddAll(treatmentConsequenceEquationJoinEntities, _unitOfWork.UserEntity?.Id);
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            _unitOfWork.Context.AddAll(treatmentCostEquationJoinEntities, _unitOfWork.UserEntity?.Id);
        }
    }
}
