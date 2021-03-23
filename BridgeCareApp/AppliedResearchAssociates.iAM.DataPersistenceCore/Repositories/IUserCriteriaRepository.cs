using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IUserCriteriaRepository
    {
        public UserCriteriaDTO GetOwnUserCriteria(UserInfoDTO userInformation);
        public List<UserCriteriaDTO> GetAllUserCriteria();
        public void SaveUserCriteria(UserCriteriaDTO model);
        public void DeleteUser(Guid userCriteriaId);
    }
}
