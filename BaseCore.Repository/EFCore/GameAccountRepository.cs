using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public class GameAccountRepositoryEF : Repository<GameAccount>, IGameAccountRepositoryEF
    {
        public GameAccountRepositoryEF(MySqlDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GameAccount>> GetAvailableAccountsAsync()
        {
            return await _context.GameAccounts
                .Where(ga => !ga.IsSold)
                .Include(ga => ga.Seller)
                .OrderByDescending(ga => ga.CreatedDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsBySellerAsync(int sellerId)
        {
            return await _context.GameAccounts
                .Where(ga => ga.SellerId == sellerId)
                .Include(ga => ga.Buyer)
                .OrderByDescending(ga => ga.CreatedDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsByGameAsync(string gameName)
        {
            return await _context.GameAccounts
                .Where(ga => ga.GameName != null && ga.GameName.Contains(gameName) && !ga.IsSold)
                .Include(ga => ga.Seller)
                .OrderByDescending(ga => ga.CreatedDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsByBuyerAsync(int buyerId)
        {
            return await _context.GameAccounts
                .Where(ga => ga.BuyerId == buyerId)
                .Include(ga => ga.Seller)
                .OrderByDescending(ga => ga.SoldAt)
                .ToListAsync();
        }

        public async Task<GameAccount?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.GameAccounts
                .Include(ga => ga.Seller)
                .Include(ga => ga.Buyer)
                .FirstOrDefaultAsync(ga => ga.Id == id);
        }
    }
}