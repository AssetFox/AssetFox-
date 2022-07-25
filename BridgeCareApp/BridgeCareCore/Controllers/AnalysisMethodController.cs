using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : BridgeCareCoreBaseController
    {
        //private readonly IReadOnlyDictionary<string, AnalysisMethodGetMethod> _analysisMethodGetMethods;
        //private readonly IReadOnlyDictionary<string, AnalysisMethodUpsertMethod> _analysisMethodUpsertMethods;
        private readonly IReadOnlyDictionary<string, NoLibraryCRUDMethods<AnalysisMethodDTO>> _analysisMethodMethods;
        public readonly IAnalysisDefaultDataService _analysisDefaultDataService;

        public AnalysisMethodController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IAnalysisDefaultDataService analysisDefaultDataService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            //_analysisMethodGetMethods = CreateGetMethods();
            //_analysisMethodUpsertMethods = CreateUpsertMethods();
            _analysisMethodMethods = CreateAnalysisMethodsByRole();
            _analysisDefaultDataService = analysisDefaultDataService ?? throw new ArgumentNullException(nameof(analysisDefaultDataService));
        }

        private Dictionary<string, NoLibraryCRUDMethods<AnalysisMethodDTO>> CreateAnalysisMethodsByRole()
        {
            AnalysisMethodDTO GetAny(Guid simulationId)
            {
                var analysisMethodDTO = UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);
                SetDefaultDataForNewScenario(analysisMethodDTO);
                return analysisMethodDTO;
            }

            AnalysisMethodDTO GetPermitted(Guid simulationId)
            {
                CheckUserSimulationReadAuthorization(simulationId);
                return GetAny(simulationId);
            }

            void UpsertAny(Guid simulationId, AnalysisMethodDTO dto) =>
                UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto);

            void UpsertPermitted(Guid simulationId, AnalysisMethodDTO dto)
            {
                CheckUserSimulationModifyAuthorization(simulationId);
                UpsertAny(simulationId, dto);
            }

            void DeleteAny(Guid simulationId)
            {
                // Do Nothing
            }

            var AdminCRUDMethods = new NoLibraryCRUDMethods<AnalysisMethodDTO>()
            {
                UpsertScenario = UpsertAny,
                DeleteScenario = DeleteAny,
                RetrieveScenario = GetAny
            };

            var PermittedCRUDMethods = new NoLibraryCRUDMethods<AnalysisMethodDTO>()
            {
                UpsertScenario = UpsertPermitted,
                DeleteScenario = DeleteAny,
                RetrieveScenario = GetPermitted
            };

            return new Dictionary<string, NoLibraryCRUDMethods<AnalysisMethodDTO>>()
            {
                [Role.Administrator] = AdminCRUDMethods,
                [Role.DistrictEngineer] = PermittedCRUDMethods,
                [Role.Cwopa] = PermittedCRUDMethods,
                [Role.PlanningPartner] = PermittedCRUDMethods
            };
        }

        //private Dictionary<string, AnalysisMethodGetMethod> CreateGetMethods()
        //{
        //    AnalysisMethodDTO GetAny(Guid simulationId)
        //    {
        //        var analysisMethodDTO = UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);
        //        SetDefaultDataForNewScenario(analysisMethodDTO);
        //        return analysisMethodDTO;
        //    }

        //    AnalysisMethodDTO GetPermitted(Guid simulationId)
        //    {
        //        CheckUserSimulationReadAuthorization(simulationId);
        //        return GetAny(simulationId);
        //    }

        //    return new Dictionary<string, AnalysisMethodGetMethod>
        //    {
        //        [Role.Administrator] = GetAny,
        //        [Role.DistrictEngineer] = GetPermitted,
        //        [Role.Cwopa] = GetPermitted,
        //        [Role.PlanningPartner] = GetPermitted
        //    };
        //}

        private void SetDefaultDataForNewScenario(AnalysisMethodDTO analysisMethodDTO)
        {            
            if (analysisMethodDTO.Attribute == null && analysisMethodDTO.Benefit.Id == Guid.Empty && analysisMethodDTO.CriterionLibrary.Id == Guid.Empty)
            {
                var analysisDefaultData = _analysisDefaultDataService.GetAnalysisDefaultData().Result;
                analysisMethodDTO.Attribute = analysisDefaultData.Weighting;
                analysisMethodDTO.OptimizationStrategy = analysisDefaultData.OptimizationStrategy;
                analysisMethodDTO.Benefit.Attribute = analysisDefaultData.BenefitAttribute;
                analysisMethodDTO.Benefit.Limit = analysisDefaultData.BenefitLimit;
                analysisMethodDTO.SpendingStrategy = analysisDefaultData.SpendingStrategy;
            }
        }

        //private Dictionary<string, AnalysisMethodUpsertMethod> CreateUpsertMethods()
        //{
        //    void UpsertAny(Guid simulationId, AnalysisMethodDTO dto) =>
        //        UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto);

        //    void UpsertPermitted(Guid simulationId, AnalysisMethodDTO dto)
        //    {
        //        CheckUserSimulationModifyAuthorization(simulationId);
        //        UpsertAny(simulationId, dto);
        //    }

        //    return new Dictionary<string, AnalysisMethodUpsertMethod>
        //    {
        //        [Role.Administrator] = UpsertAny,
        //        [Role.DistrictEngineer] = UpsertPermitted,
        //        [Role.Cwopa] = UpsertPermitted,
        //        [Role.PlanningPartner] = UpsertPermitted
        //    };
        //}

        [HttpGet]
        [Route("GetAnalysisMethod/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> AnalysisMethod(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    //_analysisMethodGetMethods[UserInfo.Role](simulationId));
                    _analysisMethodMethods[UserInfo.Role].RetrieveScenario(simulationId));

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
                    //_analysisMethodUpsertMethods[UserInfo.Role](simulationId, dto);
                    _analysisMethodMethods[UserInfo.Role].UpsertScenario(simulationId, dto);
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
