using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting.Models.PAMSSummaryReport;
using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.PavementWorkSummary
{
    public class OpiConditionSummary
    {
        private PavementWorkSummaryCommon _pavementWorkSummaryCommon;
        private WorkSummaryModel _workSummaryModel;

        public OpiConditionSummary(WorkSummaryModel workSummaryModel)
        {
            _pavementWorkSummaryCommon = new PavementWorkSummaryCommon();
            _workSummaryModel = workSummaryModel;
        }

        private void AddOpiConditionSection(
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

        public void FillOpiConditionSummarySection(
            ExcelWorksheet worksheet,
            CurrentCell currentCell,
            List<int> simulationYears
            )
        {
            AddOpiConditionSection(worksheet, currentCell, simulationYears, "OPI Condition - Pavement Segment Miles BPN 1");
            AddOpiConditionSection(worksheet, currentCell, simulationYears, "OPI Condition - Pavement Segment Miles BPN 2");
            AddOpiConditionSection(worksheet, currentCell, simulationYears, "OPI Condition - Pavement Segment Miles BPN 3");
            AddOpiConditionSection(worksheet, currentCell, simulationYears, "OPI Condition - Pavement Segment Miles BPN 4");
            AddOpiConditionSection(worksheet, currentCell, simulationYears, "OPI Condition - Pavement Segment Miles Statewide");
        }
    }
}
