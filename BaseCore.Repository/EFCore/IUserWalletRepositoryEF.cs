using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public interface IUserWalletRepositoryEF : IRepository<UserWallet>
    {
        Task<UserWallet?> GetByUserIdAsync(int userId);
        Task<bool> UpdateBalanceAsync(int userId, decimal amount);
    }
}