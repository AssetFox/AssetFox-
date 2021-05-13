using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using TreatmentUpsertMethod = Action<Guid, TreatmentLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, TreatmentUpsertMethod> _treatmentUpsertMethods;

        public TreatmentController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _treatmentUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, TreatmentUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, TreatmentLibraryDTO dto)
            {
                UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto, simulationId);
                UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, TreatmentLibraryDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

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
                var result = await Task.Factory.StartNew(() => UnitOfWork.SelectableTreatmentRepo
                    .TreatmentLibrariesWithTreatments());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertTreatmentLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertTreatmentLibrary(Guid simulationId, TreatmentLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _treatmentUpsertMethods[UserInfo.Role](simulationId, dto);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteTreatmentLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeleteTreatmentLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Treatment error::{e.Message}");
                throw;
            }
        }
    }
}
