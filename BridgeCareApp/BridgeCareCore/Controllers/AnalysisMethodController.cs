using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using AnalysisMethodGetMethod = Func<Guid, AnalysisMethodDTO>;
    using AnalysisMethodUpsertMethod = Action<Guid, AnalysisMethodDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : BridgeCareCoreBaseController
    {
        private readonly IReadOnlyDictionary<string, AnalysisMethodGetMethod> _analysisMethodGetMethods;
        private readonly IReadOnlyDictionary<string, AnalysisMethodUpsertMethod> _analysisMethodUpsertMethods;

        public AnalysisMethodController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _analysisMethodGetMethods = CreateGetMethods();
            _analysisMethodUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, AnalysisMethodGetMethod> CreateGetMethods()
        {
            AnalysisMethodDTO GetAny(Guid simulationId) =>
                UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);

            AnalysisMethodDTO GetPermitted(Guid simulationId)
            {
                CheckUserSimulationReadAuthorization(simulationId);
                return GetAny(simulationId);
            }

            return new Dictionary<string, AnalysisMethodGetMethod>
            {
                [Role.Administrator] = GetAny,
                [Role.DistrictEngineer] = GetPermitted,
                [Role.Cwopa] = GetPermitted,
                [Role.PlanningPartner] = GetPermitted
            };
        }

        private Dictionary<string, AnalysisMethodUpsertMethod> CreateUpsertMethods()
        {
            void UpsertAny(Guid simulationId, AnalysisMethodDTO dto) =>
                UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto);

            void UpsertPermitted(Guid simulationId, AnalysisMethodDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

            return new Dictionary<string, AnalysisMethodUpsertMethod>
            {
                [Role.Administrator] = UpsertAny,
                [Role.DistrictEngineer] = UpsertPermitted,
                [Role.Cwopa] = UpsertPermitted,
                [Role.PlanningPartner] = UpsertPermitted
            };
        }

        [HttpGet]
        [Route("GetAnalysisMethod/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> AnalysisMethod(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _analysisMethodGetMethods[UserInfo.Role](simulationId));

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertAnalysisMethod/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _analysisMethodUpsertMethods[UserInfo.Role](simulationId, dto);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
                throw;
            }
        }
    }
}
