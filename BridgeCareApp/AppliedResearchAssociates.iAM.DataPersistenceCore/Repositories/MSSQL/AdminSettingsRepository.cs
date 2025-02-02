using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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

        public const string inventoryReportKey = "InventoryReportNames";
        public const string simulationReportKey = "SimulationReportNames";
        public const string keyFieldKey = "KeyFields";
        public const string rawDataFieldKey = "RawDataKeyFields";
        public const string primaryNetworkKey = "PrimaryNetwork";
        public const string rawDataNetworkKey = "RawDataNetwork";
        public const string assetTypeKey = "AssetType";
        public const string constraintTypeKey = "ConstraintType";

        //Reads in KeyFields record as a string but places values in a list to return.
        public IList<string> GetKeyFields()
        {
            var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == keyFieldKey).FirstOrDefault();
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

        public IList<string> GetRawKeyFields()
        {
            var existingRawKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == rawDataFieldKey).FirstOrDefault();
            if (existingRawKeyFields == null)
            {
                return null;
            }
            else
            {
                var rawKeyFields = existingRawKeyFields.Value;
                IList<string> RawKeyFieldsList = rawKeyFields.Split(',').ToList();
                return RawKeyFieldsList;
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
                var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == keyFieldKey).SingleOrDefault();
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

        //Reads in KeyFields record as a string but places values in a list to return.
        public IList<string> GetRawDataKeyFields()
        {
            var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == rawDataFieldKey).FirstOrDefault();
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
        public void SetRawDataKeyFields(string keyFields)
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
                var existingKeyFields = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == rawDataFieldKey).SingleOrDefault();
                var KeyFieldsString = string.Join(",", keyFields);
                //If the entry doesn't exist in the AdminSettings table
                if (existingKeyFields == null)
                {
                    _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                    {
                        Key = rawDataFieldKey,
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

        //Reads in AssetType record as a string but places values in a list to return.
        public IList<string> GetAssetType()
        {
            var existingAssetType = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == assetTypeKey).FirstOrDefault();
            if (existingAssetType == null)
            {
                return null;
            }
            else
            {
                var assetType = existingAssetType.Value;
                IList<string> AssetTypeList = assetType.Split(',').ToList();
                return AssetTypeList;
            }
        }

        //String is to be passed in as parameter. Sets the AssetType in the AdminSettings table. 
        public void SetAssetType(string assetType)
        {
            var existingAssetType = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == assetTypeKey).FirstOrDefault();
            if (existingAssetType == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = assetTypeKey,
                    Value = assetType
                });
            }
            else
            {
                existingAssetType.Value = assetType;
                _unitOfWork.Context.AdminSettings.Update(existingAssetType);
            }
            _unitOfWork.Context.SaveChanges();
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
                return existingNetwork.Name;
            }
        }

        public string GetRawDataNetwork()
        {
            var existingRawDataNetwork = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == rawDataNetworkKey);
            if (existingRawDataNetwork == null)
            {
                return null;
            }
            var rawDataNetworkGuid = new Guid(existingRawDataNetwork.Value);
            var existingNetwork = _unitOfWork.Context.Network.SingleOrDefault(_ => _.Id == rawDataNetworkGuid);

            if (existingNetwork == null)
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

        public void SetRawDataNetwork(string name)
        {
            var existingNetworkAdminSetting = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == rawDataNetworkKey).FirstOrDefault();
            var existingNetwork = _unitOfWork.Context.Network.FirstOrDefault(_ => _.Name == name);
            if (existingNetwork == null) {
                throw new RowNotInTableException("The specified network was not found.");
            }
            if (existingNetworkAdminSetting == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity { Key = rawDataNetworkKey, Value = existingNetwork.Id.ToString() });
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

        public void SetAgencyLogo(Image agencyLogo, string ImageType)
        {
            //https://www.andrewhoefling.com/Blog/Post/basic-image-manipulation-in-c-sharp
            int h = 50;
            float ratio = (float)agencyLogo.Width / (float)agencyLogo.Height;
            int w = (int)(ratio * h);
            if (agencyLogo.Width > w || agencyLogo.Height > h)
                agencyLogo = agencyLogo.GetThumbnailImage(w, h, null, IntPtr.Zero);
            //https://stackoverflow.com/questions/21325661/convert-an-image-selected-by-path-to-base64-string
            byte[] imageBytes;
            if (ImageType.Contains("png"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    agencyLogo.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = m.ToArray();
                }
            }
            else
            {
                using (MemoryStream m = new MemoryStream())
                {
                    agencyLogo.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageBytes = m.ToArray();
                }
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
            if (!existingAgencyLogo.Value.StartsWith("data:image/")) return "";
           
            return _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "AgencyLogo").FirstOrDefault().Value;
        }

        public void SetImplementationLogo(Image productLogo, string ImageType)
        {
            int h = 50;
            float ratio = (float)productLogo.Width / (float)productLogo.Height;
            int w = (int)(ratio * h);
            if (productLogo.Width > w || productLogo.Height > h)
                productLogo = productLogo.GetThumbnailImage(w, h, null, IntPtr.Zero);

            byte[] imageBytes;
            if (ImageType.Contains("png"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    productLogo.Save(m, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = m.ToArray();
                }
            }
            else
            {
                using (MemoryStream m = new MemoryStream())
                {
                    productLogo.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageBytes = m.ToArray();
                }
            }

            
            var implementationLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault();
            if (implementationLogo == null)
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "ImplementationLogo",
                    Value = string.Format("data:"+ImageType+";base64,{0}", Convert.ToBase64String(imageBytes))
                });
            else
            {
                implementationLogo.Value = string.Format("data:" + ImageType + ";base64,{0}", Convert.ToBase64String(imageBytes));
                _unitOfWork.Context.AdminSettings.Update(implementationLogo);
            }
            _unitOfWork.Context.SaveChanges();
        }

        public string GetImplementationLogo()
        {
            var implementationLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault();
            if (implementationLogo == null) return "";
            if (!implementationLogo.Value.StartsWith("data:image/")) return "";

            return _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault().Value;
        }
        public void SetImplementationLogo(byte[] temp)
        {
            var implementationLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "ImplementationLogo").FirstOrDefault();
            if (implementationLogo == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "ImplementationLogo",
                    Value = string.Format("data:image/svg+xml;base64,{0}", Convert.ToBase64String(temp))
                }) ;
            }    
            else
            {
                implementationLogo.Value = string.Format("data:image/svg+xml;base64,{0}", Convert.ToBase64String(temp));
                _unitOfWork.Context.AdminSettings.Update(implementationLogo);
            }
            _unitOfWork.Context.SaveChanges();
            

        }
        public void SetAgencyLogo(byte[] bytes)
        {
            var agencyLogo = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "AgencyLogo").FirstOrDefault();
            if (agencyLogo == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "AgencyLogo",
                    Value = string.Format("data:image/svg+xml;base64,{0}", Convert.ToBase64String(bytes))
                });
            }
            else
            {
                agencyLogo.Value = string.Format("data:image/svg+xml;base64,{0}", Convert.ToBase64String(bytes));
                _unitOfWork.Context.AdminSettings.Update(agencyLogo);
            }
            _unitOfWork.Context.SaveChanges();


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
        public void DeleteAdminSetting(string settingKey)
        {
            _unitOfWork.Context.DeleteEntity<AdminSettingsEntity>(_ => _.Key == settingKey);
        }

        public Guid? GetPrimaryNetworkId()
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
                return existingNetwork.Id;
            }

        }
        public Guid? GetRawDataNetworkId()
        {
            var existingRawDataNetwork = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == rawDataNetworkKey);
            if (existingRawDataNetwork == null)
            {
                return null;
            }
            var rawDataNetworkGuid = new Guid(existingRawDataNetwork.Value);
            var existingNetwork = _unitOfWork.Context.Network.SingleOrDefault(_ => _.Id == rawDataNetworkGuid);

            if (existingNetwork == null)
            {
                return null;
            }
            else
            {
                return existingNetwork.Id;
            }

        }

        public string GetAdminContactEmail()
        {
            var existingAdminContactEmail = _unitOfWork.Context.AdminSettings
                .Where(_ => _.Key == "AdminContactEmail")
                .FirstOrDefault();

            if (existingAdminContactEmail == null)
            {
                return null;
            }

            return existingAdminContactEmail.Value;
        }

        public void SetAdminContactEmail(string email)
        {
            var existingAdminContactEmail = _unitOfWork.Context.AdminSettings
                .Where(_ => _.Key == "AdminContactEmail")
                .FirstOrDefault();

            if (existingAdminContactEmail == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "AdminContactEmail",
                    Value = email
                });
            }
            else
            {
                existingAdminContactEmail.Value = email;
                _unitOfWork.Context.AdminSettings.Update(existingAdminContactEmail);
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}
