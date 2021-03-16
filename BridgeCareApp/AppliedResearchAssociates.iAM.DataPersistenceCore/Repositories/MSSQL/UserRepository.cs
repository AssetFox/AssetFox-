using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class UserRepository : IUserRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public UserRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void AddUser(string username, string role)
        {
            if (string.IsNullOrEmpty(username) || _unitOfDataPersistenceWork.Context.User.Any(_ => _.Username == username))
            {
                return;
            }

            _unitOfDataPersistenceWork.Context.User.Add(new UserEntity
            {
                Id = Guid.NewGuid(), Username = username, HasInventoryAccess = !string.IsNullOrEmpty(role) && role == Role.Administrator
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public Task<List<UserDTO>> GetAllUsers()
        {
            if (!_unitOfDataPersistenceWork.Context.User.Any())
            {
                return Task.Factory.StartNew(() => new List<UserDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfDataPersistenceWork.Context.User
                .Include(_ => _.CriterionLibraryUserJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList());
        }
    }
}
