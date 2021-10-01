using System.Threading.Tasks;
using idolapi.DB.Models;

namespace idolapi.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserById(int id);
        Task<int> InsertUser(User user);
    }
}