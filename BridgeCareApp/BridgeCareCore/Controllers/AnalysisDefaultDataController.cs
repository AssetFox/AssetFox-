using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisDefaultDataController : BridgeCareCoreBaseController
    {
        public readonly IAnalysisDefaultDataService _analysisDefaultDataService;

        public AnalysisDefaultDataController(IAnalysisDefaultDataService analysisDefaultDataService, IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _analysisDefaultDataService = analysisDefaultDataService ?? throw new ArgumentNullException(nameof(analysisDefaultDataService));

        [HttpGet]
        [Route("GetAnalysisDefaultData")]
        [Authorize]
        public async Task<IActionResult> GetAnalysisDefaultData()
        {
            try
            {
                var result = await _analysisDefaultDataService.GetAnalysisDefaultData();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis default data configuration read error::{e.Message}");
                throw;
            }
        }
    }
}
