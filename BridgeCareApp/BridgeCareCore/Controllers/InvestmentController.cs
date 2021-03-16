using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using InvestmentUpsertMethod = Action<UserInfoDTO, Guid, UpsertInvestmentDataDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, InvestmentUpsertMethod> _investmentUpsertMethods;

        public InvestmentController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _investmentUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, InvestmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, UpsertInvestmentDataDTO data)
            {
                _unitOfDataPersistenceWork.BudgetRepo
                    .UpsertBudgetLibrary(data.BudgetLibrary, simulationId, userInfo);
                _unitOfDataPersistenceWork.BudgetRepo
                    .UpsertOrDeleteBudgets(data.BudgetLibrary.Budgets, data.BudgetLibrary.Id, userInfo);
                _unitOfDataPersistenceWork.InvestmentPlanRepo
                    .UpsertInvestmentPlan(data.InvestmentPlan, simulationId, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, UpsertInvestmentDataDTO data)
            {
                _unitOfDataPersistenceWork.BudgetRepo.UpsertPermitted(userInfo, simulationId, data.BudgetLibrary);
                _unitOfDataPersistenceWork.InvestmentPlanRepo.UpsertPermitted(userInfo, simulationId,
                    data.InvestmentPlan);
            }

            return new Dictionary<string, InvestmentUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        [Authorize]
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
        [Route("UpsertInvestment/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertInvestment(Guid simulationId, [FromBody] UpsertInvestmentDataDTO data)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _investmentUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, data);
                });

                _unitOfDataPersistenceWork.Commit();
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfDataPersistenceWork.Rollback();
                Console.WriteLine(e);
                return Unauthorized(e);
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
        [Authorize]
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
        [Authorize]
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
