using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NuGet.Packaging;

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
        [Route("GetKeyProperties")]
        [Authorize]
        public async Task<IActionResult> GetKeyProperties() =>
            Ok(_assetData.KeyProperties.Keys.ToList());

        [HttpGet]
        [Route("GetValuesForKey/{propertyName}")]
        [Authorize]
        public async Task<IActionResult> GetValuesForKey(string propertyName)
        {
            if (!_assetData.KeyProperties.ContainsKey(propertyName)) return BadRequest($"Requested key property ({propertyName}) does not exist");
            return Ok(_assetData.KeyProperties[propertyName].Select(_ => _.KeyValue.Value).ToList());
        }

        // TODO: Remove this once front end can handle generic properties
        [HttpGet]
        [Route("GetPennDOTInventory")]
        [Authorize]
        public async Task<IActionResult> GetPennDOTInventory()
        {
            var data = new List<KeyIDs>();

            if (_assetData.KeyProperties.ContainsKey("BRKEY_") && _assetData.KeyProperties.ContainsKey("BMSID"))
            {
                data = _assetData.KeyProperties["BMSID"].Join(
                    _assetData.KeyProperties["BRKEY_"],
                    assetIDBMS => assetIDBMS.AssetId,
                    assetBrKey => assetBrKey.AssetId,
                    (bmsid, brkey)
                    => new KeyIDs { BrKey = brkey.KeyValue.TextValue, BmsId = bmsid.KeyValue.TextValue })
                    .ToList();
            }

            return Ok(data.OrderBy(_ => _.BrKey.Length).ThenBy(_ => _.BrKey));
        }
        [HttpGet]
        [Route("GetInventory/{keyProperty}")]
        [Authorize]
        public async Task<IActionResult> GetInventory(string keyProperty)
        {
            var data = new List<KeyIDs>();
            if (_assetData.KeyProperties.ContainsKey(keyProperty))
            {
                data = _assetData.KeyProperties[keyProperty].Join(
                    _assetData.KeyProperties["BRKEY_"],
                    assetKp => assetKp.AssetId,
                    assetBrKey => assetBrKey.AssetId,
                    (keyid, brkey) => new KeyIDs { BrKey = brkey.KeyValue.TextValue, BmsId = keyid.KeyValue.TextValue }).ToList();
            }
            return Ok(data.OrderBy(_ => _.BrKey.Length).ThenBy(_ => _.BrKey));
        }
    }
}
