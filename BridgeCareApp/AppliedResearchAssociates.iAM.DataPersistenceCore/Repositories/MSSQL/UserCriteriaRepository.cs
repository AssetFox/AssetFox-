﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public UserCriteriaRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void RevokeUserAccess(Guid userCriteriaId)
        {
            if (!_unitOfWork.Context.UserCriteria.Any(_ => _.UserCriteriaId == userCriteriaId))
            {
                return;
            }

            var userCriteriaFilterEntity =
                _unitOfWork.Context.UserCriteria.AsNoTracking()
                    .Include(_ => _.User)
                    .Single(_ => _.UserCriteriaId == userCriteriaId);

            userCriteriaFilterEntity.User.HasInventoryAccess = false;

            _unitOfWork.Context.UpdateEntity(userCriteriaFilterEntity.User, userCriteriaFilterEntity.User.Id,
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.DeleteEntity<UserCriteriaFilterEntity>(_ => _.UserCriteriaId == userCriteriaId);
        }

        public List<UserCriteriaDTO> GetAllUserCriteria()
        {
            if (!_unitOfWork.Context.UserCriteria.Any())
            {
                return new List<UserCriteriaDTO>();
            }

            return _unitOfWork.Context.UserCriteria
                .Include(_ => _.User)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public UserCriteriaDTO GetOwnUserCriteria(UserInfoDTO userInfo, string adminCheckConst)
        {
            // First time login
            if (!_unitOfWork.Context.User.Any(_ => _.Username == userInfo.Sub))
            {
                var newUserEntity = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Username = userInfo.Sub,
                    HasInventoryAccess = userInfo.Roles.Contains(adminCheckConst)
                };
                _unitOfWork.Context.AddEntity(newUserEntity, newUserEntity.Id);

                // if the newly logged in user is an admin
                switch (newUserEntity.HasInventoryAccess)
                {
                case true:
                    var newCriteriaFilter = newUserEntity.GenerateDefaultCriteriaForAdmin();
                    _unitOfWork.Context.AddEntity(newCriteriaFilter, newUserEntity.Id);
                    return newCriteriaFilter.ToDto();
                    break;
                // user does not have admin access, so don't enter the data in userCriteria_Filter table and return an empty object
                case false:
                    return new UserCriteriaDTO { UserName = userInfo.Sub };
                }
            }

            var userEntity = _unitOfWork.Context.User
                .Include(_ => _.UserCriteriaFilterJoin)
                .Single(_ => _.Username == userInfo.Sub);

            if (userEntity.UserCriteriaFilterJoin == null) // user is present in the user table, but doesn't have data in UserCriteria_Filter table
            {
                if (!userEntity.HasInventoryAccess)
                {
                    return new UserCriteriaDTO {UserName = userInfo.Sub, HasAccess = userEntity.HasInventoryAccess};
                }

                var newCriteriaFilter = userEntity.GenerateDefaultCriteriaForAdmin();
                _unitOfWork.Context.AddEntity(newCriteriaFilter, userEntity.Id);
                return newCriteriaFilter.ToDto();
            }

            return userEntity.UserCriteriaFilterJoin.ToDto();
        }

        public void UpsertUserCriteria(UserCriteriaDTO dto)
        {
            if (!_unitOfWork.Context.User.Any(_ => _.Id == dto.UserId))
            {
                throw new RowNotInTableException($"No user found having id {dto.UserId}.");
            }

            _unitOfWork.Context.Upsert(dto.ToEntity(), _ => _.UserId == dto.UserId, _unitOfWork.UserEntity?.Id);

            var userEntity = new UserEntity
            {
                Id = dto.UserId, Username = dto.UserName, HasInventoryAccess = dto.HasAccess
            };
            _unitOfWork.Context.UpdateEntity(userEntity, dto.UserId, _unitOfWork.UserEntity?.Id);
        }

        public void DeleteUser(Guid userId)
        {
            if (!_unitOfWork.Context.User.Any(_ => _.Id == userId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<UserEntity>(_ => _.Id == userId);
        }
    }
}
