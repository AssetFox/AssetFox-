using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using InvestmentUpsertMethod = Action<Guid, UpsertInvestmentDataDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, InvestmentUpsertMethod> _investmentUpsertMethods;

        public InvestmentController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _investmentUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, InvestmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                UnitOfWork.BudgetRepo.UpsertBudgetLibrary(data.BudgetLibrary, simulationId);
                UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(data.BudgetLibrary.Budgets, data.BudgetLibrary.Id);
                UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(data.InvestmentPlan, simulationId);
            }

            void UpsertPermitted(Guid simulationId, UpsertInvestmentDataDTO data)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, data);
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
                    var budgetLibraries = UnitOfWork.BudgetRepo
                        .BudgetLibrariesWithBudgets();

                    var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo
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
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
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
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _investmentUpsertMethods[UserInfo.Role](simulationId, data);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
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
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
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
                var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetRepo
                    .ScenarioSimpleBudgetDetails(simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }
    }
}
