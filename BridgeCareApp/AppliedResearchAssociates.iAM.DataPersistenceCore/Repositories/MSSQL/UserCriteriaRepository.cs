using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class UserCriteriaRepository : IUserCriteriaRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public UserCriteriaRepository(UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void RevokeUserAccess(Guid userCriteriaId)
        {
            var criteriaToBedeleted = _unitOfDataPersistenceWork.Context.UserCriteria.FirstOrDefault(_ => _.UserCriteriaId == userCriteriaId);
            var userToBeChanged = _unitOfDataPersistenceWork.Context.User.FirstOrDefault(_ => _.Id == criteriaToBedeleted.UserId);
            userToBeChanged.HasInventoryAccess = false;
            _unitOfDataPersistenceWork.Context.User.Update(userToBeChanged);
            _unitOfDataPersistenceWork.Context.Delete<UserCriteriaFilterEntity>(_ => _.UserCriteriaId == userCriteriaId);
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
        public List<UserCriteriaDTO> GetAllUserCriteria()
        {
            var result = new List<UserCriteriaDTO>();
            var data = _unitOfDataPersistenceWork.Context.UserCriteria
                .Include(_ => _.User)
                .Select(_ => _).ToList();
            foreach (var item in data)
            {
                result.Add(item.ToDto());
            }
            return result;
        }
        public UserCriteriaDTO GetOwnUserCriteria(UserInfoDTO userInformation, string adminCheckConst)
        {
            // First time login
            if (!_unitOfDataPersistenceWork.Context.User.Any(u => u.Username == userInformation.Sub))
            {
                var newUser = GenerateDefaultUser(userInformation, adminCheckConst);
                _unitOfDataPersistenceWork.Context.User.Add(newUser);

                // if the newly logged in user is an admin
                var newCriteriaFilter = new UserCriteriaFilterEntity();
                if (newUser.HasInventoryAccess)
                {
                    newCriteriaFilter = GenerateDefaultCriteriaForAdmin(newUser);
                    _unitOfDataPersistenceWork.Context.UserCriteria.Add(newCriteriaFilter);
                }
                _unitOfDataPersistenceWork.Context.SaveChanges();

                // user does not have admin access, so don't enter the data in userCriteria_Filter table and return an empty object
                if (!newUser.HasInventoryAccess)
                {
                    return new UserCriteriaDTO { UserName = userInformation.Sub };
                }
                return newCriteriaFilter.ToDto();
            }
            var currUser = _unitOfDataPersistenceWork.Context.User.FirstOrDefault(_ => _.Username == userInformation.Sub);
            var userCriteria = _unitOfDataPersistenceWork.Context.UserCriteria.SingleOrDefault(criteria => criteria.User.Username == userInformation.Sub);

            if(userCriteria == null) // user is present in the user table, but doesn't have data in UserCriteria_Filter table (no inventory access)
            {
                return new UserCriteriaDTO{ UserName = userInformation.Sub };
            }
            userCriteria.User = currUser;
            return userCriteria.ToDto();
        }

        public void SaveUserCriteria(UserCriteriaDTO model)
        {
            var user = _unitOfDataPersistenceWork.Context.User.FirstOrDefault(s => s.Username == model.UserName);
            var userCriteria = new UserCriteriaFilterEntity{
                User = user
            };
            if (!_unitOfDataPersistenceWork.Context.UserCriteria.Any(criteria => criteria.User.Username == model.UserName))
            {
                userCriteria.Criteria = model.Criteria;
                userCriteria.User.Username = model.UserName;
                userCriteria.User.HasInventoryAccess = model.HasAccess;
                userCriteria.HasCriteria = model.HasCriteria;
                userCriteria.UserCriteriaId = model.CriteriaId;
                _unitOfDataPersistenceWork.Context.Add(userCriteria);
            }
            else
            {
                userCriteria = _unitOfDataPersistenceWork.Context.UserCriteria.Single(criteria => criteria.User.Username == model.UserName);
                userCriteria.User = user;

                userCriteria.Criteria = model.Criteria;
                userCriteria.HasCriteria = model.HasCriteria;
                userCriteria.User.HasInventoryAccess = model.HasAccess;

                _unitOfDataPersistenceWork.Context.Update(userCriteria);
            }
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        private UserEntity GenerateDefaultUser(UserInfoDTO userInformation, string adminCheckConst)
        {
            var newUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = userInformation.Sub,
                HasInventoryAccess = userInformation.Roles == adminCheckConst ? true : false
            };
            return newUser;
        }
        private UserCriteriaFilterEntity GenerateDefaultCriteriaForAdmin(UserEntity newUser)
        {
            var newUserCriteriaFilter = new UserCriteriaFilterEntity
            {
                UserCriteriaId = Guid.NewGuid(),
                HasCriteria = false, // because this user is admin, HasCriteria is set to false. Meaning, the user doesn't have any criteria filter
                CreatedBy = newUser.Id,
                LastModifiedBy = newUser.Id,
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                UserId = newUser.Id,
                User = newUser
            };
            return newUserCriteriaFilter;
        }

        public void DeleteUser(Guid userId)
        {
            _unitOfDataPersistenceWork.Context.Delete<UserCriteriaFilterEntity>(_ => _.UserId == userId);
            _unitOfDataPersistenceWork.Context.Delete<UserEntity>(_ => _.Id == userId);
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
