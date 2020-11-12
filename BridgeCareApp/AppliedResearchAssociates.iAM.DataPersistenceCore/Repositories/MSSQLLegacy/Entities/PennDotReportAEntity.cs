using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities
{
    [Table("PennDot_Report_A")]
    public class PennDotReportAEntity
    {
        [Key]
        public int BRKEY { get; set; }

        public string BRIDGE_ID { get; set; }

        public string DISTRICT { get; set; }

        public string DECK_AREA { get; set; }

        public string NHS_IND { get; set; }

        public string BUS_PLAN_NETWORK { get; set; }

        public string FUNC_CLASS { get; set; }

        public string YEAR_BUILT { get; set; }

        public string ADTTOTAL { get; set; }

        public string Length { get; private set; }

        public string Structure_Type { get; private set; }

        //public string PlanningPartner { get; private set; }

        public string Post_Status { get; private set; }

        public int P3_Bridge { get; private set; }

        public int Parallel_Struct { get; private set; }

        public string Owner_Code { get; private set; }
    }
}
