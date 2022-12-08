using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
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
    public class AnalysisDefaultDataController : BridgeCareCoreBaseController
    {
        public readonly IAnalysisDefaultDataService _analysisDefaultDataService;

        public AnalysisDefaultDataController(IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IAnalysisDefaultDataService analysisDefaultDataService) :
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Analysis Default Data Error::GetAnalysisDefaultData - { e.Message }");
                throw;
            }
        }
    }
}
