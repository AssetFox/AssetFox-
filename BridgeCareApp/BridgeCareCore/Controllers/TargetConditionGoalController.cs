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
    using TargetConditionGoalUpsertMethod = Action<UserInfoDTO, Guid, TargetConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class TargetConditionGoalController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;

        private readonly IReadOnlyDictionary<string, TargetConditionGoalUpsertMethod>
            _targetConditionGoalUpsertMethods;

        public TargetConditionGoalController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity;
            _targetConditionGoalUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, TargetConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .UpsertTargetConditionGoalLibrary(dto, simulationId, userInfo);
                _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                _unitOfDataPersistenceWork.TargetConditionGoalRepo.UpsertPermitted(userInfo, simulationId, dto);
            }

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
                var result = await _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .TargetConditionGoalLibrariesWithTargetConditionGoals();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _targetConditionGoalUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
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
        [Route("DeleteTargetConditionGoalLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteTargetConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.TargetConditionGoalRepo
                    .DeleteTargetConditionGoalLibrary(libraryId));
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
    }
}
