using BaseCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCore.Services.Authen
{
    public interface IUserService
    {
        Task<User?> Authenticate(string username, string password);
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User?> Create(User user, string password);
        Task Update(User user, string password);
        Task Delete(int id);
        Task<(List<User> Users, int TotalCount)> Search(string keyword, int page, int pageSize);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<int> GetTotalCountAsync(string keyword);
    }
}