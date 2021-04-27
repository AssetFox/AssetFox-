using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using InvestmentUpsertMethod = Action<Guid, UpsertInvestmentDataDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IReadOnlyDictionary<string, InvestmentUpsertMethod> _investmentUpsertMethods;

        public InvestmentController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _investmentUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, InvestmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                _unitOfWork.BudgetRepo.UpsertBudgetLibrary(data.BudgetLibrary, simulationId);
                _unitOfWork.BudgetRepo.UpsertOrDeleteBudgets(data.BudgetLibrary.Budgets, data.BudgetLibrary.Id);
                _unitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(data.InvestmentPlan, simulationId);
            }

            void UpsertPermitted(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                _unitOfWork.BudgetRepo.UpsertPermitted(simulationId, data.BudgetLibrary);
                _unitOfWork.InvestmentPlanRepo.UpsertPermitted(simulationId, data.InvestmentPlan);
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
                var result = await Task.Factory.StartNew(() =>
                {
                    var budgetLibraries = _unitOfWork.BudgetRepo
                        .BudgetLibrariesWithBudgets();

                    var scenarioInvestmentPlan = _unitOfWork.InvestmentPlanRepo
                        .ScenarioInvestmentPlan(simulationId);
                    return new InvestmentDTO
                    {
                        BudgetLibraries = budgetLibraries, InvestmentPlan = scenarioInvestmentPlan
                    };
                });


                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
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

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _investmentUpsertMethods[userInfo.Role](simulationId, data);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBudgetLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSimpleBudgetDetails/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _unitOfWork.BudgetRepo
                    .ScenarioSimpleBudgetDetails(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }
    }
}
