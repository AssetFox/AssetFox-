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
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;

        private readonly IReadOnlyDictionary<string, TargetConditionGoalUpsertMethod>
            _targetConditionGoalUpsertMethods;

        public TargetConditionGoalController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity;
            _targetConditionGoalUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, TargetConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                _unitOfWork.TargetConditionGoalRepo
                    .UpsertTargetConditionGoalLibrary(dto, simulationId, userInfo);
                _unitOfWork.TargetConditionGoalRepo
                    .UpsertOrDeleteTargetConditionGoals(dto.TargetConditionGoals, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TargetConditionGoalLibraryDTO dto)
            {
                _unitOfWork.TargetConditionGoalRepo.UpsertPermitted(userInfo, simulationId, dto);
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
                var result = await _unitOfWork.TargetConditionGoalRepo
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
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _targetConditionGoalUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
                });

                _unitOfWork.Commit();
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                return Unauthorized(e);
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
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
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfWork.TargetConditionGoalRepo
                    .DeleteTargetConditionGoalLibrary(libraryId));
                _unitOfWork.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
