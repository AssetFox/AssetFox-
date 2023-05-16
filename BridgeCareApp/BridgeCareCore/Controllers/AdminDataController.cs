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

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminDataController : BridgeCareCoreBaseController
    {
        public const string SiteError = "Site Error";

        public AdminDataController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor) :
                         base(esecSecurity, unitOfWork, hubService, contextAccessor)
        { }


        [HttpGet]
        [Route("GetKeyFields")]
        [Authorize]
        public async Task<IActionResult> GetKeyFields()
        {
            try
            {
                var KeyFields = UnitOfWork.AdminDataRepo.GetKeyFields();
                return Ok(KeyFields);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetPrimaryNetwork - {e.Message}");
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
                    UnitOfWork.AdminDataRepo.SetKeyFields(KeyFields);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetInventoryReports/{InventoryReports}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetInventoryRepos(string inventoryReports)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    //To verify existence of reports since repository cannot
                    var reportExistence = false;
                    var reportFactoryList = new List<IReportFactory>();
                    reportFactoryList.Add(new BAMSInventoryReportFactory());
                    reportFactoryList.Add(new PAMSInventorySectionsReportFactory());
                    reportFactoryList.Add(new PAMSInventorySegmentsReportFactory());
                    ReportLookupLibrary library = new ReportLookupLibrary(reportFactoryList);
                    IList<string> InventoryReportsList = inventoryReports.Split(',').ToList();
                    
                  
                    foreach (string inventoryReport in InventoryReportsList)
                    {
                        //If report is in factory list
                        if (library.CanGenerateReport(inventoryReport)== true)
                        {
                            reportExistence= true;
                        }
                        else
                        {
                            reportExistence= false;
                            throw new Exception("Report Type Does Not Exist.");
                        }
                        
                    }
                    //If all reports in list exist, save to database.
                    if (reportExistence)
                    {
                        UnitOfWork.AdminDataRepo.SetInventoryReports(inventoryReports);
                    }
                    
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

    }
}
