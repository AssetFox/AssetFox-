using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class UserRepository : IUserRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public UserRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void AddUser(string username, bool hasAdminClaim)
        {
            if (string.IsNullOrEmpty(username) || _unitOfWork.Context.User.Any(_ => _.Username == username))
            {
                return;
            }

            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.User.Add(new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    HasInventoryAccess = hasAdminClaim,
                    ActiveStatus = true
                });
                
            });
        }

        public void UpdateLastNewsAccessDate(Guid id, DateTime accessDate)
        {
            var user = _unitOfWork.Context.User.Where(x => x.Id == id).FirstOrDefault();

            if (user == null)
            {
                return;
            }

            user.LastNewsAccessDate = accessDate;
            _unitOfWork.Context.User.Update(user);
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

        public Task<UserDTO> GetUserByUserName(string userName)
        {
            var existingUser = _unitOfWork.Context.User.Where(_ => _.Username == userName).FirstOrDefault();
            if (existingUser == null)
            {
                // This is satisfactory given the frontend only needs
                // the username. The was introduced when the asynchronicity
                // was implemented for getting the user. It may try to obtain
                // a user before it is created in the database, hence this code.
                return Task.Factory.StartNew(() =>
                    new UserDTO
                    {
                        Id = Guid.Empty,
                        Username = userName,
                        HasInventoryAccess = false,
                        LastNewsAccessDate = new DateTime(),
                        CriterionLibrary = new CriterionLibraryDTO()
                    }
                    );
            }
            else return Task.Factory.StartNew(() => _unitOfWork.Context.User.Where(_ => _.Username == userName).FirstOrDefault().ToDto());
        }

        public bool UserExists(string userName)
        {
            return _unitOfWork.Context.User.Any(_ => _.Username == userName);
        }

        public Task<UserDTO> GetUserById(Guid id)
        {
            return Task.Factory.StartNew(() => _unitOfWork.Context.User.Where(_ => _.Id == id).FirstOrDefault().ToDto());
        }
    }
}
