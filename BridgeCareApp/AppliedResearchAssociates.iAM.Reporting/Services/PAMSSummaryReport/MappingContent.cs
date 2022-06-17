using System;
using System.Collections.Generic;
using System.ComponentModel;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport
{
    public static class MappingContent
    {
        public static (string previousPick, string currentPick) GetCashFlowProjectPick(TreatmentCause treatmentCause, AssetDetail prevYearSection)
        {
            if (prevYearSection.TreatmentCause == treatmentCause)
            {
                return ("PAMS Pick CF", "PAMS Pick CFE"); // middle and last year
            }
            else
            {
                return ("PAMS Pick CFB", "PAMS Pick CFE"); // first and last years
            }
        }

        public static string GetNonCashFlowProjectPick(TreatmentCause treatmentCause)
        {
            switch (treatmentCause)
            {
            case TreatmentCause.NoSelection:
            case TreatmentCause.ScheduledTreatment:
            case TreatmentCause.SelectedTreatment:
                return "PAMS Pick";

            case TreatmentCause.CommittedProject:
                return "MPMS Pick";

            default:
                return treatmentCause.ToString();
            }
        }
    }
}
