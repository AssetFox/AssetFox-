using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IUserRepository
    {
        void AddUser(string username, bool hasAdminClaim);

        void UpdateLastNewsAccessDate(Guid id, DateTime accessDate);

        Task<List<UserDTO>> GetAllUsers();

        Task<UserDTO> GetUserByUserName(string username);

        bool UserExists(string userName);

        Task<UserDTO> GetUserById(Guid id);
    }
}
