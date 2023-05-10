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

        public AdminDataRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public string GetPrimaryNetwork()
        {
            var existingPrimaryNetwork = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "PrimaryNetwork").FirstOrDefault();
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
            var existingPrimaryNetwork = _unitOfWork.Context.AdminSettings.Where(_ => _.Key == "PrimaryNetwork").FirstOrDefault();
            if (!_unitOfWork.Context.Network.Any(_ => _.Name == name))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }
            else if (existingPrimaryNetwork == null)
            {
                _unitOfWork.Context.AdminSettings.Add(new AdminSettingsEntity
                {
                    Key = "PrimaryNetwork",
                    Value = name
                });
            }
            else
            {
                existingPrimaryNetwork.Value = name;
                _unitOfWork.Context.AdminSettings.Update(existingPrimaryNetwork);
            }
            _unitOfWork.Context.SaveChanges();
        }
    }
}
