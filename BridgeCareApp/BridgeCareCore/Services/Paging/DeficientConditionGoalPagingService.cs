using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Services
{
    public class DeficientConditionGoalPagingService : PagingService<DeficientConditionGoalDTO, DeficientConditionGoalLibraryDTO>,  IDeficientConditionGoalPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public DeficientConditionGoalPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override List<DeficientConditionGoalDTO> GetScenarioRows(Guid scenarioId) => _unitOfWork.DeficientConditionGoalRepo.GetScenarioDeficientConditionGoals(scenarioId);

        protected override List<DeficientConditionGoalDTO> GetLibraryRows(Guid libraryId) => _unitOfWork.DeficientConditionGoalRepo.GetDeficientConditionGoalsByLibraryId(libraryId);
        protected override List<DeficientConditionGoalDTO> CreateAsNewDataset(List<DeficientConditionGoalDTO> rows)
        {
            rows.ForEach(_ =>
            {
                _.Id = Guid.NewGuid();
                _.CriterionLibrary.Id = Guid.NewGuid();
            });

            return rows;
        }
    }
}
