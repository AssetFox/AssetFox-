﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class InvestmentStrategyModel
    {
        public InvestmentStrategyModel()
        {
            YearlyBudgets = new List<InvestmentStrategyYearlyBudgetModel>();
        }

        [Required]
        public int SimulationId { get; set; }
        public int NetworkId { get; set; }
        [Required]
        public string Name { get; set; }

        ///<remarks>Ignore, as it is agreed that years from the YearlyBudget
        ///will be the master record for FirstYear and NumberYears when
        ///updated by the web API</remarks>
        [IgnoreDataMember]
        public int FirstYear { get; set; }

        [IgnoreDataMember]
        public int NumberYears { get; set; }

        [Required]
        public double? InflationRate { get; set; }
        [Required]
        public double? DiscountRate { get; set; }
        public string Description { get; set; }
        public List<string> BudgetNamesByOrder { get; set; }

        ///<remarks>this is a variable which is uses to receive the
        ///comma delimited list of budget types, it is ignored so to
        ///not be transmitted on the web side as part of the API.</remarks>
        [IgnoreDataMember]
        public string BudgetOrder;

        [IgnoreDataMember]
        public List<string> DeletedBudgetNames { get; set; }

        public List<InvestmentStrategyYearlyBudgetModel> YearlyBudgets { get; set; }

        [IgnoreDataMember]
        public List<InvestmentStrategyYearlyBudgetModel> DeletedYearlyBudgets { get; set; }

        /// <summary>
        /// The one and only means to convert from BudgetOrder to BudgetNamesByOrder
        /// </summary>
        public void SetBudgets()
        {
            BudgetNamesByOrder = BudgetOrder.Split(',').ToList<string>();
        }

        /// <summary>
        /// The one and only means to convert from BudgetNamesByOrder to BudgetOrder
        /// </summary>
        public string GetBudgetOrder()
        {
            return string.Join(",", BudgetNamesByOrder);
        }
    }
}