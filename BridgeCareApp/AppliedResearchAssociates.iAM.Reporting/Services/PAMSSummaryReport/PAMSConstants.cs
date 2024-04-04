using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport
{
    public static class PAMSConstants
    {
        //------------------- Excel Tabs  and Titles -------------------------
        public const string Parameters_Tab = "Parameters"; //summary of parameters used in the report
        public const string Parameters_Tab_Title = "";

        public const string PAMSData_Tab = "PAMS Data"; //tabular data
        public const string PAMSData_Tab_Title = "";

        public const string UnfundedPavementProjects_Tab = "Unfunded Pavement Projects"; //tabular data
        public const string UnfundedPavementProjects_Tab_Title = "";

        public const string PavementWorkSummary_Tab = "Pavement Work Summary"; //categorized tabular data and estimates
        public const string PavementWorkSummary_Tab_Title = "";

        public const string PavementWorkSummaryByBudget_Tab = "Pavement Work Summary By Budget";
        public const string PavementWorkSummaryByBudget_Tab_Title = "District #";

        public const string CountySummary_Tab = "County Summary";
        public const string CountySummary_Tab_Title = "Overall Dollars Recommended on Treatments by County";

        public const string IRI_BPN1_Tab = "PennDOT IRI BPN 1";
        public const string IRI_BPN1_Tab_Title = "PennDOT IRI BPN 1";

        public const string IRI_BPN2_Tab = "PennDOT IRI BPN 2";
        public const string IRI_BPN2_Tab_Title = "PennDOT IRI BPN 2";

        public const string IRI_BPN3_Tab = "PennDOT IRI BPN 3";
        public const string IRI_BPN3_Tab_Title = "PennDOT IRI BPN 3";

        public const string IRI_BPN4_Tab = "PennDOT IRI BPN 4";
        public const string IRI_BPN4_Tab_Title = "PennDOT IRI BPN 4";

        public const string IRI_Statewide_Tab = "PennDOT IRI Statewide";
        public const string IRI_Statewide_Tab_Title = "PennDOT IRI Statewide";

        public const string OPI_BPN1_Tab = "PennDOT OPI BPN 1";
        public const string OPI_BPN1_Tab_Title = "PennDOT OPI BPN 1";

        public const string OPI_BPN2_Tab = "PennDOT OPI BPN 2";
        public const string OPI_BPN2_Tab_Title = "PennDOT OPI BPN 2";

        public const string OPI_BPN3_Tab = "PennDOT OPI BPN 3";
        public const string OPI_BPN3_Tab_Title = "PennDOT OPI BPN 3";

        public const string OPI_BPN4_Tab = "PennDOT OPI BPN 4";
        public const string OPI_BPN4_Tab_Title = "PennDOT OPI BPN 4";

        public const string OPI_Statewide_Tab = "PennDOT OPI Statewide";
        public const string OPI_Statewide_Tab_Title = "PennDOT OPI Statewide";

        public const string Graph_Tabs_YAxisTitle = "Miles";
        public const string Graph_Tabs_XAxisTitle = "Year";

        public const string Legend_Tab = "Legend";

        public const string AsphaltTotal = "PAMS Full Depth Asphalt Total";
        public const string CompositeTotal = "PAMS Composite Total";
        public const string ConcreteTotal = "PAMS Concrete Total";
        public const string CommittedTotal = "Committed Total";
        public const string OPICalculationsTab = "Condition Data";

        public const string TotalWorkBudget = "Total Work Budget";
        //Data Variables
        public const string Work = "Work";
        public const string Cost = "Cost";
        public const string Total = "Total";

        public const string NoTreatment = "no treatment";
        public const string NoTreatmentForWorkSummary = "No Treatment";


        public const string PercentBudgetSpentMPMS = "% of Budget - MPMS";
        public const string PercentBudgetSpentPAMS = "% of Budget - PAMS";
        public const string PercentBudgetSpentSAP = "% of Budget - SAP";
        public const string PercentBudgetSpentProjectBuilder = "% of Budget - Project Builder";
        public const string RemainingBudget = "Remaining Budget";

        public const string Poor = "Poor";
        public const string Fair = "Fair";
        public const string Good = "Good";        
        public const string Excellent = "Excellent";

        public const string Bundled = "Bundled";
        public const string BundledTreatments = "Bundled Treatments";
    }
}
