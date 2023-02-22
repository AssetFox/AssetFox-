using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Services
{
    public class TreatmentPagingService : PagingService<TreatmentDTO, TreatmentLibraryDTO>, ITreatmentPagingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private void FixNullLists(TreatmentDTO dto)
        {
            dto.Consequences ??= new List<TreatmentConsequenceDTO>();
            dto.Costs ??= new List<TreatmentCostDTO>();
        }

        public override List<TreatmentDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<TreatmentDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId) :
                    _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(request.LibraryId.Value);
            rows = SyncDataset(rows, request);

            if (request.LibraryId != null)
            {
                var budgets = _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId);
                var budgetIds = budgets.Select(_ => _.Id).ToList();

                rows.ForEach(_ =>
                {
                    FixNullLists(_);
                    _.Id = Guid.NewGuid();
                    if (_.CriterionLibrary != null)
                    {
                        _.CriterionLibrary.Id = Guid.NewGuid();
                    }
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        if (__.CriterionLibrary != null)
                        {
                            __.CriterionLibrary.Id = Guid.NewGuid();
                        }
                        if (__.Equation != null)
                        {
                            __.Equation.Id = Guid.NewGuid();
                        }
                    });
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        if (__.Equation != null)
                        {
                            __.Equation.Id = Guid.NewGuid();
                        }
                        if (__.CriterionLibrary != null)
                        {
                            __.CriterionLibrary.Id = Guid.NewGuid();
                        }
                    });
                    if (_.BudgetIds == null || !_.BudgetIds.Any())
                    {
                        _.BudgetIds = budgetIds;
                    }
                });
            }
            return rows;
        }

        protected override List<TreatmentDTO> GetScenarioRows(Guid scenarioId) => _unitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(scenarioId);

        protected override List<TreatmentDTO> GetLibraryRows(Guid libraryId) => _unitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId);
        protected override List<TreatmentDTO> CreateAsNewDataset(List<TreatmentDTO> rows)
        {
            rows.ForEach(_ =>
            {
                _.Id = Guid.NewGuid();
                if (_.CriterionLibrary != null)
                {
                    _.CriterionLibrary.Id = Guid.NewGuid();
                }
                if (_.Consequences != null)
                {
                    _.Consequences.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        if (__.CriterionLibrary != null)
                        {
                            __.CriterionLibrary.Id = Guid.NewGuid();
                        }
                        if (__.Equation != null)
                        {
                            __.Equation.Id = Guid.NewGuid();
                        }
                    });
                }
                if (_.Costs != null)
                {
                    _.Costs.ForEach(__ =>
                    {
                        __.Id = Guid.NewGuid();
                        if (__.Equation != null)
                        {
                            __.Equation.Id = Guid.NewGuid();
                        }
                        if (__.CriterionLibrary != null)
                        {
                            __.CriterionLibrary.Id = Guid.NewGuid();
                        }
                    });
                }
            });

            return rows;
        }
    }
}
