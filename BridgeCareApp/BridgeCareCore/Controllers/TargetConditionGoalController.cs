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

namespace BridgeCareCore.Controllers
{
    using TargetConditionGoalUpsertMethod = Action<Guid, TargetConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        private readonly IReadOnlyDictionary<string, TargetConditionGoalUpsertMethod>
            _targetConditionGoalUpsertMethods;

        public TargetConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _targetConditionGoalUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, TargetConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                _unitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto, simulationId);
                _unitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, TargetConditionGoalLibraryDTO dto) =>
                _unitOfWork.TargetConditionGoalRepo.UpsertPermitted(simulationId, dto);

            return new Dictionary<string, TargetConditionGoalUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetTargetConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> TargetConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _unitOfWork.TargetConditionGoalRepo
                    .TargetConditionGoalLibrariesWithTargetConditionGoals());
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTargetConditionGoalLibrary/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertTargetConditionGoalLibrary(Guid simulationId, TargetConditionGoalLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _targetConditionGoalUpsertMethods[userInfo.Role](simulationId, dto);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                _unitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTargetConditionGoalLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.TargetConditionGoalRepo.DeleteTargetConditionGoalLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Target Condition Goal error::{e.Message}");
                throw;
            }
        }
    }
}
