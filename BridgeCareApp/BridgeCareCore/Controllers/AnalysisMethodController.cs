using System;
using System.Threading.Tasks;
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
using BridgeCareCore.Utils.Interfaces;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using System.Data;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : BridgeCareCoreBaseController
    {
        public const string AnalysisMethodError = "Analysis Method Error";
        public readonly IAnalysisDefaultDataService _analysisDefaultDataService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AnalysisMethodController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IAnalysisDefaultDataService analysisDefaultDataService, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {            
            _analysisDefaultDataService = analysisDefaultDataService ?? throw new ArgumentNullException(nameof(analysisDefaultDataService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

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

        [HttpGet]
        [Route("GetAnalysisMethod/{simulationId}")]
        [Authorize(Policy = Policy.ViewAnalysisMethod)]
        public async Task<IActionResult> AnalysisMethod(Guid simulationId)
        {
            try
            {
                var result = new AnalysisMethodDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = UnitOfWork.AnalysisMethodRepo.GetAnalysisMethod(simulationId);
                    SetDefaultDataForNewScenario(result);
                });

                return Ok(result);
            }
            catch(UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::GetAnalysisMethod for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::GetAnalysisMethod for {simulationName} - {e.Message}", e);
                return Ok();
            }
        }

        [HttpGet]
        [Route("GetSimulationAnalysisSetting/{simulationId}")]
        [Authorize(Policy = Policy.ViewAnalysisMethod)]
        public async Task<IActionResult> SimulationAnalysisSetting(Guid simulationId)
        {


            try
            {
                var result = UnitOfWork.AnalysisMethodRepo.GetSimulationAnalysisMethodSetting(simulationId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::GetSimulationAnalysisMethodSetting for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::GetSimulationAnalysisMethodSetting for {simulationName} - {e.Message}", e);
                return Ok();
            }
        }


        [HttpPost]
        [Route("UpsertAnalysisMethod/{simulationId}")]
        [Authorize(Policy = Policy.ModifyAnalysisMethod)]
        public async Task<IActionResult> UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    UnitOfWork.AnalysisMethodRepo.UpsertAnalysisMethod(simulationId, dto);                    
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::UpsertAnalysisMethod for {simulationName} - {HubService.errorList["Unauthorized"]}", e);
                return Ok();
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AnalysisMethodError}::UpsertAnalysisMethod for {simulationName} - {e.Message}", e);
                return Ok();
            }
        }
    }
}
