using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Security;
using Humanizer;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;
using AppliedResearchAssociates.iAM.Reporting;
using System.Linq;
using static BridgeCareCore.Security.SecurityConstants;
using System.Data;

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminDataController : BridgeCareCoreBaseController
    {
        public const string AdminSettingError = "Admin Data Settings Error";
        public const string ServerWarning = "Server Warning:";
        private readonly IReportGenerator _generator;
        private readonly IReportLookupLibrary _factory;
        public AdminDataController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor, IReportGenerator generator, IReportLookupLibrary factory) :
                         base(esecSecurity, unitOfWork, hubService, contextAccessor)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }
        [HttpGet]
        [Route("GetKeyFields")]
        [Authorize]
        public async Task<IActionResult> GetKeyFields()
        {
            try
            {
                var KeyFields = UnitOfWork.AdminSettingsRepo.GetKeyFields();
                return Ok(KeyFields);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetKeyFields - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetKeyFields/{KeyFields}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetKeyFields(string KeyFields)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminSettingsRepo.SetKeyFields(KeyFields);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetKeyFields - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetRawDataKeyFields")]
        [Authorize]
        public async Task<IActionResult> GetRawDataKeyFields()
        {
            try
            {
                var KeyFields = UnitOfWork.AdminSettingsRepo.GetRawDataKeyFields();
                return Ok(KeyFields);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetRawDataKeyFields - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetRawDataKeyFields/{KeyFields}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetRawDataKeyFields(string KeyFields)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminSettingsRepo.SetRawDataKeyFields(KeyFields);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetRawDataKeyFields - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPrimaryNetwork")]
        [Authorize]
        public async Task<IActionResult> GetPrimaryNetwork()
        {
            try
            {
                var name = UnitOfWork.AdminSettingsRepo.GetPrimaryNetwork();
                if (name == null)
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, $"{ServerWarning} Primary Network not set::A primary network key must be set in the administration settings");
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetRawDataNetwork")]
        [Authorize]
        public async Task<IActionResult> GetRawDataNetwork()
        {
            try
            {
                var name = UnitOfWork.AdminSettingsRepo.GetRawDataNetwork();
                if (name == null)
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, $"{ServerWarning} Raw Data Network not set::A Raw Data network key must be set in the administration settings");
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetRawDataNetwork - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetPrimaryNetwork/{name}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetPrimaryNetwork(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminSettingsRepo.SetPrimaryNetwork(name);
                });
                    return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetPrimaryNetwork - {e.Message}");
                return BadRequest($"{AdminSettingError}::SetPrimaryNetwork - {e.Message}");
            }
        }

        [HttpPost]
        [Route("SetRawDataNetwork/{name}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetRawDataNetwork(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminSettingsRepo.SetRawDataNetwork(name);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetRawDataNetwork - {e.Message}");
                return BadRequest($"{AdminSettingError}::SetRawDataNetwork - {e.Message}");
            }
        }

        [HttpGet]
        [Route("GetSimulationReportNames")]
        [Authorize]
        public async Task<IActionResult> GetSimulationReportNames()
        {
            try
            {
                var SimulationReportNames = UnitOfWork.AdminSettingsRepo.GetSimulationReportNames();
                return Ok(SimulationReportNames);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetSimulationReportNames - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAvailableReports")]
        [Authorize]
        public async Task<IActionResult> GetAvailableReports()
        {
            try
            {
                List<string> AvailableReportNames = new List<string>();
                foreach (IReportFactory report in _factory.ReportList)
                {
                    AvailableReportNames.Add(report.Name);
                }
                return Ok(AvailableReportNames);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetAvailableReports - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetInventoryReports/{InventoryReports}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetInventoryReports(string inventoryReports)
        {
            try
            {
                var reportCriteriaCheck = true;
                IList<string> InventoryReportsList = inventoryReports.Split(',').ToList();
                   
                //Checking every report being passed in from the parameter
                foreach (string inventoryReport in InventoryReportsList)
                {
                    try
                    {
                        var unTypedReport = inventoryReport.Substring(0, inventoryReport.Length - 3);
                        var reportObject = await _generator.Generate(unTypedReport);
                        
                        //If cannot be created in lookup library (Existence Check)
                        if (!_factory.CanGenerateReport(unTypedReport))
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {unTypedReport} for an inventory report.");
                        }
                        //Report type isn't HTML
                        else if (reportObject.Type != ReportType.HTML)
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {unTypedReport} for an inventory report.");
                        }                       
                    }
                    catch (Exception e)
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetInventoryReports - {e.Message}");
                        return BadRequest($"{AdminSettingError}::SetInventoryReports - {e.Message}");
                    }
                    
                };
                //If all reports in list exist and use the right type, save to database.
                if (reportCriteriaCheck)
                {
                    UnitOfWork.AdminSettingsRepo.SetInventoryReports(inventoryReports);
                }
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetInventoryReports - {e.Message}");
                return BadRequest($"{AdminSettingError}::SetInventoryReports - {e.Message}");
            }
        }


        [HttpGet]
        [Route("GetInventoryReports")]
        [Authorize]
        public async Task<IActionResult> GetInventoryReports()
        {
            try
            {
                var SimulationReportNames = UnitOfWork.AdminSettingsRepo.GetInventoryReports();
                return Ok(SimulationReportNames);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetInventoryReportNames - {e.Message}");
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("GetAttributeName")]
        [ClaimAuthorize("AttributesViewAccess")]
        public async Task<IActionResult> Attributes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAttributesAsync();
                var allAttributes = await UnitOfWork.AttributeRepo.GetAttributesAsync();
                var attributeNameLookup = new Dictionary<Guid, string>();
                foreach (var attribute in allAttributes)
                {
                    attributeNameLookup[attribute.Id] = attribute.Name;
                }
                //return attributeNameLookup;
                return Ok(attributeNameLookup);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetAttributeName - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetSimulationReports/{SimulationReports}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetSimulationReports(string simulationReports)
        {
            try
            {
                var reportCriteriaCheck = true;
                IList<string> SimulationReportsList = simulationReports.Split(',').ToList();

                //Checking every report being passed in from the parameter
                foreach (string simulationReport in SimulationReportsList)
                {
                    try
                    {
                        var reportObject = await _generator.Generate(simulationReport);
                        //If cannot be created in lookup library (Existence Check)
                        if (!_factory.CanGenerateReport(simulationReport))
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {simulationReport} for an simulation report.");
                        }
                        //Report type isn't File Type
                        else if (reportObject.Type != ReportType.File)
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {simulationReport} for an simulation report.");
                        }
                    }
                    catch (Exception e)
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetSimulationReports - {e.Message}");
                        return BadRequest($"{AdminSettingError}::SetSimulationReports - {e.Message}");
                    }

                };
                //If all reports in list exist and use the right type, save to database.
                if (reportCriteriaCheck)
                {
                    UnitOfWork.AdminSettingsRepo.SetSimulationReports(simulationReports);
                }
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetSimulationReports - {e.Message}");
                return BadRequest($"{AdminSettingError}::SetSimulationReports - {e.Message}");
            }
        }

        [HttpGet]
        [Route("GetConstraintType")]
        public async Task<IActionResult> GetConstraintType()
        {
            try
            {
                var result = await Task.Factory.StartNew(() => UnitOfWork.AdminSettingsRepo.GetConstraintType());
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::GetConstraintType - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetConstraintType/{constraintType}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetConstraintType(string constraintType)
        {
            try
            {
                await Task.Factory.StartNew(() => UnitOfWork.AdminSettingsRepo.SetConstraintType(constraintType));
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AdminSettingError}::SetConstraintType - {e.Message}");
                throw;
            }
        }

    }
}
