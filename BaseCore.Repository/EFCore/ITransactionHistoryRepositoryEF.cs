using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public interface ITransactionHistoryRepositoryEF : IRepository<TransactionHistory>
    {
        Task<IEnumerable<TransactionHistory>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10);
        Task<IEnumerable<TransactionHistory>> GetAllTransactionsAsync(int page = 1, int pageSize = 10);
        Task<IEnumerable<TransactionHistory>> GetPendingTransactionsAsync();
        Task<int> GetTotalCountByUserIdAsync(int userId);
        Task<int> GetTotalCountAsync();
    }
}