using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    using AnalysisMethodGetMethod = Func<Guid, AnalysisMethodDTO>;
    using AnalysisMethodUpsertMethod = Action<Guid, AnalysisMethodDTO>;

    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : ControllerBase
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly IEsecSecurity _esecSecurity;
        private readonly IReadOnlyDictionary<string, AnalysisMethodGetMethod> _analysisMethodGetMethods;
        private readonly IReadOnlyDictionary<string, AnalysisMethodUpsertMethod> _analysisMethodUpsertMethods;

        public AnalysisMethodController(UnitOfDataPersistenceWork unitOfWork, IEsecSecurity esecSecurity)
        {
            _unitOfWork = unitOfWork;
            _esecSecurity = esecSecurity;
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
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}
