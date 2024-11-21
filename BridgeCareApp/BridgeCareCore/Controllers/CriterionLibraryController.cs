using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriterionLibraryController : BridgeCareCoreBaseController
    {
        public const string CriterionLibraryError = "Criterion Library Error";
        public CriterionLibraryController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) { }

        [HttpGet]
        [Route("GetCriterionLibraries")]
        [Authorize]
        public async Task<IActionResult> CriterionLibraries()
        {
            try
            {
                var result = await UnitOfWork.CriterionLibraryRepo.CriterionLibraries();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name,  $"{CriterionLibraryError}::CriterionLibraries - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetSpecificCriteria/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetCriteriaForScenario(Guid libraryId)
        {
            var result = await UnitOfWork.CriterionLibraryRepo.CriteriaLibrary(libraryId);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpsertCriterionLibrary")]
        [Authorize]
        public async Task<IActionResult> UpsertCriterionLibrary([FromBody] CriterionLibraryDTO dto)
        {
            try
            {
                var criterionLibraryId = Guid.Empty;
                await Task.Factory.StartNew(() =>
                {
                    criterionLibraryId = UnitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(dto);
                });

                return Ok(criterionLibraryId);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CriterionLibraryError}::UpsertCriterionLibrary {dto.MergedCriteriaExpression} - {e.Message}", e);
            }
            return Ok();
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
                    UnitOfWork.CriterionLibraryRepo.DeleteCriterionLibrary(libraryId);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{CriterionLibraryError}::DeleteCriterionLibrary - {e.Message}", e);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        //[Authorize(Policy = Policy.ModifyFromCriterionLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }
    }
}
