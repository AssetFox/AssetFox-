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
using Org.BouncyCastle.Asn1.Cms;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AdminDataRepository : IAdminDataRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly NetworkRepository _networkRepo;

        public AdminDataRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));


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
            var existingNetwork = _unitOfWork.Context.Network.Where(_ => _.Id == adminNetworkGuid).SingleOrDefault();

            if (existingPrimaryNetwork == null)
            {
                return null;
            }
            else
            {
                var name = existingNetwork.Name;
                return name;
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
    }
}
