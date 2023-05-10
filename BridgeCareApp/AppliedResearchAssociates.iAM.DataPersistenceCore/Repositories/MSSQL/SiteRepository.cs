using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Org.BouncyCastle.Asn1.Cms;
using SQLitePCL;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SiteRepository : ISiteRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SiteRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

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
    }
}
