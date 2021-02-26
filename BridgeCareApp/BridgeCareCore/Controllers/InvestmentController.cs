using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public InvestmentController(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        public async Task<IActionResult> GetInvestment(Guid simulationId)
        {
            try
            {
                var budgetLibraries = await _unitOfDataPersistenceWork.BudgetRepo
                    .BudgetLibrariesWithBudgets();
                var scenarioInvestmentPlan = await _unitOfDataPersistenceWork.InvestmentPlanRepo
                    .ScenarioInvestmentPlan(simulationId);
                return Ok(new InvestmentDTO
                {
                    BudgetLibraries = budgetLibraries,
                    InvestmentPlan = scenarioInvestmentPlan
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("AddOrUpdateInvestment/{simulationId}")]
        public async Task<IActionResult> AddOrUpdateInvestment(Guid simulationId, BudgetLibraryDTO budgetLibraryDTO, InvestmentPlanDTO investmentPlanDTO)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.BudgetRepo
                        .AddOrUpdateBudgetLibrary(budgetLibraryDTO, simulationId);
                    _unitOfDataPersistenceWork.BudgetRepo
                        .AddOrUpdateOrDeleteBudgets(budgetLibraryDTO.Budgets, budgetLibraryDTO.Id);
                    _unitOfDataPersistenceWork.InvestmentPlanRepo
                        .AddOrUpdateInvestmentPlan(investmentPlanDTO, simulationId);
                });

                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetLibrary/{libraryId}")]
        public async Task<IActionResult> DeleteBudgetLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.BudgetRepo
                    .DeleteBudgetLibrary(libraryId));
                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("GetScenarioSimpleBudgetDetails/{simulationId}")]
        public async Task<IActionResult> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.BudgetRepo
                    .ScenarioSimpleBudgetDetails(simulationId);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
