using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public class TransactionHistoryRepositoryEF : Repository<TransactionHistory>, ITransactionHistoryRepositoryEF
    {
        public TransactionHistoryRepositoryEF(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransactionHistory>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 10)
        {
            return await _context.TransactionHistories
                .Where(t => t.UserId == userId)
                .Include(t => t.GameAccount)
                .OrderByDescending(t => t.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionHistory>> GetAllTransactionsAsync(int page = 1, int pageSize = 10)
        {
            return await _context.TransactionHistories
                .Include(t => t.User)
                .Include(t => t.GameAccount)
                .OrderByDescending(t => t.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionHistory>> GetPendingTransactionsAsync()
        {
            return await _context.TransactionHistories
                .Where(t => t.Status == TransactionStatus.Pending)
                .Include(t => t.User)
                .Include(t => t.GameAccount)
                .OrderBy(t => t.Created)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _context.TransactionHistories
                .Where(t => t.UserId == userId)
                .CountAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.TransactionHistories.CountAsync();
        }
    }
}