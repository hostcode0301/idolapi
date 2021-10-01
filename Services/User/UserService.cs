using System.Linq;
using System.Threading.Tasks;
using idolapi.DB;
using idolapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace idolapi.Services
{
    public class UserService : IUserService
    {
        private DataContext _dataContext;

        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Get user from database by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A user with that username / null</returns>
        public async Task<User> GetUserByUsername(string username)
        {
            return await _dataContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get a user from database by userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A user with that id / null</returns>
        public async Task<User> GetUserById(int id)
        {
            return await _dataContext.Users.Where(u => u.UserId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert new user to data base if user does not existed
        /// </summary>
        /// <param name="user"></param>
        /// <returns>-1: user existed / 0: fail / 1: insert done</returns>
        public async Task<int> InsertUser(User user)
        {
            if (_dataContext.Users.Any(u => u.Username == user.Username))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Users.Add(user);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }

    }
}