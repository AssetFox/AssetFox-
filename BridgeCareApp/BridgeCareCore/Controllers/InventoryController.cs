using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
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

        [HttpGet]
        [Route("GetInventory/{keyProperty1}/{keyProperty2}")]
        [Authorize]
        public async Task<IActionResult> GetInventory(string keyProperty1, string keyProperty2)
        {
            var data = new List<KeyIDs>();
            if (_assetData.KeyProperties.ContainsKey(keyProperty1))
            {
                data = _assetData.KeyProperties[keyProperty1].Join(
                    _assetData.KeyProperties[keyProperty2],
                    assetKeyProperty1 => assetKeyProperty1.AssetId,
                    assetKeyProperty2 => assetKeyProperty2.AssetId,
                    (keyid1, keyid2) => new KeyIDs { KeyProperty2 = keyid2.KeyValue.TextValue, KeyProperty1 = keyid1.KeyValue.TextValue }).ToList();
            }
            var list = new List<string> { keyProperty1, keyProperty2 };
            return await GetInventory(list);  //Ok(data.OrderBy(_ => _.KeyProperty2.Length).ThenBy(_ => _.KeyProperty2));
        }

        [HttpGet]
        [Route("GetInventory/{keyProperties}")]
        [Authorize]
        public async Task<IActionResult> GetInventory(List<string> keyProperties)
        {
            var assetKeyData = new Dictionary<Guid, List<string>>();
            var keySegmentDatums = new List<List<KeySegmentDatum>>();

            // Todo check how to handle sort, should we just sort first key property's ?

            foreach (var keyProperty in keyProperties)
            {
                if (_assetData.KeyProperties.ContainsKey(keyProperty))
                {
                    keySegmentDatums.Add(_assetData.KeyProperties[keyProperty]);
                }
            }

            foreach (var keySegmentDatum in keySegmentDatums)
            {
                foreach (var keyDatum in keySegmentDatum)
                {
                    var assetId = keyDatum.AssetId;
                    var value = keyDatum.KeyValue.TextValue;
                    if (!assetKeyData.ContainsKey(assetId))
                    {
                        assetKeyData.Add(assetId, new List<string> { value });
                    }
                    else
                    {
                        assetKeyData[assetId].Add(value);
                    }
                }
            }            

            return Ok(assetKeyData.Values);
        }
    }
}
