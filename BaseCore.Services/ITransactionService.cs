using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionHistory>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 10);
        Task<IEnumerable<TransactionHistory>> GetAllTransactionsAsync(int page = 1, int pageSize = 10);
        Task<TransactionHistory?> GetByIdAsync(int id);
        Task<TransactionHistory> CreateAsync(TransactionHistory transaction);
        Task UpdateAsync(TransactionHistory transaction);
        Task<IEnumerable<TransactionHistory>> GetPendingTransactionsAsync();
        Task<int> GetTotalCountAsync(int userId);
        Task<int> GetTotalCountAllAsync();
    }
}