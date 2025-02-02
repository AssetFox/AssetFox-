﻿using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Services
{
    public class TargetConditionGoalPagingService : PagingService<TargetConditionGoalDTO, TargetConditionGoalLibraryDTO>,  ITargetConditionGoalPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public TargetConditionGoalPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override List<TargetConditionGoalDTO> GetScenarioRows(Guid scenarioId) => _unitOfWork.TargetConditionGoalRepo.GetScenarioTargetConditionGoals(scenarioId);

        protected override List<TargetConditionGoalDTO> GetLibraryRows(Guid libraryId) => _unitOfWork.TargetConditionGoalRepo.GetTargetConditionGoalsByLibraryId(libraryId);
        protected override List<TargetConditionGoalDTO> CreateAsNewDataset(List<TargetConditionGoalDTO> rows)
        {
            rows.ForEach(_ =>
            {
                _.Id = Guid.NewGuid();
                if (_.CriterionLibrary != null)
                {
                    _.CriterionLibrary.Id = Guid.NewGuid();
                }
            });

            return rows;
        }
    }
}
