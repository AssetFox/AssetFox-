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

        public void DeleteUser(Guid userCriteriaId)
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
            // Need to write a entity to dto conversion extension method
            var result = new List<UserCriteriaDTO>();
            var data = _unitOfDataPersistenceWork.Context.UserCriteria
                .Include(_ => _.UserEntityJoin)
                .Select(_ => _).ToList();
            foreach (var item in data)
            {
                result.Add(item.ToDto());
            }
            return result;
        }
        public UserCriteriaDTO GetOwnUserCriteria(UserInfoDTO userInformation) => throw new NotImplementedException();
        public void SaveUserCriteria(UserCriteriaDTO model)
        {
            var user = _unitOfDataPersistenceWork.Context.User.FirstOrDefault(s => s.Username == model.UserName);
            var userCriteria = new UserCriteriaFilterEntity{
                UserEntityJoin = user
            };
            if (!_unitOfDataPersistenceWork.Context.UserCriteria.Any(criteria => criteria.UserEntityJoin.Username == model.UserName))
            {
                userCriteria.Criteria = model.Criteria;
                userCriteria.UserEntityJoin.Username = model.UserName;
                userCriteria.UserEntityJoin.HasInventoryAccess = model.HasAccess;
                userCriteria.HasCriteria = model.HasCriteria;
                userCriteria.UserCriteriaId = model.CriteriaId;
                _unitOfDataPersistenceWork.Context.Add(userCriteria);
            }
            else
            {
                userCriteria = _unitOfDataPersistenceWork.Context.UserCriteria.Single(criteria => criteria.UserEntityJoin.Username == model.UserName);
                //user = _unitOfDataPersistenceWork.Context.User.FirstOrDefault(s => s.Username == model.Username);
                userCriteria.UserEntityJoin = user;

                userCriteria.Criteria = model.Criteria;
                userCriteria.HasCriteria = model.HasCriteria;
                //userCriteria.UserEntityJoin.Username = model.UserName;
                userCriteria.UserEntityJoin.HasInventoryAccess = model.HasAccess;

                _unitOfDataPersistenceWork.Context.Update(userCriteria);
            }
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
