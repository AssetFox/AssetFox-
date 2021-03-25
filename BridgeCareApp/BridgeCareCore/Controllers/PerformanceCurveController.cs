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
    using PerformanceCurveUpsertMethod = Action<UserInfoDTO, Guid, PerformanceCurveLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, PerformanceCurveUpsertMethod> _performanceCurveUpsertMethods;

        public PerformanceCurveController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _performanceCurveUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, PerformanceCurveUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, PerformanceCurveLibraryDTO dto)
            {
                _unitOfWork.PerformanceCurveRepo
                    .UpsertPerformanceCurveLibrary(dto, simulationId, userInfo);
                _unitOfWork.PerformanceCurveRepo
                    .UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, PerformanceCurveLibraryDTO dto) =>
                _unitOfWork.PerformanceCurveRepo.UpsertPermitted(userInfo, simulationId, dto);

            return new Dictionary<string, PerformanceCurveUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetPerformanceCurveLibraries")]
        [Authorize]
        public async Task<IActionResult> PerformanceCurveLibraries()
        {
            try
            {
                var result = await _unitOfWork.PerformanceCurveRepo
                    .PerformanceCurveLibrariesWithPerformanceCurves();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertPerformanceCurveLibrary(Guid simulationId, PerformanceCurveLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _performanceCurveUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
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
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeletePerformanceCurveLibrary(Guid libraryId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId));
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
