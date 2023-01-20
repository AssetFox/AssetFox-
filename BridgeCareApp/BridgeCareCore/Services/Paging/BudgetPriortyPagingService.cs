﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using System;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.Paging.Generics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BridgeCareCore.Services
{
    public class BudgetPriortyPagingService : PagingService<BudgetPriorityDTO, BudgetPriorityLibraryDTO>,  IBudgetPriortyPagingService
    {
        private static IUnitOfWork _unitOfWork;

        public BudgetPriortyPagingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        
        public override List<BudgetPriorityDTO> GetSyncedScenarioDataSet(Guid simulationId, PagingSyncModel<BudgetPriorityDTO> request)
        {
            var rows = request.LibraryId == null ?
                    _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId) :
                    _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(request.LibraryId.Value);
            rows =  SyncDataset(rows, request);

            if(request.LibraryId != null)
                rows.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.CriterionLibrary.Id = Guid.NewGuid();
                });
            var budgets = _unitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulationId);
            //this gets rid of percentage pairs that shouldn't be there and adds the ones that should
            rows.ForEach(row =>
            {
                row.BudgetPercentagePairs = row.BudgetPercentagePairs.Where(_ => budgets.Any(__ => __.Name == _.BudgetName)).ToList();
                budgets.ForEach(_ =>
                {
                    if (!row.BudgetPercentagePairs.Any(__ => __.BudgetName == _.Name))
                        row.BudgetPercentagePairs.Add(new BudgetPercentagePairDTO()
                        {
                            Id = Guid.NewGuid(),
                            BudgetId = _.Id,
                            BudgetName = _.Name,
                            Percentage = 100
                        });
                });
            });
            return rows;
        }


        override protected List<BudgetPriorityDTO> OrderByColumn(List<BudgetPriorityDTO> rows, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower();
            switch (sortColumn)
            {
            case "prioritylevel":
                if (isDescending)
                    return rows.OrderByDescending(_ => _.PriorityLevel).ToList();
                else
                    return rows.OrderBy(_ => _.PriorityLevel).ToList();
            default://This is sorting the budget priorities by a given budget name
                if (isDescending)
                {
                    return rows.OrderByDescending(_ =>
                    {
                        var pair = _.BudgetPercentagePairs.FirstOrDefault(__ => __.BudgetName.ToLower() == sortColumn);
                        return pair != null ? pair.Percentage : 0;
                    }).ToList();
                }
                else
                {
                    return rows.OrderBy(_ =>
                    {
                        var pair = _.BudgetPercentagePairs.FirstOrDefault(__ => __.BudgetName.ToLower() == sortColumn);
                        return pair != null ? pair.Percentage : 0;
                    }).ToList();
                }
            }
        }

        override protected List<BudgetPriorityDTO> SearchRows(List<BudgetPriorityDTO> rows, string search)
        {
            return rows
                .Where(_ => _.PriorityLevel.ToString().Contains(search) ||
                    _.Year.ToString().Contains(search) ||
                    (_.CriterionLibrary.MergedCriteriaExpression != null && _.CriterionLibrary.MergedCriteriaExpression.ToLower().Contains(search))).ToList();
        }

        protected override List<BudgetPriorityDTO> GetScenarioRows(Guid scenarioId) => _unitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(scenarioId);

        protected override List<BudgetPriorityDTO> GetLibraryRows(Guid libraryId) => _unitOfWork.BudgetPriorityRepo.GetBudgetPrioritiesByLibraryId(libraryId);
        protected override List<BudgetPriorityDTO> CreateAsNewDataset(List<BudgetPriorityDTO> rows)
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
