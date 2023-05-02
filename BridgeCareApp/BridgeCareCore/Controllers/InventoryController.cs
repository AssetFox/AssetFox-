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
using Newtonsoft.Json;

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
        
        // TODO update UI to call new API, check from where to get keys, setting?

        [HttpGet]
        [Route("GetInventory")]
        [Authorize]
        public async Task<IActionResult> GetInventory(string keyProperties)
        {
            var assetKeyData = new Dictionary<Guid, List<string>>();
            var keySegmentDatums = new List<List<KeySegmentDatum>>();
                        
            var keyPropertiesList = JsonConvert.DeserializeObject<List<string>>(keyProperties);
            foreach (var keyProperty in keyPropertiesList)
            {
                if (_assetData.KeyProperties.ContainsKey(keyProperty))
                {
                    keySegmentDatums.Add(_assetData.KeyProperties[keyProperty]);
                }
            }
            
            foreach (var keySegmentDatum in keySegmentDatums)
            {
                foreach (var keyDatum in keySegmentDatum.OrderBy(_ => _.KeyValue.TextValue))
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

            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach(var assetKeyDataValue in  assetKeyData.Values)
            {
                inventoryItems.Add(new InventoryItem { keyProperties = assetKeyDataValue });
            }

            return Ok(inventoryItems);
        }
    }
}
