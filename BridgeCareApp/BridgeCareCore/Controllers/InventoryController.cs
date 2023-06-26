using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        [HttpPost]
        [Route("GetQuery")]
        [Authorize]
        public async Task<IActionResult> GetQuery()
        {
            var parameters = await GetParameters();
            try {
                var keyQuery = JsonConvert.DeserializeObject<List<string>>(parameters);
            }
            catch
            {
                return BadRequest("Unable to parse content");
            }

            var result = new List<QueryResponse>()
            {
                new QueryResponse() {Attribute = "COUNTY", Values = new List<string> {"ADAMS"} },
                new QueryResponse() {Attribute = "SR", Values = new List<string> {"1", "2", "3"} },
                new QueryResponse() {Attribute = "CRS", Values = new List<string> {"Sec_1", "Sec_2", "Sec_3"} },
            };

            return Ok(result);
        }

        private class QueryResponse
        {
            public string Attribute { get; set; }
            public List<string> Values { get; set; }
        }

        private async Task<string> GetParameters()
        {
            // Manually bring in the body JSON as doing so in the parameters (i.e., [FromBody] JObject parameters) will fail when the body does not exist
            var parameters = string.Empty;
            if (Request.ContentLength > 0)
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                parameters = await reader.ReadToEndAsync();
            }

            return parameters;
        }

    }
}
