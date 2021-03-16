using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IUserRepository
    {
        void AddUser(string username, string role);

        Task<List<UserDTO>> GetAllUsers();
    }
}
