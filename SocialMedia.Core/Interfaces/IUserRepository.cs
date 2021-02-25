using SocialMedia.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task InsertUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(int id);
    }
}