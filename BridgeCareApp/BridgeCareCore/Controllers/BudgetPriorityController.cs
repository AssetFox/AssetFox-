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
    using BudgetPriorityUpsertMethod = Action<Guid, BudgetPriorityLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, BudgetPriorityUpsertMethod> _budgetPriorityUpsertMethods;

        public BudgetPriorityController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _budgetPriorityUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, BudgetPriorityUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, BudgetPriorityLibraryDTO dto)
            {
                UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto, simulationId);
                UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, BudgetPriorityLibraryDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

            return new Dictionary<string, BudgetPriorityUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [Authorize]
        public async Task<IActionResult> BudgetPriorityLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.BudgetPriorityRepo
                    .BudgetPriorityLibrariesWithBudgetPriorities());

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetPriorityLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(Guid simulationId, BudgetPriorityLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _budgetPriorityUpsertMethods[UserInfo.Role](simulationId, dto);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetPriorityLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }
    }
}
