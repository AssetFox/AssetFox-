using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : BridgeCareCoreBaseController
    {
        private readonly IAssetData _assetData;
        private readonly IAttributeRepository _attributeRepository;
        private readonly IAssetData _maintainableAssetRepository;
        private readonly IAdminSettingsRepository _adminSettingsRepository;

        public InventoryController(IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _assetData = unitOfWork.AssetDataRepository;
            _attributeRepository = unitOfWork.AttributeRepo;
            _maintainableAssetRepository = unitOfWork.AssetDataRepository;
            _adminSettingsRepository = unitOfWork.AdminSettingsRepo;
        }

        [HttpGet]
        [Route("GetKeyProperties")]
        [Authorize]
        public async Task<IActionResult> GetKeyProperties() =>
            Ok(_assetData.KeyProperties.Keys.ToList());

        [HttpGet]
        [Route("GetValuesForPrimaryKey/{propertyName}")]
        [Authorize]
        public async Task<IActionResult> GetValuesForPrimaryKey(string propertyName)
        {
            if (!_assetData.KeyProperties.ContainsKey(propertyName)) return BadRequest($"Requested key property ({propertyName}) does not exist");
            return Ok(_assetData.KeyProperties[propertyName].Select(_ => _.KeyValue.Value).ToList());
        }

        [HttpGet]
        [Route("GetValuesForRawKey/{propertyName}")]
        [Authorize]
        public async Task<IActionResult> GetValuesForRawKey(string propertyName)
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
            var dictionaryProperties = new Dictionary<Guid, List<string>>();

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

            var temp = new List<string>(); 
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach(var assetKeyDataValue in  assetKeyData.Values)
            {
                if(temp != assetKeyDataValue)
                {
                    inventoryItems.Add(new InventoryItem { keyProperties = assetKeyDataValue });
                }
                temp = assetKeyDataValue;
            }

            // Remove the duplicates
            //for (int i = 0; i < inventoryItems.Count - 1; i++)
            //{
            //    for (int j = i + 1; j < inventoryItems.Count; j++)
            //    {
            //        if (inventoryItems[i].keyProperties[0] == inventoryItems[j].keyProperties[0])
            //        {
            //            inventoryItems.RemoveAt(j);

            //            j--;
            //        }
            //    }
            //}

            return Ok(inventoryItems);
        }

        [HttpPost]
        [Route("GetQuery")]
        [Authorize]
        public async Task<IActionResult> GetQuery()
        {
            var parameters = await GetParameters();
            string[] keyQuery;

            // Deseralize & validate the provided parameters
            try {
                keyQuery = JsonConvert.DeserializeObject<string[]>(parameters);
            }
            catch
            {
                return BadRequest("Unable to parse content");
            }
            if (keyQuery.Length % 2 != 0) return BadRequest("Provided parameters had an odd number of parameters");

            // Ensure the attributes are real and get their objects
            var attributeList = new Dictionary<AttributeDTO, string>();
            for (int index = 0; index < keyQuery.Length; index += 2)
            {
                var attribute = _attributeRepository.GetSingleByName(keyQuery[index]);
                if (attribute == null) return BadRequest($"Unable to find attribute {keyQuery[index]}");
                attributeList.Add(attribute, keyQuery[index+1]);
            }

            // Get the data from the repo
            List<MaintainableAssetQueryDTO> queryData;
            var reportTypeParam = _adminSettingsRepository.GetInventoryReports();

            try
            {
                if (reportTypeParam[0].Contains("(P)"))
                {
                    queryData = _maintainableAssetRepository.QueryKeyAttributes(attributeList, NetworkTypes.Main);
                }
                else
                {
                    queryData = _maintainableAssetRepository.QueryKeyAttributes(attributeList, NetworkTypes.Raw);
                }
            }
            catch (RowNotInTableException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Key attribute query failed with an unknown error");
            }

            // Flatten out data

            var result = new List<QueryResponse>();
            if (reportTypeParam[0].Contains("(P)"))
            {
                foreach (var field in _adminSettingsRepository.GetKeyFields())
                {
                    result.Add(GetUniqueForAttribute(field, queryData));
                }
            }
            else if (reportTypeParam[0].Contains("(R)"))
            {
                foreach (var field in _adminSettingsRepository.GetRawKeyFields())
                {
                    result.Add(GetUniqueForAttribute(field, queryData));
                }
            }

            return Ok(result);
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

        private QueryResponse GetUniqueForAttribute(string attribute, List<MaintainableAssetQueryDTO> data)
        {
            var assetsValues = data.SelectMany(_ => _.AssetProperties)
                .Where(_ => _.Key.Name == attribute)
                .Select(_ => _.Value)
                .Distinct()
                .ToList();
            return new QueryResponse() { Attribute = attribute, Values = assetsValues };
        }
    }
}
