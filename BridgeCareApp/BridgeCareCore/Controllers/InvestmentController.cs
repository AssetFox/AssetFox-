using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services.General_Work_Queue.WorkItems;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : BridgeCareCoreBaseController
    {
        public const string InvestmentError = "Investment Error";
        public const string DeteriorationModelError = "Deterioration Model Error";
        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        private static IInvestmentBudgetsService _investmentBudgetsService;
        private static IInvestmentPagingService _investmentPagingService;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;
        public readonly IGeneralWorkQueueService _generalWorkQueueService;
        private readonly IClaimHelper _claimHelper;

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public InvestmentController(
            IInvestmentBudgetsService investmentBudgetsService,
            IInvestmentPagingService investmentPagingService,
            IEsecSecurity esecSecurity,
            IUnitOfWork unitOfWork,
            IHubService hubService,
            IHttpContextAccessor httpContextAccessor,
            IInvestmentDefaultDataService investmentDefaultDataService,
            IClaimHelper claimHelper,
            IGeneralWorkQueueService generalWorkQueueService) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _investmentBudgetsService = investmentBudgetsService ?? throw new ArgumentNullException(nameof(investmentBudgetsService));
            _investmentPagingService = investmentPagingService ?? throw new ArgumentNullException(nameof(investmentPagingService));
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
            this._generalWorkQueueService = generalWorkQueueService ?? throw new ArgumentNullException(nameof(generalWorkQueueService));
        }

        [HttpPost]
        [Route("GetScenarioInvestmentPage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioInvestmentPage(Guid simulationId, InvestmentPagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentPagingService.GetScenarioPage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetScenarioInvestmentPage - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryInvestmentPage/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetLibraryInvestmentPage(Guid libraryId, InvestmentPagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentPagingService.GetLibraryPage(libraryId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{DeteriorationModelError}::GetLibraryInvestmentPage - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetInvestment(Guid simulationId)
        {
            try
            {
                var result = new InvestmentDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = GetForScenario(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetInvestment for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetInvestment for {simulationName} - {e.Message}");
                throw;
            }
        }
                
        [HttpPost]
        [Route("UpsertInvestment/{simulationId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromScenario)]
        public async Task<IActionResult> UpsertInvestment(Guid simulationId, InvestmentPagingSyncModel pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);

                    var dtos = _investmentPagingService.GetSyncedScenarioDataSet(simulationId, pagingSync);
                    UnitOfWork.BudgetRepo.AddModifiedToScenarioBudget(dtos, pagingSync.IsModified);
                    UnitOfWork.BudgetRepo.AddLibraryIdToScenarioBudget(dtos, pagingSync.LibraryId);

                    InvestmentDTO investment = new InvestmentDTO();
                    var investmentPlan = pagingSync.Investment;
                    investment.ScenarioBudgets = dtos;
                    investment.InvestmentPlan = investmentPlan;
                    
                    UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgetsWithInvestmentPlan(dtos, investmentPlan, simulationId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::UpsertInvestment for {simulationName} - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                var simulationName = UnitOfWork.SimulationRepo.GetSimulationNameOrId(simulationId);
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::UpsertInvestment for {simulationName} - {e.Message}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetBudgetLibraries")]
        [Authorize(Policy = Policy.ViewInvestmentFromLibrary)]
        public async Task<IActionResult> GetBudgetLibraries()
        {
            try
            {
                var result = new List<BudgetLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildrenAccessibleToUser(UserId);
                    }
                    else
                    {
                        result = UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetBudgetLibraries - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetLibrary")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> UpsertBudgetLibrary(InvestmentLibraryUpsertPagingRequestModel upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryAccess = UnitOfWork.BudgetRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                    var budgets = new List<BudgetDTO>();
                    if (upsertRequest.ScenarioId != null)
                        budgets = _investmentPagingService.GetSyncedScenarioDataSet(upsertRequest.ScenarioId.Value, upsertRequest.SyncModel);
                    else if (upsertRequest.SyncModel.LibraryId != null && upsertRequest.SyncModel.LibraryId != Guid.Empty)
                        budgets = _investmentPagingService.GetSyncedLibraryDataset(upsertRequest.SyncModel.LibraryId.Value, upsertRequest.SyncModel);
                    else if (!upsertRequest.IsNewLibrary)
                        budgets = _investmentPagingService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.SyncModel);
                    else if (upsertRequest.IsNewLibrary && upsertRequest.SyncModel.LibraryId == Guid.Empty)
                    {
                        budgets = _investmentPagingService.GetNewLibraryDataset(upsertRequest.SyncModel);
                    }

                    if (upsertRequest.IsNewLibrary)
                        budgets.ForEach(budget =>
                        {
                            budget.Id = Guid.NewGuid();
                            budget.BudgetAmounts.ForEach(_ => _.Id = Guid.NewGuid());
                        });
                    var dto = upsertRequest.Library;
                    dto.Budgets = budgets;
                    if (upsertRequest.IsNewLibrary)
                    {
                        UnitOfWork.BudgetRepo.CreateNewBudgetLibrary(dto, UserId);

                    } else
                    {
                        UnitOfWork.BudgetRepo.UpdateBudgetLibraryAndUpsertOrDeleteBudgets(dto);
                    }
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (RowNotInTableException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::UpsertBudgetLibrary - {HubService.errorList["RowNotInTable"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::UpsertBudgetLibrary - {HubService.errorList["Exception"]}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetBudgetLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetBudgetLibraryUsers(Guid libraryId)
        {
            try
            {
                List<LibraryUserDTO> users = new List<LibraryUserDTO>();
                await Task.Factory.StartNew(() =>
                {
                    var accessModel = UnitOfWork.BudgetRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckGetLibraryUsersValidity(accessModel, UserId);
                    users = UnitOfWork.BudgetRepo.GetLibraryUsers(libraryId);
                });
                return Ok(users);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertOrDeleteBudgetLibraryUsers/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> UpsertOrDeleteBudgetLibraryUsers(Guid libraryId, List<LibraryUserDTO> proposedUsers)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var libraryUsers = UnitOfWork.BudgetRepo.GetLibraryUsers(libraryId);
                    _claimHelper.CheckAccessModifyValidity(libraryUsers, proposedUsers, UserId);
                    UnitOfWork.BudgetRepo.UpsertOrDeleteUsers(libraryId, proposedUsers);
                });
                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return BadRequest();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetLibrary/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> DeleteBudgetLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var access = UnitOfWork.BudgetRepo.GetLibraryAccess(libraryId, UserId);
                    _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);

                    UnitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);
                });

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::DeleteBudgetLibrary - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::DeleteBudgetLibrary - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSimpleBudgetDetails/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    return UnitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetScenarioSimpleBudgetDetails - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetInvestmentPlan/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetInvestmentPlan(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    return UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetInvestmentPlan - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportLibraryInvestmentBudgetsExcelFile")]
        [Authorize(Policy = Policy.ImportInvestmentFromLibrary)]
        public async Task<IActionResult> ImportLibraryInvestmentBudgetsExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Investment budgets file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var libraryId))
                {
                    throw new ConstraintException("Request contained no budget library id.");
                }

                var budgetLibraryId = Guid.Parse(libraryId.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }

                var result = new BudgetImportResultDTO();
                var budgetLibraryName = "";
                await Task.Factory.StartNew(() =>
                {
                    var existingBudgetLibrary = UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildren().FirstOrDefault(_ => _.Id == budgetLibraryId);
                    if(existingBudgetLibrary != null)
                        budgetLibraryName = existingBudgetLibrary.Name;

                    if (_claimHelper.RequirePermittedCheck())
                    {                    
                        if (existingBudgetLibrary != null)
                        {                           
                            var accessModel = UnitOfWork.BudgetRepo.GetLibraryAccess(budgetLibraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
                        }
                    }
                });
              
                ImportLibraryInvestmentWorkitem workItem = new ImportLibraryInvestmentWorkitem(budgetLibraryId, excelPackage, currentUserCriteriaFilter, overwriteBudgets, UserInfo.Name, budgetLibraryName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = budgetLibraryId, Status = analysisHandle.MostRecentStatusMessage });
                }
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ImportLibraryInvestmentBudgetsExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ImportLibraryInvestmentBudgetsExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioInvestmentBudgetsExcelFile")]
        [Authorize(Policy = Policy.ImportInvestmentFromScenario)]
        public async Task<IActionResult> ImportScenarioInvestmentBudgetsExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Investment budgets file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }

                var simulationName = "";
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    simulationName = UnitOfWork.SimulationRepo.GetSimulationName(simulationId);
                });

                ImportScenarioInvestmentWorkitem workItem = new ImportScenarioInvestmentWorkitem(simulationId, excelPackage, currentUserCriteriaFilter, overwriteBudgets, UserInfo.Name, simulationName);
                var analysisHandle = _generalWorkQueueService.CreateAndRun(workItem);
                // Before sending a "queued" message that may overwrite early messages from the run,
                // allow a brief moment for an empty queue to start running the submission.
                await Task.Delay(500);
                if (!analysisHandle.WorkHasStarted)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWorkQueueStatusUpdate, new QueuedWorkStatusUpdateModel() { Id = simulationId, Status = analysisHandle.MostRecentStatusMessage });
                }
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ImportScenarioInvestmentBudgetsExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ImportScenarioInvestmentBudgetsExcelFile - { e.Message }");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportScenarioInvestmentBudgetsExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioInvestmentBudgetsExcelFile(Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportScenarioInvestmentBudgetsFile(simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ExportScenarioInvestmentBudgetsExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ExportScenarioInvestmentBudgetsExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportLibraryInvestmentBudgetsExcelFile/{budgetLibraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportLibraryInvestmentBudgetsExcelFile(Guid budgetLibraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportLibraryInvestmentBudgetsFile(budgetLibraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ExportLibraryInvestmentBudgetsExcelFile - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::ExportLibraryInvestmentBudgetsExcelFile - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadInvestmentBudgetsTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadInvestmentBudgetsTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\Investment_budgets_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "Investment_budgets_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::DownloadInvestmentBudgetsTemplate - {HubService.errorList["Unauthorized"]}");
                throw;
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::DownloadInvestmentBudgetsTemplate - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("GetScenarioBudgetYears/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetScenarioBudgetYears(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    return UnitOfWork.BudgetRepo.GetBudgetYearsBySimulationId(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{InvestmentError}::GetScenarioBudgetYears - {e.Message}");
                throw;
            }
        }

        private InvestmentDTO GetForScenario(Guid scenarioId)
        {
            var budgets = UnitOfWork.BudgetRepo.GetScenarioBudgets(scenarioId);

            var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(scenarioId);
            if (scenarioInvestmentPlan.Id == Guid.Empty)
            {
                var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                scenarioInvestmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                scenarioInvestmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
            }

            return new InvestmentDTO
            {
                ScenarioBudgets = budgets,
                InvestmentPlan = scenarioInvestmentPlan
            };
        }
    }
}
