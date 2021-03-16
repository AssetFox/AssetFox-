using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemainingLifeLimitController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;

        public RemainingLifeLimitController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
        }

        [HttpGet]
        [Route("GetRemainingLifeLimitLibraries")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> RemainingLifeLimitLibraries()
        {
            try
            {
                var result = await _unitOfDataPersistenceWork.RemainingLifeLimitRepo
                    .RemainingLifeLimitLibrariesWithRemainingLifeLimits();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("UpsertRemainingLifeLimitLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> UpsertRemainingLifeLimitLibrary(Guid simulationId, RemainingLifeLimitLibraryDTO dto)
        {
            try
            {
                var userInfo = _esecSecurity.GetUserInformation(Request).ToDto();
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _unitOfDataPersistenceWork.RemainingLifeLimitRepo
                        .UpsertRemainingLifeLimitLibrary(dto, simulationId, userInfo);
                    _unitOfDataPersistenceWork.RemainingLifeLimitRepo
                        .UpsertOrDeleteRemainingLifeLimits(dto.RemainingLifeLimits, dto.Id, userInfo);
                });

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

        [HttpDelete]
        [Route("DeleteRemainingLifeLimitLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.Admin)]
        public async Task<IActionResult> DeleteRemainingLifeLimitLibrary(Guid libraryId)
        {
            try
            {
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() => _unitOfDataPersistenceWork.RemainingLifeLimitRepo
                    .DeleteRemainingLifeLimitLibrary(libraryId));
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
