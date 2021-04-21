using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using BudgetPriorityUpsertMethod = Action<Guid, BudgetPriorityLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class BudgetPriorityController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IReadOnlyDictionary<string, BudgetPriorityUpsertMethod> _budgetPriorityUpsertMethods;

        public BudgetPriorityController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _budgetPriorityUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, BudgetPriorityUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, BudgetPriorityLibraryDTO dto)
            {
                _unitOfWork.BudgetPriorityRepo.UpsertBudgetPriorityLibrary(dto, simulationId);
                _unitOfWork.BudgetPriorityRepo.UpsertOrDeleteBudgetPriorities(dto.BudgetPriorities, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, BudgetPriorityLibraryDTO dto) =>
                _unitOfWork.BudgetPriorityRepo.UpsertPermitted(simulationId, dto);

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
                var result = await Task.Factory.StartNew(() => _unitOfWork.BudgetPriorityRepo
                    .BudgetPriorityLibrariesWithBudgetPriorities());

                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _budgetPriorityUpsertMethods[userInfo.Role](simulationId, dto);
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
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
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.BudgetPriorityRepo.DeleteBudgetPriorityLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Budget Priority error::{e.Message}");
                throw;
            }
        }
    }
}
