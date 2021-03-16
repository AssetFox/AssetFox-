using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQLLegacy.Entities
{
    [Table("YEARLYINVESTMENT")]
    public class YearlyInvestmentEntity
    {
        [Key]
        public int YEARID { get; set; }

        public int SIMULATIONID { get; set; }

        public int YEAR_ { get; set; }

        [Required]
        [StringLength(50)]
        public string BUDGETNAME { get; set; }

        public double? AMOUNT { get; set; }
    }
}
