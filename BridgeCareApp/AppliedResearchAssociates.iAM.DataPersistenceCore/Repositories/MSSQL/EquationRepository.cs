﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public EquationRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateEquations(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId, string joinEntity)
        {
            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.Equation.AddRange(equationEntityPerJoinEntityId.Values.ToList());
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(equationEntityPerJoinEntityId.Values.ToList());
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            switch (joinEntity)
            {
            case "PerformanceCurveEntity":
                JoinEquationsWithPerformanceCurves(equationEntityPerJoinEntityId);
                break;
            case "TreatmentConsequenceEntity":
                JoinEquationsWithTreatmentConsequences(equationEntityPerJoinEntityId);
                break;
            case "TreatmentCostEntity":
                JoinEquationsWithTreatmentCosts(equationEntityPerJoinEntityId);
                break;
            default:
                throw new InvalidOperationException("Unable to determine equation join entity type.");
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateEquations(List<EquationEntity> equationEntities)
        {
            if (IsRunningFromXUnit)
            {
                equationEntities.ForEach(entity => _unitOfDataPersistenceWork.Context.AddOrUpdate(entity, entity.Id));
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsertOrUpdate(equationEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void JoinEquationsWithPerformanceCurves(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var performanceCurveEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new PerformanceCurveEquationEntity { EquationId = _.Value.Id, PerformanceCurveId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.PerformanceCurveEquation.AddRange(performanceCurveEquationJoinEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(performanceCurveEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentConsequences(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var treatmentConsequenceEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new ConditionalTreatmentConsequenceEquationEntity { EquationId = _.Value.Id, ConditionalTreatmentConsequenceId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentConsequenceEquation.AddRange(treatmentConsequenceEquationJoinEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(treatmentConsequenceEquationJoinEntities);
            }
        }

        private void JoinEquationsWithTreatmentCosts(Dictionary<Guid, EquationEntity> equationEntityPerJoinEntityId)
        {
            var treatmentCostEquationJoinEntities = equationEntityPerJoinEntityId
                .Select(_ => new TreatmentCostEquationEntity { EquationId = _.Value.Id, TreatmentCostId = _.Key })
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.TreatmentCostEquation.AddRange(treatmentCostEquationJoinEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(treatmentCostEquationJoinEntities);
            }
        }
    }
}
