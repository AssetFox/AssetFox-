using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    using CashFlowUpsertMethod = Action<Guid, CashFlowRuleLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class CashFlowController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IReadOnlyDictionary<string, CashFlowUpsertMethod> _cashFlowUpsertMethods;

        public CashFlowController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cashFlowUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, CashFlowUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, CashFlowRuleLibraryDTO dto)
            {
                _unitOfWork.CashFlowRuleRepo.UpsertCashFlowRuleLibrary(dto, simulationId);
                _unitOfWork.CashFlowRuleRepo.UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, CashFlowRuleLibraryDTO dto) =>
                _unitOfWork.CashFlowRuleRepo.UpsertPermitted(simulationId, dto);

            return new Dictionary<string, CashFlowUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetCashFlowRuleLibraries")]
        [Authorize]
        public async Task<IActionResult> CashFlowRuleLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _unitOfWork.CashFlowRuleRepo
                    .CashFlowRuleLibrariesWithCashFlowRules());

                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertCashFlowRuleLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertCashFlowRuleLibrary(Guid simulationId, CashFlowRuleLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _cashFlowUpsertMethods[userInfo.Role](simulationId, dto);
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
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
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.CashFlowRuleRepo.DeleteCashFlowRuleLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Cash Flow error::{e.Message}");
                throw;
            }
        }
    }
}
