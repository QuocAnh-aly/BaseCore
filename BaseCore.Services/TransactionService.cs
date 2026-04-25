using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionHistoryRepositoryEF _transactionRepository;

        public TransactionService(ITransactionHistoryRepositoryEF transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<TransactionHistory>> GetUserTransactionsAsync(int userId, int page = 1, int pageSize = 10)
        {
            return await _transactionRepository.GetByUserIdAsync(userId, page, pageSize);
        }

        public async Task<IEnumerable<TransactionHistory>> GetAllTransactionsAsync(int page = 1, int pageSize = 10)
        {
            return await _transactionRepository.GetAllTransactionsAsync(page, pageSize);
        }

        public async Task<TransactionHistory?> GetByIdAsync(int id)
        {
            return await _transactionRepository.GetByIdAsync(id);
        }

        public async Task<TransactionHistory> CreateAsync(TransactionHistory transaction)
        {
            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task UpdateAsync(TransactionHistory transaction)
        {
            await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task<IEnumerable<TransactionHistory>> GetPendingTransactionsAsync()
        {
            return await _transactionRepository.GetPendingTransactionsAsync();
        }

        public async Task<int> GetTotalCountAsync(int userId)
        {
            return await _transactionRepository.GetTotalCountByUserIdAsync(userId);
        }

        public async Task<int> GetTotalCountAllAsync()
        {
            return await _transactionRepository.GetTotalCountAsync();
        }
    }
}