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

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SiteRepository : ISiteRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public SiteRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public string GetImplementationName()
        {
            if (_unitOfWork.Context.AdminSettings.FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                var name = _unitOfWork.Context.AdminSettings.First().ImplementationName;
                return name;
            }
            
        }

        public void SetImplementationName(string name)
        {
            var settings = _unitOfWork.Context.AdminSettings.FirstOrDefault();
            //If there isn't an existing entry in the database, it adds it.
            if (settings == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingEntity
                {
                    ImplementationName = name
                });
            }
            //If there is an existing entry in the database, it updates it.
            else
            {               
                if (name == null)
                {
                    return;
                }
                settings.ImplementationName = name;
                _unitOfWork.Context.AdminSettings.Update(settings);              
            }
            _unitOfWork.Context.SaveChanges();
        }
    }
}
