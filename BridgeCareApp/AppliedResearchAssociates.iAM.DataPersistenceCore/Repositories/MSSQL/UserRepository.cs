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
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public UserRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void AddUser(string username, string role)
        {
            if (string.IsNullOrEmpty(username) || _unitOfWork.Context.User.Any(_ => _.Username == username))
            {
                return;
            }

            _unitOfWork.Context.User.Add(new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = username,
                HasInventoryAccess = !string.IsNullOrEmpty(role) && role == Role.Administrator
            });

            _unitOfWork.Context.SaveChanges();
        }

        public Task<List<UserDTO>> GetAllUsers()
        {
            if (!_unitOfWork.Context.User.Any())
            {
                return Task.Factory.StartNew(() => new List<UserDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.User
                .Include(_ => _.CriterionLibraryUserJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Select(_ => _.ToDto())
                .ToList());
        }
    }
}
