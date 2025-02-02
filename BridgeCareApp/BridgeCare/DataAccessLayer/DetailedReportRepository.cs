﻿using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class DetailedReportRepository : IDetailedReportRepository
    {
        [Column(TypeName = "VARCHAR")]
        public string Facility { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Section { get; set; }

        [Column(TypeName = "VARCHAR")]
        public string Treatment { get; set; }

        public int NumberTreatment { get; set; }
        public bool IsCommitted { get; set; }
        public int Years { get; set; }

        private readonly BridgeCareContext db;

        public DetailedReportRepository(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Required by entity framework
        public DetailedReportRepository() { }

        public List<YearlyDataModel> GetYearsData(SimulationModel data)
        {
            var yearsForBudget = new List<YearlyDataModel>();
            try
            {
                yearsForBudget = db.YearlyInvestments.AsNoTracking().Where(_ => _.SIMULATIONID == data.simulationId)
                                                              .OrderBy(year => year.YEAR_)
                                                              .Select(p => new YearlyDataModel
                                                              {
                                                                  Year = p.YEAR_,
                                                                  Amount = p.AMOUNT,
                                                                  BudgetName = p.BUDGETNAME
                                                              }).ToList();
            }
            catch (SqlException ex)
            {
                var log = log4net.LogManager.GetLogger(typeof(DetailedReportRepository));
                log.Error(ex.Message);
                HandleException.SqlError(ex, "Years");
            }
            return yearsForBudget;
        }

        public IQueryable<DetailedReportRepository> GetRawQuery(SimulationModel data, BridgeCareContext dbContext)
        {
            var query = "SELECT Facility, Section, Treatment, NumberTreatment, IsCommitted, Years " +
                        $"FROM Report_{data.networkId}_{data.simulationId} Rpt WITH (NOLOCK) " +
                        $"INNER JOIN Section_{data.networkId} Sec WITH (NOLOCK) " +
                        "ON Rpt.SectionID = Sec.SectionID " +
                        "Order By Facility, Section, Years";

            return dbContext.Database.SqlQuery<DetailedReportRepository>(query).AsQueryable();
        }
    }
}
