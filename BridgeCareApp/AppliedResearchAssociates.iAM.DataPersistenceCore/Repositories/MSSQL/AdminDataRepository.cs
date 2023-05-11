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
    public class AdminDataRepository : IAdminDataRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        private readonly NetworkRepository _networkRepo;

        public AdminDataRepository(UnitOfDataPersistenceWork unitOfWork, NetworkRepository networkRepo)
        { 
                _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
                _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public string GetPrimaryNetwork()
        {
            var existingPrimaryNetwork = _unitOfWork.Context.AdminSettings.SingleOrDefault(_ => _.Key == "PrimaryNetwork");
            if (existingPrimaryNetwork == null)               
            {
                return null;
            }
            else
            {
                var name = _unitOfWork.Context.AdminSettings.First().Value;
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
    }
}
