using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AdminDataRepository : IAdminDataRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly NetworkRepository _networkRepo;

        public AdminDataRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
            


        //Reads in KeyFields record as a string but places values in a list to return.
        public IList<string> GetKeyFields()
        {
            var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "KeyFields").FirstOrDefault();
            if (existingKeyFields == null)
            {
                return null;
            }
            else
            {
                var keyFields = existingKeyFields.Value;
                IList<string> KeyFieldsList = keyFields.Split(',').ToList();
                return KeyFieldsList;
            }
        }

        //String is to be passed in as parameter. Sets the KeyFields in the AdminSettings table. 
        public void SetKeyFields(string keyFields)
        {
            var boolAttributeExistence = false;
            var boolDuplicateExistence = false;
            IList<string> KeyFieldsList = keyFields.Split(',').ToList();
            var duplicateCount = KeyFieldsList.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z.Key).ToList();

            //This if statement checks if there are duplicates
            if (duplicateCount.Count > 0)
            {
                boolDuplicateExistence = true;
                throw new RowNotInTableException("A duplicate attribute is selected.");

            }
            //This checks that each attribute exists in the attribute table
            foreach (string KeyField in KeyFieldsList)
            {
                if (!_unitOfWork.Context.Attribute.Any(_ => _.Name == KeyField))
                {
                    boolAttributeExistence = false;
                    throw new RowNotInTableException("The specified attribute was not found.");
                }
                else
                    boolAttributeExistence = true;
            }
            //If each attribute is unique and exists in the attribute table
            if (boolAttributeExistence && !boolDuplicateExistence)
            {
                var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "KeyFields").SingleOrDefault();
                var KeyFieldsString = string.Join(",", keyFields);
                //If the entry doesn't exist in the AdminSettings table
                if (existingKeyFields == null)
                {
                    _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                    {
                        Key = "KeyFields",
                        Value = KeyFieldsString
                    });
                }
                //Updates existing KeyFields entry in the AdminSettings table
                else
                {
                    existingKeyFields.Value = KeyFieldsString;
                    _unitOfWork.Context.AdminSettings.Update(existingKeyFields);
                }
                _unitOfWork.Context.SaveChanges();
            }

        }

        public string GetPrimaryNetwork()
        {
            var existingPrimaryNetwork = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == "PrimaryNetwork");
            var adminNetworkGuid = new Guid(existingPrimaryNetwork.Value);
            var existingNetwork = _unitOfWork.Context.Network.SingleOrDefault(_ => _.Id == adminNetworkGuid);

            if (existingPrimaryNetwork == null)
            {
                return null;
            }
            else
            {                
                return existingNetwork.Name;
            }
        }

        public void SetPrimaryNetwork(string name)
        {
            var existingNetworkAdminSetting = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "PrimaryNetwork").FirstOrDefault();
            var existingNetwork = _unitOfWork.Context.Network.FirstOrDefault(_ => _.Name == name);
            if (existingNetwork == null)
            {
                throw new RowNotInTableException("The specified network was not found.");
            }
            if (existingNetworkAdminSetting == null)
            {

                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "PrimaryNetwork",
                    Value = Guid.NewGuid().ToString()
                });
            }
            else
            {

                existingNetworkAdminSetting.Value = existingNetwork.Id.ToString();
                _unitOfWork.Context.AdminSettings.Update(existingNetworkAdminSetting);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public IList<string> GetSimulationReportNames()
        {
            var existingSimulationReports = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == "SimulationReportNames");
            if (existingSimulationReports == null)
            {
                return null;
            }
            else
            {
                var name = existingSimulationReports.Value;
                IList<string> GetSimulationReportNames = name.Split(',').ToList();

                return GetSimulationReportNames;
            }

        }

        public IList<string> GetInventoryReports()
        {
            if (!_unitOfWork.Context.AdminSettings.Any())
            {
                throw new RowNotInTableException("No AdminSettings available");
            }

            var existingInventoryReports = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == "InventoryReportNames");

            if (existingInventoryReports == null)
            {
                throw new KeyNotFoundException("InventoryReportNames setting not found in AdminSettings");
            }

            var name = existingInventoryReports.Value;
            IList<string> getSimulationReportNames = name.Split(',').ToList();

            return getSimulationReportNames;
        }

        public string GetAttributeName(Guid attributeId)
        {
            var attributeName = _unitOfWork.Context.Attribute.AsNoTracking().FirstOrDefault(a => a.Id == attributeId)?.Name;
            return attributeName ?? throw new InvalidOperationException("Cannot find attribute for the given id.");
        }


        public void SetInventoryReports(string InventoryReports)
        {
            var existingInventoryReports = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "InventoryReportNames").SingleOrDefault();
            if (existingInventoryReports == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "InventoryReportNames",
                    Value = InventoryReports
                }) ;
            }
            else
            {
                existingInventoryReports.Value = InventoryReports;
                _unitOfWork.Context.AdminSettings.Update(existingInventoryReports);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public void SetSimulationReports(string SimulationReports)
        {
            var existingSimulationReports = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "SimulationReportsNames").SingleOrDefault();
            if (existingSimulationReports == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "SimulationReportsNames",
                    Value = SimulationReports
                });
            }
            else
            {
                existingSimulationReports.Value = SimulationReports;
                _unitOfWork.Context.AdminSettings.Update(existingSimulationReports);
            }
            _unitOfWork.Context.SaveChanges();
        }
    }
}
