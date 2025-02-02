﻿using BridgeCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using OfficeOpenXml;
using BridgeCare.Models;
using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Security;
using Simulation;
using System.Net.Mail;
using System.Configuration;
using System.Collections.Specialized;

namespace BridgeCare.Services
{
    using CommittedProjectsGetMethod = Func<int, BridgeCareContext, UserInformationModel, List<CommittedEntity>>;
    using CommittedProjectsSaveMethod = Action<int, List<CommittedProjectModel>, BridgeCareContext, UserInformationModel>;

    public class CommittedProjects : ICommittedProjects
    {
        readonly ICommittedRepository _committedRepositoryRepo;
        private readonly ISectionsRepository _sectionsRepository;
        private static readonly SimulationQueue SimulationQueue = SimulationQueue.MainSimulationQueue;
        /// <summary>Maps user roles to methods for fetching committed projects</summary>
        private readonly IReadOnlyDictionary<string, CommittedProjectsGetMethod> CommittedProjectsGetMethods;
        /// <summary>Maps user roles to methods for saving committed projects</summary>
        private readonly IReadOnlyDictionary<string, CommittedProjectsSaveMethod> CommittedProjectsSaveMethods;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CommittedProjects));

        public CommittedProjects(ICommittedRepository committedRepositoryRepo, ISectionsRepository sectionsRepository)
        {
            this._committedRepositoryRepo = committedRepositoryRepo;
            this._sectionsRepository = sectionsRepository;

            List<CommittedEntity> GetAnyProjects(int id, BridgeCareContext db, UserInformationModel userInformation) =>
                committedRepositoryRepo.GetCommittedProjects(id, db);
            List<CommittedEntity> GetPermittedProjects(int id, BridgeCareContext db, UserInformationModel userInformation) =>
                committedRepositoryRepo.GetPermittedCommittedProjects(id, db, userInformation.Name);

            void SaveAnyProjects(int simulationId, List<CommittedProjectModel> models, BridgeCareContext db, UserInformationModel userInformation) =>
                committedRepositoryRepo.SaveCommittedProjects(simulationId, models, db);
            void SavePermittedProjects(int simulationId, List<CommittedProjectModel> models, BridgeCareContext db, UserInformationModel userInformation) =>
                committedRepositoryRepo.SavePermittedCommittedProjects(simulationId, models, db, userInformation.Name);

            CommittedProjectsGetMethods = new Dictionary<string, CommittedProjectsGetMethod>
            {
                [Role.ADMINISTRATOR] = GetAnyProjects,
                [Role.DISTRICT_ENGINEER] = GetPermittedProjects,
                [Role.CWOPA] = GetPermittedProjects,
                [Role.PLANNING_PARTNER] = GetPermittedProjects
            };
            CommittedProjectsSaveMethods = new Dictionary<string, CommittedProjectsSaveMethod>
            {
                [Role.ADMINISTRATOR] = SaveAnyProjects,
                [Role.DISTRICT_ENGINEER] = SavePermittedProjects,
                [Role.CWOPA] = SavePermittedProjects,
                [Role.PLANNING_PARTNER] = SavePermittedProjects
            };
        }

        /// <summary>
        /// Save committed projects from the template files
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="db"></param>
        public void SaveCommittedProjectsFiles(HttpRequest httpRequest, BridgeCareContext db, UserInformationModel userInformation)
        {
            if (httpRequest.Files.Count < 1)
                throw new ConstraintException("Files Not Found");

            var files = httpRequest.Files;
            List<ExcelPackage> packages = new List<ExcelPackage>();

            for (int i = 0; i < files.Count; i++) {
                packages.Add(new ExcelPackage(files[i].InputStream));
            }

            Action saveCommittedProjectsAction = () => {
                var mail = CreateAlertEmail();
                var simulationId = int.Parse(httpRequest.Form.Get("selectedScenarioId"));
                try
                {
                    var networkId = int.Parse(httpRequest.Form.Get("networkId"));
                    var applyNoTreatment = httpRequest.Form.Get("applyNoTreatment") == "1";

                    var committedProjectModels = new List<CommittedProjectModel>();

                    foreach (var package in packages)
                    {
                        GetCommittedProjectModels(package, simulationId, networkId, applyNoTreatment, committedProjectModels, db);
                    }

                    CommittedProjectsSaveMethods[userInformation.Role](simulationId, committedProjectModels, db, userInformation);

                    SetAlertMessage(mail, simulationId);
                }
                catch (Exception exception)
                {
                    SetAlertMessage(mail, simulationId, exception);
                    log.Error(exception);
                    throw exception;
                }
                /*finally
                {
                    SendAlertEmail(mail, userInformation);
                }*/
            };

            SimulationQueue.Enqueue(saveCommittedProjectsAction);
        }

        private void SetAlertMessage(MailMessage mail, int simulationId, Exception exception = null)
        {
            if (exception == null)
            {
                mail.Subject += "Completed";
                mail.Body = $"Committed Projects have finished uploading for scenario {simulationId}.";
            }
            else
            {
                mail.Subject += "Failed";
                mail.Body = $"Committed Projects failed to upload for scenario {simulationId}, due to the following exception:\n\n{exception.GetType().Name}: {exception.Message}\n\nThe details of this error have been logged.";
            }
        }

        private MailMessage CreateAlertEmail()
        {
            var emailConfig = (NameValueCollection)ConfigurationManager.GetSection("emailConfig");
            var mail = new MailMessage();
            mail.From = new MailAddress(emailConfig["alertEmailAddress"]);
            mail.Subject = "BridgeCare - Committed Project Upload ";
            return mail;
        }

        private void SendAlertEmail(MailMessage message, UserInformationModel userInformation)
        {
            var emailConfig = (NameValueCollection)ConfigurationManager.GetSection("emailConfig");
            if (string.IsNullOrEmpty(userInformation.Email) || emailConfig["sendAlertEmails"] != "true")
            {
                return;
            }
            message.To.Add(userInformation.Email);
            System.Diagnostics.Debug.WriteLine(userInformation.Email);
            var SmtpServer = new SmtpClient(emailConfig["smtpServerAddress"]);
            SmtpServer.Port = int.Parse(emailConfig["smtpServerPort"]);
            SmtpServer.Credentials = new NetworkCredential(emailConfig["alertEmailAddress"], emailConfig["alertEmailPassword"]);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(message);
            SmtpServer.Dispose();
        }

        /// <summary>
        /// Export committed projects for a simulation
        /// </summary>
        /// <param name="simulationId"></param>
        /// <param name="networkId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public byte[] ExportCommittedProjects(int simulationId, int networkId, BridgeCareContext db, UserInformationModel userInformation)
        {
            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("CommittedProjects.xlsx")))
            {
                // This method may stay here or if too long then move to helper class   Fill(worksheet, , db);
                var committedProjects = CommittedProjectsGetMethods[userInformation.Role](simulationId, db, userInformation);
                var worksheet = excelPackage.Workbook.Worksheets.Add("Committed Projects");
                if (committedProjects.Count != 0)
                {
                    AddHeaderCells(worksheet, committedProjects.FirstOrDefault().COMMIT_CONSEQUENCES.ToList());
                    AddDataCells(worksheet, committedProjects, networkId, db);
                }
                return excelPackage.GetAsByteArray();
            }
        }

        private void AddDataCells(ExcelWorksheet worksheet, List<CommittedEntity> committedProjects, int networkId, BridgeCareContext db)
        {
            var committedProjectsSectionIds = committedProjects.Select(cproj => cproj.SECTIONID).ToList();
            var sectionModels = _sectionsRepository.GetSections(networkId, db);
            // get all committed projects that have a matching section, if any, and add them to the excel file
            var row = 2;
            sectionModels?.Where(sec => committedProjectsSectionIds.Contains(sec.SectionId)).OrderBy(sec => sec.ReferenceKey).ToList().ForEach(model =>
            {
                committedProjects.Where(cproj => cproj.SECTIONID == model.SectionId).OrderByDescending(cproj => cproj.YEARS).ToList()
                    .ForEach(committedProject =>
                    {
                        var column = 1;
                        // BRKEY, BMSID
                        worksheet.Cells[row, column++].Value = Convert.ToInt32(model.ReferenceKey);
                        worksheet.Cells[row, column++].Value = model.ReferenceId;
                        // Committed_
                        worksheet.Cells[row, column++].Value = committedProject.TREATMENTNAME;
                        worksheet.Cells[row, column++].Value = committedProject.YEARS;
                        worksheet.Cells[row, column++].Value = committedProject.YEARANY;
                        worksheet.Cells[row, column++].Value = committedProject.YEARSAME;
                        worksheet.Cells[row, column++].Value = committedProject.BUDGET;
                        worksheet.Cells[row, column++].Value = committedProject.COST_;
                        worksheet.Cells[row, column++].Value = string.Empty; // AREA
                        // Consequences
                        committedProject.COMMIT_CONSEQUENCES.ToList().ForEach(commitConsequence =>
                        {
                            worksheet.Cells[row, column++].Value = commitConsequence.CHANGE_;
                        });
                        row++;
                    });
            });

            // get all the committed projects that didn't have a matching section, if any, and add them to the excel file noting that the section was not found
            var sectionIds = sectionModels != null ? sectionModels.Select(model => model.SectionId).ToList() : new List<int>();
            committedProjects.Where(cproj => !sectionIds.Contains(cproj.SECTIONID)).OrderByDescending(cproj => cproj.YEARS).ToList()
            .ForEach(committedProject =>
            {
                var column = 1;
                // note section not found here
                worksheet.Cells[row, column++].Value = "Section Not Found";
                worksheet.Cells[row, column++].Value = "Section Not Found";
                // Committed_
                worksheet.Cells[row, column++].Value = committedProject.TREATMENTNAME;
                worksheet.Cells[row, column++].Value = committedProject.YEARS;
                worksheet.Cells[row, column++].Value = committedProject.YEARANY;
                worksheet.Cells[row, column++].Value = committedProject.YEARSAME;
                worksheet.Cells[row, column++].Value = committedProject.BUDGET;
                worksheet.Cells[row, column++].Value = committedProject.COST_;
                worksheet.Cells[row, column++].Value = string.Empty; // AREA
                // Consequences
                committedProject.COMMIT_CONSEQUENCES.ToList().ForEach(commitConsequence =>
                {
                    worksheet.Cells[row, column++].Value = commitConsequence.CHANGE_;
                });
                row++;
            });
        }

        private void AddHeaderCells(ExcelWorksheet worksheet, List<CommitConsequencesEntity> commitConsequences)
        {
            var fixColumnHeaders = new List<string>() { "BRKEY", "BMSID", "TREATMENT", "YEAR", "YEARANY", "YEARSAME", "BUDGET", "COST", "AREA" };
            int headerRow = 1;
            for (int column = 0; column < fixColumnHeaders.Count; column++)
            {
                worksheet.Cells[headerRow, column + 1].Value = fixColumnHeaders[column];
            }
            var currentColumn = fixColumnHeaders.Count;
            foreach (var commitConsequence in commitConsequences)
            {
                worksheet.Cells[headerRow, ++currentColumn].Value = commitConsequence.ATTRIBUTE_;
            }
        }

        private void GetCommittedProjectModels(ExcelPackage package, int simulationId, int networkId, bool applyNoTreatment,
        List<CommittedProjectModel> committedProjectModels, BridgeCareContext db)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            var headers = worksheet.Cells.GroupBy(cell => cell.Start.Row).First();
            var start = worksheet.Dimension.Start;
            var end = worksheet.Dimension.End;
            var simulation = db.Simulations.SingleOrDefault(s => s.SIMULATIONID == simulationId);

            var committedProjectYearsByBrKey = new Dictionary<int, List<int>>();

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                var brKey = Convert.ToInt32(GetCellValue(worksheet, row, 1));
                var year = Convert.ToInt32(GetCellValue(worksheet, row, start.Column + 3));
                if (committedProjectYearsByBrKey.ContainsKey(brKey))
                {
                    committedProjectYearsByBrKey[brKey].Add(year);
                } else
                {
                    committedProjectYearsByBrKey[brKey] = new List<int>() { year };
                }
            }

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                var column = start.Column + 2;
                var brKey = Convert.ToInt32(GetCellValue(worksheet, row, 1));
                var sectionId = _sectionsRepository.GetSectionId(networkId, brKey, db);

                // BMSID till COST -> entry in COMMITTED_
                var committedProjectModel = new CommittedProjectModel
                {
                    SectionId = sectionId,
                    SimulationId = simulationId,
                    TreatmentName = GetCellValue(worksheet, row, column),
                    Years = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                    YearAny = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                    YearSame = Convert.ToInt32(GetCellValue(worksheet, row, ++column)),
                    Budget = GetCellValue(worksheet, row, ++column),
                    Cost = double.Parse(GetCellValue(worksheet, row, ++column))
                };

                var commitConsequences = new List<CommitConsequenceModel>();
                // Ignore AREA column, from current column till end.Column -> attributes i.e. entry in COMMIT_CONSEQUENCES
                for (var col = column + 2; col <= end.Column; col++)
                {
                    commitConsequences.Add(new CommitConsequenceModel
                    {
                        Attribute_ = GetHeader(headers, col),
                        Change_ = GetCellValue(worksheet, row, col)
                    });
                }
                committedProjectModel.CommitConsequences = commitConsequences;
                committedProjectModels.Add(committedProjectModel);

                if (applyNoTreatment && simulation != null)
                {
                    var noTreatmentConsequences = commitConsequences
                        .Select(consequence => new CommitConsequenceModel() { Attribute_ = consequence.Attribute_, Change_ = "+0" })
                        .ToList();
                    if (simulation.COMMITTED_START < committedProjectModel.Years)
                    {
                        var year = committedProjectModel.Years - 1;
                        while (year >= simulation.COMMITTED_START && !committedProjectYearsByBrKey[brKey].Contains(year))
                        {
                            committedProjectModels.Add(new CommittedProjectModel()
                            {
                                SectionId = committedProjectModel.SectionId,
                                SimulationId = committedProjectModel.SimulationId,
                                TreatmentName = "No Treatment",
                                Years = year,
                                YearAny = committedProjectModel.YearAny,
                                YearSame = committedProjectModel.YearSame,
                                Budget = committedProjectModel.Budget,
                                Cost = 0,
                                CommitConsequences = noTreatmentConsequences
                            });
                            year--;
                        }
                    }
                }
            }
        }

        private string GetHeader(IGrouping<int, ExcelRangeBase> headers, int col)
        {
            return (string)headers.Skip(col - 1).First().Value;
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col].Value.ToString().Trim();
        }
    }
}
