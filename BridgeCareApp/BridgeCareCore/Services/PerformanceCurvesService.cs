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
            var performanceCurvesWorksheet = excelPackage.Workbook.Worksheets[0];
            var worksheetStart = performanceCurvesWorksheet.Dimension.Start;
            var worksheetEnd = performanceCurvesWorksheet.Dimension.End;

            var worksheetColumnNames = performanceCurvesWorksheet.Cells[1, 1, 1, worksheetEnd.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            var startColumn = worksheetStart.Column;
            var existingPerformanceCurves = _unitOfWork.PerformanceCurveRepo.GetPerformanceCurvesForLibrary(performanceCurveLibraryId);
            var performanceCurvesToImport = new List<PerformanceCurveDTO>();
            var missingAttributes = new List<string>();
            var invalidCriteria = new List<string>();
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
                if(!string.IsNullOrEmpty(criterionExpression))
                {
                    var validationResult = _expressionValidationService.ValidateCriterionWithoutResults(criterionExpression, currentUserCriteriaFilter);
                    if (!validationResult.IsValid)
                    {
                        invalidCriteria.Add(criterionExpression);
                        criterionExpression = null;
                    }
                }
                if(string.IsNullOrEmpty(attribute))
                {
                    missingAttributes.Add(attribute);                
                }

                performanceCurvesToImport.Add(new PerformanceCurveDTO
                {
                    Id = Guid.NewGuid(),
                    Name = performanceEquationName,
                    Attribute = attribute,
                    Equation = string.IsNullOrEmpty(equationExpression) ? null : new EquationDTO { Id = Guid.NewGuid(), Expression = equationExpression },
                    CriterionLibrary = string.IsNullOrEmpty(criterionExpression)
                                        ? null
                                        : new CriterionLibraryDTO { Id = Guid.NewGuid(), MergedCriteriaExpression = criterionExpression, IsSingleUse = true, Name = performanceEquationName + " " + attribute + " " + "Criterion" }
                });
            }
            // create dto here basically get perflobrarybyId
            var performanceCurveRepo = _unitOfWork.PerformanceCurveRepo;
            var performanceCurveLibraryDto = performanceCurveRepo.GetPerformanceCurveLibrary(performanceCurveLibraryId);
            performanceCurveLibraryDto.PerformanceCurves.AddRange(performanceCurvesToImport);
            performanceCurveRepo.UpsertPerformanceCurveLibrary(performanceCurveLibraryDto);
            performanceCurveRepo.UpsertOrDeletePerformanceCurves(performanceCurveLibraryDto.PerformanceCurves, performanceCurveLibraryId);
            var warningSb = new StringBuilder();
            if (missingAttributes.Any())            {
                
                warningSb.Append($"The following performace curves had no attributes: {string.Join(", ", missingAttributes)}. ");
            }
            if (invalidCriteria.Any())
            {
                warningSb.Append($"The following performace curves had invalid criteria: {string.Join(", ", invalidCriteria)}");
            }

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
            return new ScenarioPerformanceCurvesImportResultDTO();
        }
    }
}
