﻿using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class DeficientReportRepository : IDeficientReportRepository
    {
        private readonly BridgeCareContext db;
        private readonly TargetsMetRepository deficients;
        private readonly CellAddress address;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DeficientReportRepository));

        public DeficientReportRepository(BridgeCareContext context, TargetsMetRepository deficient, CellAddress cell)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            deficients = deficient ?? throw new ArgumentNullException(nameof(deficient));
            address = cell ?? throw new ArgumentNullException(nameof(cell));
        }

        public DeficientResult GetData(SimulationModel data, int[] totalYears)
        {
            // Deficient and DeficientResults are models. Deficient gets data
            // from the database. DeficientResult gets the processed data
            IQueryable<DeficientReportModel> deficients = null;
            DeficientResult result = null;

            var select =
                "SELECT TargetID, Years, coalesce(TARGETMET, 0) as TargetMet, IsDeficient " +
                    " FROM Target_" + data.networkId
                    + "_" + data.simulationId;

            try
            {
                var rawDeficientList = db.Database.SqlQuery<DeficientReportModel>(select).AsQueryable();

                deficients = rawDeficientList.Where(_ => _.IsDeficient == true);

                var targetAndYear = this.deficients.GetData(deficients);
                result = GetDeficientInformation(data, targetAndYear, totalYears);
            }
            catch (SqlException ex)
            {
                log.Error(ex.Message);
                HandleException.SqlError(ex, "Target");
            }
            catch (OutOfMemoryException ex)
            {
                log.Error(ex.Message);
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                HandleException.GeneralError(ex);
            }
            return result;
        }

        private DeficientResult GetDeficientInformation(SimulationModel data, Hashtable yearsIDValues, int[] totalYears)
        {
            var deficientTableData = db.Deficients.AsNoTracking().Where(_ => _.SIMULATIONID == data.simulationId);
            var totalYearCount = totalYears.Count();
            DataTable DeficientTable = new DataTable();
            DeficientTable.Columns.Add("Attribute");
            DeficientTable.Columns.Add("Group");
            for (int i = 0; i < totalYearCount; i++)
            {
                DeficientTable.Columns.Add(totalYears[i].ToString());
            }
            FillData(deficientTableData, yearsIDValues, DeficientTable, totalYears);
            var forDeficient = new DeficientResult
            {
                Deficients = DeficientTable,
                Address = address
            };
            return forDeficient;
        }

        private void FillData(IQueryable<DeficientsEntity> deficientTableData, Hashtable yearsIDValues, DataTable DeficientTable, int[] totalYears)
        {
            var increment = 2;
            foreach (var item in deficientTableData)
            {
                Hashtable yearValues = new Hashtable();
                yearValues = (Hashtable)yearsIDValues[item.ID_];

                DataRow newDataRow = DeficientTable.NewRow();
                newDataRow["Attribute"] = item.ATTRIBUTE_;
                newDataRow["Group"] = item.DEFICIENTNAME;

                if (yearValues.Count <= 0)
                {
                    continue;
                }
                foreach (int key in yearValues.Keys)
                {
                    int column = key - totalYears[0] + 2;
                    var value = (double)yearValues[key] * 100;
                    newDataRow[column] = value + "%/" + item.PERCENTDEFICIENT + "% (" + item.DEFICIENT + ")";

                    if (value > item.PERCENTDEFICIENT)
                    {
                        address.Cells.Add((increment, column));
                    }
                }
                DeficientTable.Rows.Add(newDataRow);
                increment++;
            }
        }
    }
}
