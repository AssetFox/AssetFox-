using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : BridgeCareCoreBaseController
    {
        private readonly IAssetData _assetData;

        public InventoryController(IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _assetData = unitOfWork.AssetDataRepository;
        }

        [HttpGet]
        [Route("GetInventory")]
        [Authorize]
        public async Task<IActionResult> GetInventory()
        {
            var data = new List<BMSIDAndBRKeyDTO>();

            // TODO:  Need to figure out a way to make this more generic.  Another setting in appSettings.json?

            if (_assetData.KeyProperties.ContainsKey("BRKEY") && _assetData.KeyProperties.ContainsKey("BMSID"))
            {
                data = _assetData.KeyProperties["BMSID"].Join(
                    _assetData.KeyProperties["BRKEY"],
                    assetIDBMS => assetIDBMS.AssetId,
                    assetBrKey => assetBrKey.AssetId,
                    (bmsid, brkey)
                    => new BMSIDAndBRKeyDTO { BrKey = brkey.KeyValue.TextValue, BmsId = bmsid.KeyValue.TextValue })
                    .ToList();
            }

            return Ok(data.OrderBy(_ => _.BrKey.Length).ThenBy(_ => _.BrKey));
        }
    }
}
