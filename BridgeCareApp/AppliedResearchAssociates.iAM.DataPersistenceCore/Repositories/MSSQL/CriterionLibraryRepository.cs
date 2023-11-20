using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CriterionLibraryRepository : ICriterionLibraryRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CriterionLibraryRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public Task<List<CriterionLibraryDTO>> CriterionLibraries()
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any())
            {
                return Task.Factory.StartNew(() => new List<CriterionLibraryDTO>());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.CriterionLibrary.Where(_ => _.IsSingleUse == false).Select(_ => _.ToDto()).ToList());
        }

        public Task<CriterionLibraryDTO> CriteriaLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any(_ => _.Id == libraryId))
            {
                return Task.Factory.StartNew(() => new CriterionLibraryDTO());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.CriterionLibrary.First(_ => _.Id == libraryId).ToDto());
        }

        public Guid UpsertCriterionLibrary(CriterionLibraryDTO dto)
        {
            var entity = _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);

            return entity.Id;
        }

        public void DeleteCriterionLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CriterionLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<CriterionLibraryEntity>(_ => _.Id == libraryId);
        }

        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForSimulation(Guid simulationId, List<string> budgetNames)
        {
            _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                _.IsSingleUse && _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                    join.ScenarioBudget.SimulationId == simulationId &&
                    budgetNames.Contains(join.ScenarioBudget.Name)));
        }

        public void DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary(Guid budgetLibraryId, List<string> budgetNames)
        {
            _unitOfWork.Context.DeleteAll<CriterionLibraryEntity>(_ =>
                _.IsSingleUse && _.CriterionLibraryBudgetJoins.Any(join =>
                    join.Budget.BudgetLibraryId == budgetLibraryId &&
                    budgetNames.Contains(join.Budget.Name)));
        }

        public void AddLibraries(List<CriterionLibraryDTO> criteria)
        {
            var entities = criteria.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }

        public void AddLibraryScenarioBudgetJoins(List<CriterionLibraryScenarioBudgetDTO> criteriaJoins)
        {
            var entities = criteriaJoins.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }
        public void AddLibraryBudgetJoins(List<CriterionLibraryBudgetDTO> criteriaJoins)
        {
            var entities = criteriaJoins.Select(c => c.ToEntity()).ToList();
            _unitOfWork.Context.AddAll(entities, _unitOfWork.CurrentUser?.Id);
        }
    }
}
