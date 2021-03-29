using System;
using System.Threading.Tasks;
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
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionLibraryController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CriterionLibraryController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        }

        [HttpGet]
        [Route("GetCriterionLibraries")]
        [Authorize]
        public async Task<IActionResult> CriterionLibraries()
        {
            try
            {
                var result = await _unitOfWork.CriterionLibraryRepo.CriterionLibraries();
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Criterion Library error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertCriterionLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertCriterionLibrary([FromBody] CriterionLibraryDTO dto)
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(dto);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Criterion Library error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteCriterionLibrary/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCriterionLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(libraryId);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Criterion Library error::{e.Message}");
                throw;
            }
        }
    }
}
