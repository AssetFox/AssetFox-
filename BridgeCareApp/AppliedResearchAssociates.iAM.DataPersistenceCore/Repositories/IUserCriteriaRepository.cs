using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IUserCriteriaRepository
    {
        public UserCriteriaDTO GetOwnUserCriteria(UserInfoDTO userInfo, string adminCheckConst);
        public List<UserCriteriaDTO> GetAllUserCriteria();
        public void UpsertUserCriteria(UserCriteriaDTO dto);
        public void DeleteUser(Guid userId);
        public void RevokeUserAccess(Guid userCriteriaId);
    }
}
