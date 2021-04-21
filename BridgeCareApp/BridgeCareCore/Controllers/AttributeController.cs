using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : HubControllerBase
    {
        private readonly IEsecSecurity _esecSecurity;
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly AttributeService _attributeService;

        public AttributeController(IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            AttributeService attributeService, IHubService hubService) : base(hubService)
        {
            _esecSecurity = esecSecurity ?? throw new ArgumentNullException(nameof(esecSecurity));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
        }

        [HttpGet]
        [Route("GetAttributes")]
        [Authorize]
        public async Task<IActionResult> Attributes()
        {
            try
            {
                var result = await _unitOfWork.AttributeRepo.Attributes();
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetAttributesSelectValues")]
        [Authorize]
        public async Task<IActionResult> GetAttributeSelectValues([FromBody] List<string> attributeNames)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _attributeService.GetAttributeSelectValues(attributeNames));
                return Ok(result);
            }
            catch (Exception e)
            {
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateAttributes")]
        [Authorize]
        public async Task<IActionResult> CreateAttributes()
        {
            try
            {
                _unitOfWork.SetUser(_esecSecurity.GetUserInformation(Request).Name);

                await Task.Factory.StartNew(() =>
                {
                    var configurableAttributes = _unitOfWork.AttributeMetaDataRepo.GetAllAttributes();
                    _unitOfWork.BeginTransaction();
                    _unitOfWork.AttributeRepo.UpsertAttributes(configurableAttributes);
                    _unitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }
    }
}
