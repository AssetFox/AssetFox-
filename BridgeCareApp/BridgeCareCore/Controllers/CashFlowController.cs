using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using CashFlowUpsertMethod = Action<Guid, List<CashFlowRuleDTO>>;

    [Route("api/[controller]")]
    [ApiController]
    public class CashFlowController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, CashFlowUpsertMethod> _cashFlowUpsertMethods;

        public CashFlowController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _cashFlowUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, CashFlowUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, List<CashFlowRuleDTO> dtos)
            {
                UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteScenarioCashFlowRules(dtos, simulationId);
            }

            void UpsertPermitted(Guid simulationId, List<CashFlowRuleDTO> dtos)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dtos);
            }

            return new Dictionary<string, CashFlowUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetCashFlowRuleLibraries")]
        [Authorize]
        public async Task<IActionResult> GetCashFlowRuleLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CashFlowRuleRepo
                    .GetCashFlowRuleLibraries());

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioCashFlowRules/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioCashFlowRules(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.CashFlowRuleRepo
                    .GetScenarioCashFlowRules(simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertCashFlowRuleLibrary")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto)
        {
            try
            {
                 await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto);
                    UnitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioCashFlowRules/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertScenarioCashFlowRules(Guid simulationId, List<CashFlowRuleDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _cashFlowUpsertMethods[UserInfo.Role](simulationId, dtos);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteCashFlowRuleLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }
    }
}
