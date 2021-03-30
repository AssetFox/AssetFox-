using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    using AnalysisMethodGetMethod = Func<Guid, AnalysisMethodDTO>;
    using AnalysisMethodUpsertMethod = Action<Guid, AnalysisMethodDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IReadOnlyDictionary<string, AnalysisMethodGetMethod> _analysisMethodGetMethods;
        private readonly IReadOnlyDictionary<string, AnalysisMethodUpsertMethod> _analysisMethodUpsertMethods;

        public AnalysisMethodController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity;
            _unitOfWork = unitOfWork;
            _analysisMethodGetMethods = CreateGetMethods();
            _analysisMethodUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, AnalysisMethodGetMethod> CreateGetMethods()
        {
            AnalysisMethodDTO GetAny(Guid simulationId) =>
                _unitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);

            AnalysisMethodDTO GetPermitted(Guid simulationId) =>
                _unitOfWork.AnalysisMethodRepo.GetPermittedAnalysisMethod(simulationId);

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
                _unitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto);

            void UpsertPermitted(Guid simulationId, AnalysisMethodDTO dto) =>
                _unitOfWork.AnalysisMethodRepo.UpsertPermittedAnalysisMethod(simulationId, dto);

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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                var result = await Task.Factory.StartNew(() =>
                    _analysisMethodGetMethods[userInfo.Role](simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
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
                var userInfo = _esecSecurity.GetUserInformation(Request);

                _unitOfWork.SetUser(userInfo.Name);

                await Task.Factory.StartNew(() =>
                {
                    _unitOfWork.BeginTransaction();
                    _analysisMethodUpsertMethods[userInfo.Role](simulationId, dto);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
                throw;
            }
        }
    }
}
