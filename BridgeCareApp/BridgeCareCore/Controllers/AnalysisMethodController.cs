﻿using System;
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

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisMethodController : BridgeCareCoreBaseController
    {       
        public readonly IAnalysisDefaultDataService _analysisDefaultDataService;
        private readonly IClaimHelper _claimHelper;
        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public AnalysisMethodController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor, IAnalysisDefaultDataService analysisDefaultDataService, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {            
            _analysisDefaultDataService = analysisDefaultDataService ?? throw new ArgumentNullException(nameof(analysisDefaultDataService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");                
                throw;
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Method error::{e.Message}");
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
