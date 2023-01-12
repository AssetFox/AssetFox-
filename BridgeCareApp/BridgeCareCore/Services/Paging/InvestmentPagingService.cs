using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Interfaces;
using System.Linq;

namespace BridgeCareCore.Services.Paging
{
    public class InvestmentPagingService : IInvestmentPagingService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;

        public InvestmentPagingService(UnitOfDataPersistenceWork unitOfWork,
            IInvestmentDefaultDataService investmentDefaultDataService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
        }
        public InvestmentPagingPageModel GetLibraryPage(Guid libraryId, InvestmentPagingRequestModel request)
        {
            var skip = 0;
            var take = 0;
            var total = 0;
            var items = new List<BudgetDTO>();
            var lastYear = 0;

            var budgets = _unitOfWork.BudgetRepo.GetBudgetLibrary(libraryId).Budgets;


            budgets = SyncedDataset(budgets, request.SyncModel);



            if (request.sortColumn.Trim() != "")
                budgets = OrderByColumn(budgets, request.sortColumn, request.isDescending);
            if (budgets.Count > 0 && budgets[0].BudgetAmounts.Count > 0)
            {
                lastYear = budgets[0].BudgetAmounts.Max(_ => _.Year);
            }

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                total = budgets.Count != 0 ? budgets.First().BudgetAmounts.Count : 0;
                budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.Skip(skip).Take(take).ToList());
                items = budgets;
            }
            else
            {
                items = budgets;
                return new InvestmentPagingPageModel()
                {
                    Items = items,
                    TotalItems = total,
                    LastYear = lastYear,
                };
            }

            return new InvestmentPagingPageModel()
            {
                Items = items,
                TotalItems = total,
                LastYear = lastYear
            };
        }

        public InvestmentPagingPageModel GetScenarioPage(Guid simulationId, InvestmentPagingRequestModel request)
        {
            var skip = 0;
            var take = 0;
            var items = new List<BudgetDTO>();
            var total = 0;
            var lastYear = 0;
            var firstYear = 0;
            var investmentPlan = request.SyncModel.Investment == null ? _unitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId) : request.SyncModel.Investment;
            if (investmentPlan.Id == Guid.Empty)
            {
                var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                investmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                investmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
                if (investmentPlan.FirstYearOfAnalysisPeriod == 0)
                    investmentPlan.FirstYearOfAnalysisPeriod = DateTime.Now.Year;
                investmentPlan.Id = Guid.NewGuid();
            }

            investmentPlan.NumberOfYearsInAnalysisPeriod = investmentPlan.NumberOfYearsInAnalysisPeriod == 0 ? 1 : investmentPlan.NumberOfYearsInAnalysisPeriod;

            var budgets = request.SyncModel.LibraryId == null ? _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId) :
                _unitOfWork.BudgetRepo.GetBudgetLibrary(request.SyncModel.LibraryId.Value).Budgets;

            budgets = SyncedDataset(budgets, request.SyncModel);

            if (request.sortColumn.Trim() != "")
                budgets = OrderByColumn(budgets, request.sortColumn, request.isDescending);

            if (budgets.Count > 0 && budgets[0].BudgetAmounts.Count > 0)
            {
                firstYear = budgets[0].BudgetAmounts.Min(_ => _.Year);
                lastYear = budgets[0].BudgetAmounts.Max(_ => _.Year);
            }

            if (request.RowsPerPage > 0)
            {
                take = request.RowsPerPage;
                skip = request.RowsPerPage * (request.Page - 1);
                total = budgets.Count != 0 ? budgets.First().BudgetAmounts.Count : 0;
                budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.Skip(skip).Take(take).ToList());
                items = budgets;
            }
            else
            {
                items = budgets;
                return new InvestmentPagingPageModel()
                {
                    Items = items,
                    TotalItems = total,
                    LastYear = lastYear,
                    FirstYear = firstYear,
                    InvestmentPlan = investmentPlan
                };
            }

            return new InvestmentPagingPageModel()
            {
                Items = items,
                TotalItems = total,
                LastYear = lastYear,
                FirstYear = firstYear,
                InvestmentPlan = investmentPlan
            };
        }

        public List<BudgetDTO> GetSyncedScenarioDataSet(Guid simulationId, InvestmentPagingSyncModel request)
        {
            var budgets = request.LibraryId == null ?
                    _unitOfWork.BudgetRepo.GetScenarioBudgets(simulationId) :
                    _unitOfWork.BudgetRepo.GetBudgetLibrary(request.LibraryId.Value).Budgets;
            budgets = SyncedDataset(budgets, request);

            if (request.LibraryId != null)
            {
                budgets.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.BudgetAmounts.ForEach(__ => __.Id = Guid.NewGuid());
                });
            }

            budgets.ForEach(_ => _.BudgetAmounts.ForEach(__ => __.Year += request.FirstYearAnalysisBudgetShift));
            return budgets;
        }

        public List<BudgetDTO> GetSyncedLibraryDataset(Guid libraryId, InvestmentPagingSyncModel request)
        {
            var budgets = _unitOfWork.BudgetRepo.GetBudgetLibrary(libraryId).Budgets;
            return SyncedDataset(budgets, request);
        }

        public List<BudgetDTO> GetNewLibraryDataset(InvestmentPagingSyncModel request)
        {
            var budgets = new List<BudgetDTO>();
            return SyncedDataset(budgets, request);
        }

        private List<BudgetDTO> OrderByColumn(List<BudgetDTO> budgets, string sortColumn, bool isDescending)
        {
            sortColumn = sortColumn?.ToLower().Trim();
            switch (sortColumn)
            {
            case "year":
                if (isDescending)
                {
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderByDescending(__ => __.Year).ToList());
                    return budgets;
                }
                else
                {
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderBy(__ => __.Year).ToList());
                    return budgets;
                }
            default:
                var budget = budgets.FirstOrDefault(_ => _.Name.ToLower().Trim() == sortColumn);
                if (isDescending)
                {
                    budget.BudgetAmounts = budget.BudgetAmounts.OrderByDescending(_ => _.Value).ToList();
                    var dict = budget.BudgetAmounts.ToDictionary(_ => _.Year, _ => _.Value);
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderByDescending(__ => dict[__.Year]).ToList());
                }
                else
                {
                    budget.BudgetAmounts = budget.BudgetAmounts.OrderBy(_ => _.Value).ToList();
                    var dict = budget.BudgetAmounts.ToDictionary(_ => _.Year, _ => _.Value);
                    budgets.ForEach(_ => _.BudgetAmounts = _.BudgetAmounts.OrderBy(__ => dict[__.Year]).ToList());
                }

                return budgets;
            }
        }

        private List<BudgetDTO> SyncedDataset(List<BudgetDTO> budgets, InvestmentPagingSyncModel syncModel)
        {
            budgets = budgets.Concat(syncModel.AddedBudgets).Where(_ => !syncModel.BudgetsForDeletion.Contains(_.Id)).ToList();
            for (var i = 0; i < budgets.Count; i++)
            {
                var budget = budgets[i];
                var item = syncModel.UpdatedBudgets.FirstOrDefault(row => row.Id == budget.Id);
                if (item != null)
                {
                    budget.Name = item.Name;
                    budget.BudgetOrder = item.BudgetOrder;
                    budget.CriterionLibrary = item.CriterionLibrary;
                }
                if (syncModel.Deletionyears.Count != 0)
                    budget.BudgetAmounts = budget.BudgetAmounts.Where(_ => !syncModel.Deletionyears.Contains(_.Year)).ToList();
                if (syncModel.AddedBudgetAmounts.ContainsKey(budget.Name))
                    budget.BudgetAmounts = budget.BudgetAmounts.Concat(syncModel.AddedBudgetAmounts[budget.Name]).ToList();
                if (syncModel.UpdatedBudgetAmounts.ContainsKey(budget.Name))
                    for (var o = 0; o < budget.BudgetAmounts.Count; o++)
                    {
                        var amount = syncModel.UpdatedBudgetAmounts[budget.Name].FirstOrDefault(row => row.Id == budget.BudgetAmounts[o].Id);
                        if (amount != null)
                            budget.BudgetAmounts[o] = amount;
                    }
            }

            return budgets;
        }
    }
}
