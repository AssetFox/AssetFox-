using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Org.BouncyCastle.Asn1.Cms;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AdminSettingsRepository : IAdminSettingsRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public AdminSettingsRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private const string inventoryReportKey = "InventoryReportNames";
        private const string simulationReportKey = "SimulationReportNames";
        private const string keyFieldKey = "KeyFields";
        private const string primaryNetworkKey = "PrimaryNetwork";
        private const string constraintTypeKey = "ConstraintType";

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
                        Key = keyFieldKey,
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
            var existingPrimaryNetwork = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == primaryNetworkKey);
            if (existingPrimaryNetwork == null)
            {
                return null;
            }
            var adminNetworkGuid = new Guid(existingPrimaryNetwork.Value);
            var existingNetwork = _unitOfWork.Context.Network.SingleOrDefault(_ => _.Id == adminNetworkGuid);

            if (existingNetwork == null)
            {
                return null;
            }
            else
            {
                var adminNetworkGuid = new Guid(existingPrimaryNetwork.Value);
                var existingNetwork = _unitOfWork.Context.Network.SingleOrDefault(_ => _.Id == adminNetworkGuid);
                return existingNetwork.Name;
            }
        }

        public void SetPrimaryNetwork(string name)
        {
            var existingNetworkAdminSetting = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == primaryNetworkKey).FirstOrDefault();
            var existingNetwork = _unitOfWork.Context.Network.FirstOrDefault(_ => _.Name == name);
            if (existingNetwork == null)
            {
                throw new RowNotInTableException("The specified network was not found.");
            }
            if (existingNetworkAdminSetting == null)
            {

                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = primaryNetworkKey,
                    Value = existingNetwork.Id.ToString()
                });
            }
            else
            {

                existingNetworkAdminSetting.Value = existingNetwork.Id.ToString();
                _unitOfWork.Context.AdminSettings.Update(existingNetworkAdminSetting);
            }
            _unitOfWork.Context.SaveChanges();
        }
        public IList<string> GetAvailableReports()
        {

            return null;

        }
        public IList<string> GetSimulationReportNames()
        {
            var existingSimulationReports = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == simulationReportKey);
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
            var existingInventoryReports = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == inventoryReportKey);

            if (existingInventoryReports == null)
            {
                return null;
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
            var existingInventoryReports = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == inventoryReportKey).SingleOrDefault();
            if (existingInventoryReports == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = inventoryReportKey,
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
            var existingSimulationReports = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == simulationReportKey).SingleOrDefault();
            if (existingSimulationReports == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = simulationReportKey,
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

        public string GetImplementationName()
        {
            var existingImplementationName = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationName").FirstOrDefault();
            if (existingImplementationName == null)
            {
                return null;
            }
            else
            {
                return existingImplementationName.Value;
            }
        }

        public void SetImplementationName(string name)
        {
            var existingImplementationName = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationName").FirstOrDefault();
            if (existingImplementationName == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "ImplementationName",
                    Value = name
                });
            }
            else
            {
                existingImplementationName.Value = name;
                _unitOfWork.Context.AdminSettings.Update(existingImplementationName);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public void SetAgencyLogo(Image agencyLogo)
        {
            //https://stackoverflow.com/questions/21325661/convert-an-image-selected-by-path-to-base64-string
            byte[] imageBytes;
            using (MemoryStream m = new MemoryStream())
            {
                agencyLogo.Save(m, agencyLogo.RawFormat);
                imageBytes = m.ToArray();
            }

            var existingAgencyLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "AgencyLogo").FirstOrDefault();
            if (existingAgencyLogo == null)
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "AgencyLogo",
                    Value = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imageBytes))
                });
            else
            {
                existingAgencyLogo.Value = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imageBytes));
                _unitOfWork.Context.AdminSettings.Update(existingAgencyLogo);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public string GetAgencyLogo()
        {
            var existingAgencyLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "AgencyLogo").FirstOrDefault();
            if (existingAgencyLogo == null) return "";
            if (!existingAgencyLogo.Value.StartsWith("data:image/jpg;base64,")) return "";
            return _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "AgencyLogo").FirstOrDefault().Value;
        }

        public void SetImplementationLogo(Image productLogo)
        {
            byte[] imageBytes;
            using (MemoryStream m = new MemoryStream())
            {
                productLogo.Save(m, productLogo.RawFormat);
                imageBytes = m.ToArray();
            }
            var implementationLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault();
            if (implementationLogo == null)
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "ImplementationLogo",
                    Value = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imageBytes))
                });
            else
            {
                implementationLogo.Value = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(imageBytes));
                _unitOfWork.Context.AdminSettings.Update(implementationLogo);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public string GetImplementationLogo()
        {
            var implementationLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault();
            if (implementationLogo == null) return "";
            if (!implementationLogo.Value.StartsWith("data:image/jpg;base64,")) return "";
            return _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault().Value;
        }

        public string GetConstraintType()
        {
            var existingConstraintType = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == constraintTypeKey).FirstOrDefault();
            if (existingConstraintType == null)
            {
                return null;
            }
            else
            {
                return existingConstraintType.Value;
            }
        }
        public void SetConstraintType(string constraintType)
        {
            var existingConstraintType = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == constraintTypeKey).FirstOrDefault();
            if (existingConstraintType == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = constraintTypeKey,
                    Value = constraintType
                });
            }
            else
            {
                existingConstraintType.Value = constraintType;
                _unitOfWork.Context.AdminSettings.Update(existingConstraintType);
            }
            _unitOfWork.Context.SaveChanges();
        }
    }
}
