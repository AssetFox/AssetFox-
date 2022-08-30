using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Utils.Interfaces;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : BridgeCareCoreBaseController
    { 
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public BudgetPriorityController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }              

        [HttpGet]
        [Route("GetBudgetPriorityLibraries")]
        [Authorize]
        public async Task<IActionResult> GetBudgetPriorityLibraries()
        {
            try
            {
                var result = new List<BudgetPriorityLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    result = GetAllBudgetPriorityLibraries();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = result.Where(_ => _.Owner == UserId || _.IsShared == true).ToList();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetPriorityLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertBudgetPriorityLibrary(BudgetPriorityLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var currentRecord = UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries().FirstOrDefault(_ => _.Id == dto.Id);
                    // by pass owner check if no record
                    if (currentRecord != null)
                    {
                        _claimHelper.CheckUserLibraryModifyAuthorization(currentRecord.Owner);
                    }
                    UnitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
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
        [Authorize]
        public async Task<IActionResult> DeleteBudgetPriorityLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var dto = GetAllBudgetPriorityLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (dto == null) return;
                        _claimHelper.CheckUserLibraryModifyAuthorization(dto.Owner);
                    }
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

        [HttpGet]
        [Route("GetScenarioBudgetPriorities/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioBudgetPriorities(Guid simulationId)
        {
            try
            {
                var result = new List<BudgetPriorityDTO>();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId);
                    result = UnitOfWork.BudgetPriorityRepo.GetScenarioBudgetPriorities(simulationId);
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertScenarioBudgetPriorities/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertScenarioBudgetPriorities(Guid simulationId, List<BudgetPriorityDTO> dtos)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId);
                    UnitOfWork.BudgetPriorityRepo.UpsertOrDeleteScenarioBudgetPriorities(dtos, simulationId);
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

        private List<BudgetPriorityLibraryDTO> GetAllBudgetPriorityLibraries()
        {
            return UnitOfWork.BudgetPriorityRepo.GetBudgetPriorityLibraries();
        }
    }
}
