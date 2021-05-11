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
    using PerformanceCurveUpsertMethod = Action<Guid, PerformanceCurveLibraryDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceCurveController : BridgeCareCoreBaseController
    {
        private readonly IEsecSecurity _esecSecurity;

        private readonly IReadOnlyDictionary<string, PerformanceCurveUpsertMethod> _performanceCurveUpsertMethods;

        public PerformanceCurveController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _performanceCurveUpsertMethods = CreateUpsertMethods();

        private Dictionary<string, PerformanceCurveUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, PerformanceCurveLibraryDTO dto)
            {
                UnitOfWork.PerformanceCurveRepo.UpsertPerformanceCurveLibrary(dto, simulationId);
                UnitOfWork.PerformanceCurveRepo.UpsertOrDeletePerformanceCurves(dto.PerformanceCurves, dto.Id);
            }

            void UpsertPermitted(Guid simulationId, PerformanceCurveLibraryDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

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
                var result = await Task.Factory.StartNew(() => UnitOfWork.PerformanceCurveRepo
                    .PerformanceCurveLibrariesWithPerformanceCurves());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertPerformanceCurveLibrary/{simulationId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> UpsertPerformanceCurveLibrary(Guid simulationId, PerformanceCurveLibraryDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _performanceCurveUpsertMethods[UserInfo.Role](simulationId, dto);
                    UnitOfWork.Commit();
                });


                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                return Unauthorized();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeletePerformanceCurveLibrary/{libraryId}")]
        [Authorize(Policy = SecurityConstants.Policy.AdminOrDistrictEngineer)]
        public async Task<IActionResult> DeletePerformanceCurveLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.PerformanceCurveRepo.DeletePerformanceCurveLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Performance Curve error::{e.Message}");
                throw;
            }
        }
    }
}
