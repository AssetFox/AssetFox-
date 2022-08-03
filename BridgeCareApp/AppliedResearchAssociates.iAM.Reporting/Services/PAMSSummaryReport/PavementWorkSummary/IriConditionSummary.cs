using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class IriConditionSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;
        private WorkSummaryModel _workSummaryModel;

        public IriConditionSummary(WorkSummaryModel workSummaryModel)
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        private void AddIriConditionSection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears,
            string title
            )
        {
            _pavementWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "", title);

            int startRow, startColumn, row, column;
            _pavementWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            var cheatLabels = new List<string> { "Excellent", "Good", "Fair", "Poor" };
            _pavementWorkSummaryCommon.SetPavementTreatmentExcelString(worksheet, cheatLabels, ref row, ref column);

            _pavementWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        public void FillIriConditionSummarySection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears
            )
        {
            AddIriConditionSection(worksheet, currentCell, simulationYears, "IRI Condition - Pavement Segment Miles BPN 1");
            AddIriConditionSection(worksheet, currentCell, simulationYears, "IRI Condition - Pavement Segment Miles BPN 2");
            AddIriConditionSection(worksheet, currentCell, simulationYears, "IRI Condition - Pavement Segment Miles BPN 3");
            AddIriConditionSection(worksheet, currentCell, simulationYears, "IRI Condition - Pavement Segment Miles BPN 4");
            AddIriConditionSection(worksheet, currentCell, simulationYears, "IRI Condition - Pavement Segment Miles Statewide");
        }
    }
}
