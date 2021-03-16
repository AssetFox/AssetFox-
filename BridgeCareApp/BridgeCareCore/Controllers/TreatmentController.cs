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
    using TreatmentUpsertMethod = Action<UserInfoDTO, Guid, TreatmentLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, TreatmentUpsertMethod> _treatmentUpsertMethods;

        public TreatmentController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _treatmentUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, TreatmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, TreatmentLibraryDTO dto)
            {
                _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .UpsertTreatmentLibrary(dto, simulationId, userInfo);
                _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .UpsertOrDeleteTreatments(dto.Treatments, dto.Id, userInfo);
            }

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, TreatmentLibraryDTO dto) =>
                _unitOfDataPersistenceWork.SelectableTreatmentRepo.UpsertPermitted(userInfo, simulationId, dto);

            return new Dictionary<string, TreatmentUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetTreatmentLibraries")]
        [Authorize]
        public async Task<IActionResult> TreatmentLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .TreatmentLibrariesWithTreatments();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("UpsertTreatmentLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertTreatmentLibrary(Guid simulationId, TreatmentLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request);
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _treatmentUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
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
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteTreatmentLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.SelectableTreatmentRepo
                    .DeleteTreatmentLibrary(libraryId));
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
