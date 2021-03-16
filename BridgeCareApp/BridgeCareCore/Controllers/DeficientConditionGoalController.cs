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
    using DeficientConditionGoalUpsertMethod = Action<UserInfoDTO, Guid, DeficientConditionGoalLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class DeficientConditionGoalController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;

        private readonly IReadOnlyDictionary<string, DeficientConditionGoalUpsertMethod>
            _deficientConditionGoalUpsertMethods;

        public DeficientConditionGoalController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _deficientConditionGoalUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, DeficientConditionGoalUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, DeficientConditionGoalLibraryDTO dto)
            {
                _unitOfDataPersistenceWork.DeficientConditionGoalRepo
                    .UpsertDeficientConditionGoalLibrary(dto, simulationId, userInfo);
                _unitOfDataPersistenceWork.DeficientConditionGoalRepo
                    .UpsertOrDeleteDeficientConditionGoals(dto.DeficientConditionGoals, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, DeficientConditionGoalLibraryDTO dto)
            {
                _unitOfDataPersistenceWork.DeficientConditionGoalRepo.UpsertPermitted(userInfo, simulationId, dto);
            }

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
                var result = await _unitOfDataPersistenceWork.DeficientConditionGoalRepo
                    .DeficientConditionGoalLibrariesWithDeficientConditionGoals();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _deficientConditionGoalUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
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
        [Route("DeleteDeficientConditionGoalLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDeficientConditionGoalLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.DeficientConditionGoalRepo
                    .DeleteDeficientConditionGoalLibrary(libraryId));
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
