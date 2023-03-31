using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class PerformanceCurvesService : IPerformanceCurvesService
    {
        private static IUnitOfWork _unitOfWork;        
        protected readonly IHubService _hubService;
        private readonly IExpressionValidationService _expressionValidationService;
        public const string ImportedWithoutCriterioDueToInvalidValues = "The following performace curves are imported without criteria due to invalid values:";

        public PerformanceCurvesService(IUnitOfWork unitOfWork, IHubService hubService, IExpressionValidationService expressionValidationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
            _expressionValidationService = expressionValidationService ?? throw new ArgumentNullException(nameof(expressionValidationService));
        }

        public PerformanceCurvesImportResultDTO ImportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
        {
            var performanceCurvesToImport = new List<PerformanceCurveDTO>();
            var performanceCurvesWithMissingAttributes = new List<string>();
            var performanceCurvesWithInvalidCriteria = new List<string>();
            var performanceCurvesWithInvalidEquation = new List<string>();
            var warningSb = new StringBuilder();
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var performanceCurveLibraryDto = performanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
            try
            {
                CreatePerformanceCurvesDtos(excelPackage, currentUserCriteriaFilter, performanceCurvesWithMissingAttributes, performanceCurvesWithInvalidCriteria, performanceCurvesWithInvalidEquation, performanceCurvesToImport);

                #region Commented update of existing and keeping existing curves
                //var existingPerformanceCurves = performanceCurveLibraryDto.PerformanceCurves;
                //// Update ids if curves exists
                //foreach (var performanceCurveToImport in performanceCurvesToImport)
                //{
                //    var existingCurve = existingPerformanceCurves.FirstOrDefault(_ => _.Name == performanceCurveToImport.Name);
                //    performanceCurveToImport.Id = existingCurve != null ? existingCurve.Id : performanceCurveToImport.Id;
                //}
                //var curvesNamesToImport = performanceCurvesToImport.Select(_ => _.Name);
                //existingPerformanceCurves.RemoveAll(_ => curvesNamesToImport.Contains(_.Name));
                //// Combine curves to be imported
                //performanceCurvesToImport.AddRange(existingPerformanceCurves);
                #endregion
                performanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);
                performanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurvesToImport, performanceCurveLibraryId);
            }
            catch (Exception ex)
            {
                warningSb.Append($"Error occured in import of performance curve(s): {ex.Message}");
            }

            UpdateWarningForMissingAttributes(performanceCurvesWithMissingAttributes, warningSb);
            UpdateWarningForInvalidCriteria(performanceCurvesWithInvalidCriteria, warningSb);
            UpdateWarningForInvalidEquation(performanceCurvesWithInvalidEquation, warningSb);
            
            return new PerformanceCurvesImportResultDTO
            {
                PerformanceCurveLibraryDTO = new PerformanceCurveLibraryDTO { Id = performanceCurveLibraryId, Name = performanceCurveLibraryDto.Name, PerformanceCurves = performanceCurveRepo.GetPerformanceCurvesForLibrary(performanceCurveLibraryId) },
                WarningMessage = !string.IsNullOrEmpty(warningSb.ToString())
                    ? warningSb.ToString()
                    : null
            };
        }

        public ScenarioPerformanceCurvesImportResultDTO ImportScenarioPerformanceCurvesFile(Guid simulationId, ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter)
        {
            var performanceCurvesToImport = new List<PerformanceCurveDTO>();
            var performanceCurvesWithMissingAttributes = new List<string>();
            var performanceCurvesWithInvalidCriteria = new List<string>();
            var performanceCurvesWithInvalidEquation = new List<string>();
            var warningSb = new StringBuilder();
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            try
            {
                CreatePerformanceCurvesDtos(excelPackage, currentUserCriteriaFilter, performanceCurvesWithMissingAttributes, performanceCurvesWithInvalidCriteria, performanceCurvesWithInvalidEquation, performanceCurvesToImport);

                #region Commented update of existing and keeping existing curves
                //var existingPerformanceCurves = performanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
                //// Update ids if curves exists
                //foreach (var performanceCurveToImport in performanceCurvesToImport)
                //{
                //    var existingCurve = existingPerformanceCurves.FirstOrDefault(_ => _.Name == performanceCurveToImport.Name);
                //    performanceCurveToImport.Id = existingCurve != null ? existingCurve.Id : performanceCurveToImport.Id;
                //}
                //var curvesNamesToImport = performanceCurvesToImport.Select(_ => _.Name);
                //existingPerformanceCurves.RemoveAll(_ => curvesNamesToImport.Contains(_.Name));
                //// Combine curves to be imported
                //performanceCurvesToImport.AddRange(existingPerformanceCurves);
                #endregion

                performanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(performanceCurvesToImport, simulationId);
            }
            catch (Exception ex)
            {
                warningSb.Append($"Error occured in import of performance curve(s): {ex.Message}");
            }

            UpdateWarningForMissingAttributes(performanceCurvesWithMissingAttributes, warningSb);
            UpdateWarningForInvalidCriteria(performanceCurvesWithInvalidCriteria, warningSb);
            UpdateWarningForInvalidEquation(performanceCurvesWithInvalidEquation, warningSb);
            var scenarioCurves = performanceCurveRepo.GetScenarioPerformanceCurves(simulationId);

            return new ScenarioPerformanceCurvesImportResultDTO
            {
                PerformanceCurves = scenarioCurves,
                WarningMessage = !string.IsNullOrEmpty(warningSb.ToString())
                    ? warningSb.ToString()
                    : null
            };
        }

        private static void UpdateWarningForInvalidEquation(List<string> performanceCurvesWithInvalidEquation, StringBuilder warningSb)
        {
            if (performanceCurvesWithInvalidEquation.Any())
            {
                warningSb.Append($"The following performace curves are imported without equation due to invalid values: {string.Join(", ", performanceCurvesWithInvalidEquation)}");
            }
        }

        private static void UpdateWarningForInvalidCriteria(List<string> performanceCurvesWithInvalidCriteria, StringBuilder warningSb)
        {
            if (performanceCurvesWithInvalidCriteria.Any())
            {
                warningSb.Append($"{ImportedWithoutCriterioDueToInvalidValues} {string.Join(", ", performanceCurvesWithInvalidCriteria)}");
            }
        }

        private static void UpdateWarningForMissingAttributes(List<string> performanceCurvesWithMissingAttributes, StringBuilder warningSb)
        {
            if (performanceCurvesWithMissingAttributes.Any())
            {

                warningSb.Append($"The following performace curves could not be imported due to missing attributes: {string.Join(", ", performanceCurvesWithMissingAttributes)}. ");
            }
        }

        private void CreatePerformanceCurvesDtos(ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter, List<string> performanceCurvesWithMissingAttributes, List<string> performanceCurvesWithInvalidCriteria, List<string> performanceCurvesWithInvalidEquation, List<PerformanceCurveDTO> performanceCurvesToImport)
        {
            var performanceCurvesWorksheet = excelPackage.Workbook.Worksheets[0];
            var worksheetStart = performanceCurvesWorksheet.Dimension.Start;
            var worksheetEnd = performanceCurvesWorksheet.Dimension.End;            
            for (var dataRow = worksheetStart.Row + 1; dataRow <= worksheetEnd.Row; dataRow++)
            {
                var startColumn = worksheetStart.Column;
                var performanceEquationName = performanceCurvesWorksheet.GetValue<string>(dataRow, startColumn++);
                var attribute = performanceCurvesWorksheet.GetValue<string>(dataRow, startColumn++);
                if (performanceEquationName == null && attribute == null)
                {
                    continue;
                }

                var equationExpression = performanceCurvesWorksheet.GetValue<string>(dataRow, startColumn++);
                var criterionExpression = performanceCurvesWorksheet.GetValue<string>(dataRow, startColumn++);
                if (!string.IsNullOrEmpty(criterionExpression))
                {
                    // Validate criterion
                    var validationResult = _expressionValidationService.ValidateCriterionWithoutResults(criterionExpression, currentUserCriteriaFilter);
                    if (!validationResult.IsValid)
                    {
                        performanceCurvesWithInvalidCriteria.Add(performanceEquationName + ": " + criterionExpression);
                        criterionExpression = null;
                    }
                }
                if (string.IsNullOrEmpty(attribute))
                {
                    performanceCurvesWithMissingAttributes.Add(performanceEquationName);
                    continue;
                }

                // Validate equation
                var isPiecewise = !string.IsNullOrEmpty(equationExpression) && equationExpression.Contains(")(");
                var validateEquationResult = _expressionValidationService.ValidateEquation(new EquationValidationParameters { Expression = equationExpression, IsPiecewise = isPiecewise });
                if (!validateEquationResult.IsValid)
                {
                    performanceCurvesWithInvalidEquation.Add(performanceEquationName + ": " + equationExpression);
                    equationExpression   = null;
                }

                performanceCurvesToImport.Add(new PerformanceCurveDTO
                {
                    Id = Guid.NewGuid(),
                    Name = performanceEquationName,
                    Attribute = attribute,
                    Equation = string.IsNullOrEmpty(equationExpression) ? null : new EquationDTO { Id = Guid.NewGuid(), Expression = equationExpression },
                    CriterionLibrary = string.IsNullOrEmpty(criterionExpression)
                                        ? null
                                        : new CriterionLibraryDTO { Id = Guid.NewGuid(), MergedCriteriaExpression = criterionExpression }
                });
            }
        }

        public FileInfoDTO ExportScenarioPerformanceCurvesFile(Guid simulationId)
        {
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var PerformanceCurves = performanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var simulationName = _unitOfWork.SimulationRepo.GetSimulationName(simulationId);
            var fileName = $"{simulationName.Trim().Replace(" ", "_")}_performance_curves.xlsx";
            
            return CreateExportFile(PerformanceCurves, fileName);
        }        

        public FileInfoDTO ExportLibraryPerformanceCurvesFile(Guid performanceCurveLibraryId)
        {
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var PerformanceCurves = performanceCurveRepo.GetPerformanceCurvesForLibrary(performanceCurveLibraryId);
            var libraryName = performanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId).Name;
            var fileName = $"{libraryName.Trim().Replace(" ", "_")}_performance_curves.xlsx";
            
            return CreateExportFile(PerformanceCurves, fileName);
        }

        private static FileInfoDTO CreateExportFile(List<PerformanceCurveDTO> PerformanceCurves, string fileName)
        {
            using var excelPackage = new ExcelPackage(new FileInfo(fileName));
            var worksheet = excelPackage.Workbook.Worksheets.Add("Performance Curves");

            // headers
            var startRow = worksheet.Cells.Start.Row;
            var startColumn = worksheet.Cells.Start.Column;
            var headerColumn = startColumn;
            worksheet.Cells[startRow, headerColumn++].Value = "Performance_Equation_Name";
            worksheet.Cells[startRow, headerColumn++].Value = "Attribute";
            worksheet.Cells[startRow, headerColumn++].Value = "Equation_Expression";
            worksheet.Cells[startRow, headerColumn++].Value = "Criterion_Expression";

            // data rows
            var dataRow = startRow + 1;
            foreach (var performanceCurve in PerformanceCurves)
            {
                var dataColumn = startColumn;
                worksheet.Cells[dataRow, dataColumn++].Value = performanceCurve.Name;
                worksheet.Cells[dataRow, dataColumn++].Value = performanceCurve.Attribute;
                worksheet.Cells[dataRow, dataColumn++].Value = performanceCurve.Equation?.Expression;
                worksheet.Cells[dataRow, dataColumn++].Value = performanceCurve.CriterionLibrary?.MergedCriteriaExpression;
                dataRow++;
            }
            worksheet.Cells.AutoFitColumns();

            return new FileInfoDTO
            {
                FileName = fileName,
                FileData = Convert.ToBase64String(excelPackage.GetAsByteArray()),
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }      
    }
}
