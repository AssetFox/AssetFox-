using System;
using System.Data;
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
                }) ;
            }
            else
            {
                existingImplementationName.Value = name;
                _unitOfWork.Context.AdminSettings.Update(existingImplementationName);
            }
            _unitOfWork.Context.SaveChanges();
        }
    }
}
