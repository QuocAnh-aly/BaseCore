using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public interface IWalletService
    {
        Task<UserWallet> GetWalletByUserIdAsync(int userId);
        Task<UserWallet> CreateWalletAsync(int userId);
        Task<bool> DepositAsync(int userId, decimal amount, string paymentMethod, string transactionCode);
        Task<bool> WithdrawAsync(int userId, decimal amount, string paymentMethod);
        Task<bool> UpdateBalanceAsync(int userId, decimal amount);
    }
}