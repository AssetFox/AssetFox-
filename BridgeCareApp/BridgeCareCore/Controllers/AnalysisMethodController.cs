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
    using AnalysisMethodGetMethod = Func<UserInfoDTO, Guid, AnalysisMethodDTO>;
    using AnalysisMethodUpsertMethod = Action<UserInfoDTO, Guid, AnalysisMethodDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, AnalysisMethodGetMethod> _analysisMethodGetMethods;
        private readonly IReadOnlyDictionary<string, AnalysisMethodUpsertMethod> _analysisMethodUpsertMethods;

        public AnalysisMethodController(UnitOfDataPersistenceWork unitOfDataPersistenceWork, IEsecSecurity esecSecurity)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork;
            _esecSecurity = esecSecurity;
            _analysisMethodGetMethods = CreateGetMethods();
            _analysisMethodUpsertMethods = CreateUpsertMethods();
        }

        private Dictionary<string, AnalysisMethodGetMethod> CreateGetMethods()
        {
            AnalysisMethodDTO GetAny(UserInfoDTO userInfo, Guid simulationId) =>
                _unitOfDataPersistenceWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);

            AnalysisMethodDTO GetPermitted(UserInfoDTO userInfo, Guid simulationId) =>
                _unitOfDataPersistenceWork.AnalysisMethodRepo.GetPermittedAnalysisMethod(userInfo,
                    simulationId);

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
            void UpsertAny(UserInfoDTO userInfo, Guid simulationId, AnalysisMethodDTO dto) =>
                _unitOfDataPersistenceWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto, userInfo);

            void UpsertPermitted(UserInfoDTO userInfo, Guid simulationId, AnalysisMethodDTO dto) =>
                _unitOfDataPersistenceWork.AnalysisMethodRepo.UpsertPermittedAnalysisMethod(userInfo,
                    simulationId, dto);

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
                var result = await Task.Factory.StartNew(() =>
                    _analysisMethodGetMethods[userInfo.Role](userInfo.ToDto(), simulationId));
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
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
                _unitOfDataPersistenceWork.BeginTransaction();
                await Task.Factory.StartNew(() =>
                {
                    _analysisMethodUpsertMethods[userInfo.Role](userInfo.ToDto(), simulationId, dto);
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
    }
}
