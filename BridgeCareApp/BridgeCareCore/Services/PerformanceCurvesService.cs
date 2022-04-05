using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Interfaces;
using OfficeOpenXml;

namespace BridgeCareCore.Services
{
    public class PerformanceCurvesService : IPerformanceCurvesService
    {
        private static UnitOfDataPersistenceWork _unitOfWork;        
        protected readonly IHubService _hubService;
        private readonly IExpressionValidationService _expressionValidationService;

        public PerformanceCurvesService(UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IExpressionValidationService expressionValidationService)
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
            var warningSb = new StringBuilder();

            CreatePerformanceCurvesDtos(excelPackage, currentUserCriteriaFilter, performanceCurvesWithMissingAttributes, performanceCurvesWithInvalidCriteria, performanceCurvesToImport);

            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var performanceCurveLibraryDto = performanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
            var existingPerformanceCurves = performanceCurveLibraryDto.PerformanceCurves;
            // Update ids if curves exists
            foreach (var performanceCurveToImport in performanceCurvesToImport)
            {
                var existingCurve = existingPerformanceCurves.FirstOrDefault(_ => _.Name == performanceCurveToImport.Name);
                performanceCurveToImport.Id = existingCurve != null ? existingCurve.Id : performanceCurveToImport.Id;
            }

            var curvesNamesToImport = performanceCurvesToImport.Select(_ => _.Name);
            existingPerformanceCurves.RemoveAll(_ => curvesNamesToImport.Contains(_.Name));
            // Combine curves to be imported
            performanceCurvesToImport.AddRange(existingPerformanceCurves);
            performanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);
            try
            {
                performanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurvesToImport, performanceCurveLibraryId);
            }
            catch (Exception ex)
            {
                warningSb.Append($"Error occured in import of performance curve(s): {ex.Message}");
            }

            UpdateWarningForMissingAttributes(performanceCurvesWithMissingAttributes, warningSb);
            UpdateWarningForInvalidCriteria(performanceCurvesWithInvalidCriteria, warningSb);

            return new PerformanceCurvesImportResultDTO
            {
                PerformanceCurves = performanceCurveRepo.GetPerformanceCurvesForLibrary(performanceCurveLibraryId),
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
            var warningSb = new StringBuilder();

            CreatePerformanceCurvesDtos(excelPackage, currentUserCriteriaFilter, performanceCurvesWithMissingAttributes, performanceCurvesWithInvalidCriteria, performanceCurvesToImport);
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var existingPerformanceCurves = performanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            // Update ids if curves exists
            foreach (var performanceCurveToImport in performanceCurvesToImport)
            {
                var existingCurve = existingPerformanceCurves.FirstOrDefault(_ => _.Name == performanceCurveToImport.Name);
                performanceCurveToImport.Id = existingCurve != null ? existingCurve.Id : performanceCurveToImport.Id;
            }

            var curvesNamesToImport = performanceCurvesToImport.Select(_ => _.Name);
            existingPerformanceCurves.RemoveAll(_ => curvesNamesToImport.Contains(_.Name));
            // Combine curves to be imported
            performanceCurvesToImport.AddRange(existingPerformanceCurves);            
            try
            {
                performanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(performanceCurvesToImport, simulationId);
            }
            catch (Exception ex)
            {
                warningSb.Append($"Error occured in import of performance curve(s): {ex.Message}");
            }

            UpdateWarningForMissingAttributes(performanceCurvesWithMissingAttributes, warningSb);
            UpdateWarningForInvalidCriteria(performanceCurvesWithInvalidCriteria, warningSb);

            return new ScenarioPerformanceCurvesImportResultDTO
            {
                PerformanceCurves = performanceCurveRepo.GetScenarioPerformanceCurves(simulationId),
                WarningMessage = !string.IsNullOrEmpty(warningSb.ToString())
                    ? warningSb.ToString()
                    : null
            };
        }

        private static void UpdateWarningForInvalidCriteria(List<string> performanceCurvesWithInvalidCriteria, StringBuilder warningSb)
        {
            if (performanceCurvesWithInvalidCriteria.Any())
            {
                warningSb.Append($"The following performace curves are imported without criteria due to invalid values: {string.Join(", ", performanceCurvesWithInvalidCriteria)}");
            }
        }

        private static void UpdateWarningForMissingAttributes(List<string> performanceCurvesWithMissingAttributes, StringBuilder warningSb)
        {
            if (performanceCurvesWithMissingAttributes.Any())
            {

                warningSb.Append($"The following performace curves could not be imported due to missing attributes: {string.Join(", ", performanceCurvesWithMissingAttributes)}. ");
            }
        }

        private void CreatePerformanceCurvesDtos(ExcelPackage excelPackage, UserCriteriaDTO currentUserCriteriaFilter, List<string> performanceCurvesWithMissingAttributes, List<string> performanceCurvesWithInvalidCriteria, List<PerformanceCurveDTO> performanceCurvesToImport)
        {
            var performanceCurvesWorksheet = excelPackage.Workbook.Worksheets[0];
            var worksheetStart = performanceCurvesWorksheet.Dimension.Start;
            var worksheetEnd = performanceCurvesWorksheet.Dimension.End;
            var startColumn = worksheetStart.Column;
            for (var dataRow = worksheetStart.Row + 1; dataRow <= worksheetEnd.Row; dataRow++)
            {
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
    }
}
