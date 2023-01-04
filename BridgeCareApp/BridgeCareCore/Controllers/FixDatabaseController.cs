using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixDatabaseController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public FixDatabaseController(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpPost]
        [Route("FixCommittedProjects")]
        public async Task FixCommittedProjects()
        {
            if (!_unitOfWork.Context.CommittedProject.Any())
            {
                return;
            }

            _unitOfWork.BeginTransaction();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var committedProjects = _unitOfWork.Context.CommittedProject.AsNoTracking()
                        .Include(_ => _.ScenarioBudget)
                        .ThenInclude(_ => _.ScenarioBudgetAmounts)
                        .Include(_ => _.ScenarioBudget)
                        .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                        .ThenInclude(_ => _.CriterionLibrary)
                        .ToList();

                    var budgets = new List<ScenarioBudgetEntity>();
                    var budgetAmounts = new List<ScenarioBudgetAmountEntity>();
                    var criteria = new List<CriterionLibraryEntity>();
                    var criterionJoins = new List<CriterionLibraryScenarioBudgetEntity>();

                    committedProjects
                        .ForEach(_ =>
                        {
                            if (budgets.All(budget => budget.Id != _.ScenarioBudgetId) && (_.ScenarioBudgetId != null))
                            {
                                var budget = new ScenarioBudgetEntity
                                {
                                    Id = _.ScenarioBudget.Id,
                                    Name = _.ScenarioBudget.Name,
                                    SimulationId = _.SimulationId,
                                    CreatedBy = _.ScenarioBudget.CreatedBy,
                                    LastModifiedBy = _.ScenarioBudget.LastModifiedBy,
                                };
                                budgets.Add(budget);

                                if (_.ScenarioBudget.ScenarioBudgetAmounts.Any())
                                {
                                    budgetAmounts.AddRange(_.ScenarioBudget.ScenarioBudgetAmounts.Select(amount =>
                                        new ScenarioBudgetAmountEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            Year = amount.Year,
                                            Value = amount.Value,
                                            ScenarioBudgetId = budget.Id,
                                            CreatedBy = amount.CreatedBy,
                                            LastModifiedBy = amount.LastModifiedBy
                                        }).ToList());
                                }

                                if (_.ScenarioBudget.CriterionLibraryScenarioBudgetJoin != null)
                                {
                                    var criterion = new CriterionLibraryEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        MergedCriteriaExpression =
                                            _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary
                                                .MergedCriteriaExpression,
                                        Name = $"{_.Name} Criterion",
                                        IsSingleUse = true,
                                        CreatedBy = _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.CreatedBy,
                                        LastModifiedBy = _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary
                                            .LastModifiedBy
                                    };
                                    criteria.Add(criterion);
                                    criterionJoins.Add(new CriterionLibraryScenarioBudgetEntity
                                    {
                                        ScenarioBudgetId = (Guid)_.ScenarioBudgetId,  // We have filtered out ScenarioBudgetId == null earlier
                                        CriterionLibraryId = criterion.Id
                                    });
                                }
                            }
                        });

                    var entityIds = budgets.Select(_ => _.Id).ToList();
                    var existingEntityIds = _unitOfWork.Context.ScenarioBudget.AsNoTracking()
                        .Where(_ => entityIds.Contains(_.Id)).Select(_ => _.Id).ToList();

                    var simulationIds = budgets.Select(_ => _.SimulationId).Distinct().ToList();

                    _unitOfWork.Context.DeleteAll<ScenarioBudgetAmountEntity>(_ =>
                        simulationIds.Contains(_.ScenarioBudget.SimulationId));

                    _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioBudgetEntity>(_ =>
                        simulationIds.Contains(_.ScenarioBudget.SimulationId));

                    _unitOfWork.Context.DeleteAll<ScenarioBudgetEntity>(_ => !entityIds.Contains(_.Id));
                    _unitOfWork.Context.UpdateAll(budgets.Where(_ => existingEntityIds.Contains(_.Id)).ToList());
                    _unitOfWork.Context.AddAll(budgets.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());

                    _unitOfWork.Context.AddAll(budgetAmounts.ToList());

                    _unitOfWork.Context.AddAll(criteria.ToList());
                    _unitOfWork.Context.AddAll(criterionJoins);
                });
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        /*[HttpPost]
        [Route("FixBudgetPercentagePairs")]
        public async Task FixBudgetPercentagePairs()
        {
            if (!_unitOfWork.Context.BudgetPercentagePair.Any())
            {
                return;
            }

            _unitOfWork.BeginTransaction();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var budgetPercentagePairs = _unitOfWork.Context.BudgetPercentagePair.AsNoTracking()
                        .Include(_ => _.ScenarioBudget)
                        .ThenInclude(_ => _.ScenarioBudgetAmounts)
                        .Include(_ => _.ScenarioBudget)
                        .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                        .ThenInclude(_ => _.CriterionLibrary)
                        .Include(_ => _.BudgetPriority)
                        .ThenInclude(_ => _.BudgetPriorityLibrary)
                        .ThenInclude(_ => _.BudgetPriorityLibrarySimulationJoins)
                        .ToList();

                    var budgets = new List<ScenarioBudgetEntity>();
                    var budgetAmounts = new List<ScenarioBudgetAmountEntity>();
                    var criteria = new List<CriterionLibraryEntity>();
                    var criterionJoins = new List<CriterionLibraryScenarioBudgetEntity>();
                    var percentagePairs = new List<BudgetPercentagePairEntity>();

                    var simulationIds = budgetPercentagePairs.SelectMany(_ =>
                        _.BudgetPriority.BudgetPriorityLibrary.BudgetPriorityLibrarySimulationJoins
                            .Select(join => join.SimulationId)
                    ).Distinct().ToList();
                    var budgetsPerSimulationId = _unitOfWork.Context.ScenarioBudget.AsNoTracking()
                        .Where(_ => simulationIds.Contains(_.SimulationId)).ToList()
                        .GroupBy(_ => _.SimulationId, _ => _)
                        .ToDictionary(_ => _.Key, _ => _.ToList());

                    budgetPercentagePairs.ForEach(_ =>
                    {
                        _.BudgetPriority.BudgetPriorityLibrary.BudgetPriorityLibrarySimulationJoins.ToList()
                            .ForEach(join =>
                            {
                                var budgetExistsOnSimulation =
                                    budgetsPerSimulationId.ContainsKey(join.SimulationId) &&
                                    budgetsPerSimulationId[join.SimulationId].Any(budget => budget.Name == _.ScenarioBudget.Name);
                                if (!budgetExistsOnSimulation &&
                                    !budgets.Any(b => b.SimulationId == join.SimulationId && b.Name == _.ScenarioBudget.Name))
                                {
                                    var budget = new ScenarioBudgetEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        Name = _.ScenarioBudget.Name,
                                        SimulationId = join.SimulationId,
                                        CreatedBy = _.ScenarioBudget.CreatedBy,
                                        LastModifiedBy = _.ScenarioBudget.LastModifiedBy,
                                    };
                                    budgets.Add(budget);

                                    if (_.ScenarioBudget.ScenarioBudgetAmounts.Any())
                                    {
                                        budgetAmounts.AddRange(_.ScenarioBudget.ScenarioBudgetAmounts.Select(amount =>
                                            new ScenarioBudgetAmountEntity
                                            {
                                                Id = Guid.NewGuid(),
                                                Year = amount.Year,
                                                Value = amount.Value,
                                                ScenarioBudgetId = budget.Id,
                                                CreatedBy = amount.CreatedBy,
                                                LastModifiedBy = amount.LastModifiedBy
                                            }).ToList());
                                    }

                                    if (_.ScenarioBudget.CriterionLibraryScenarioBudgetJoin != null)
                                    {
                                        var criterion = new CriterionLibraryEntity
                                        {
                                            Id = Guid.NewGuid(),
                                            MergedCriteriaExpression =
                                                _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary
                                                    .MergedCriteriaExpression,
                                            Name = $"{budget.Name} Criterion",
                                            IsSingleUse = true,
                                            CreatedBy = _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.CreatedBy,
                                            LastModifiedBy = _.ScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibrary
                                                .LastModifiedBy
                                        };
                                        criteria.Add(criterion);
                                        criterionJoins.Add(new CriterionLibraryScenarioBudgetEntity
                                        {
                                            ScenarioBudgetId = budget.Id,
                                            CriterionLibraryId = criterion.Id
                                        });
                                    }

                                    percentagePairs.Add(new BudgetPercentagePairEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        //BudgetId = _.BudgetId.Value,
                                        ScenarioBudgetId = budget.Id,
                                        BudgetPriorityId = _.BudgetPriorityId,
                                        Percentage = _.Percentage,
                                        CreatedBy = _.CreatedBy,
                                        LastModifiedBy = _.LastModifiedBy
                                    });
                                }
                                else
                                {
                                    var budget = budgetsPerSimulationId.ContainsKey(join.SimulationId) &&
                                                 budgetsPerSimulationId[join.SimulationId]
                                                     .Any(budget => budget.Name == _.ScenarioBudget.Name)
                                        ? budgetsPerSimulationId[join.SimulationId]
                                            .First(budget => budget.Name == _.ScenarioBudget.Name)
                                        : budgets.First(budget => budget.Name == _.ScenarioBudget.Name);

                                    percentagePairs.Add(new BudgetPercentagePairEntity
                                    {
                                        Id = Guid.NewGuid(),
                                        //BudgetId = _.BudgetId.Value,
                                        ScenarioBudgetId = budget.Id,
                                        BudgetPriorityId = _.BudgetPriorityId,
                                        Percentage = _.Percentage,
                                        CreatedBy = _.CreatedBy,
                                        LastModifiedBy = _.LastModifiedBy
                                    });
                                }
                            });
                    });

                    var existingEntityIds = _unitOfWork.Context.BudgetPercentagePair.AsNoTracking()
                        .Select(_ => _.Id).ToList();
                    _unitOfWork.Context.DeleteAll<BudgetPercentagePairEntity>(_ => existingEntityIds.Contains(_.Id));

                    _unitOfWork.Context.AddAll(budgets.ToList());

                    _unitOfWork.Context.AddAll(budgetAmounts.ToList());

                    _unitOfWork.Context.AddAll(criteria.ToList());
                    _unitOfWork.Context.AddAll(criterionJoins);

                    _unitOfWork.Context.AddAll(percentagePairs);
                });
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }*/
    }
}
