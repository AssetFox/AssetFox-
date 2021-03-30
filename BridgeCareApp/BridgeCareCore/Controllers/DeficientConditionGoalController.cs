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
    using DeficientConditionGoalUpsertMethod = Action<Guid, DeficientConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        private readonly IReadOnlyDictionary<string, DeficientConditionGoalUpsertMethod>
            _deficientConditionGoalUpsertMethods;

        public DeficientConditionGoalController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _deficientConditionGoalUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, DeficientConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, DeficientConditionGoalLibraryDTO dto)
            {
                _unitOfWork.DeficientConditionGoalRepo.UpsertDeficientConditionGoalLibrary(dto, simulationId);
                _unitOfWork.DeficientConditionGoalRepo.UpsertOrDeleteDeficientConditionGoals(
                    dto.DeficientConditionGoals, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, DeficientConditionGoalLibraryDTO dto) =>
                _unitOfWork.DeficientConditionGoalRepo.UpsertPermitted(simulationId, dto);

            return new Dictionary<string, DeficientConditionGoalUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetDeficientConditionGoalLibraries")]
        [Authorize]
        public async Task<IActionResult> DeficientConditionGoalLibraries()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _unitOfWork.DeficientConditionGoalRepo
                    .DeficientConditionGoalLibrariesWithDeficientConditionGoals());
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertDeficientConditionGoalLibrary/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertDeficientConditionGoalLibrary(Guid simulationId, DeficientConditionGoalLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _deficientConditionGoalUpsertMethods[userInfo.Role](simulationId, dto);
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteDeficientConditionGoalLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDeficientConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.DeficientConditionGoalRepo.DeleteDeficientConditionGoalLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Deficient Condition Goal error::{e.Message}");
                throw;
            }
        }
    }
}
