using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public class UserWalletRepositoryEF : Repository<UserWallet>, IUserWalletRepositoryEF
    {
        public UserWalletRepositoryEF(MySqlDbContext context) : base(context)
        {
        }

        public async Task<UserWallet?> GetByUserIdAsync(int userId)
        {
            return await _context.UserWallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<bool> UpdateBalanceAsync(int userId, decimal amount)
        {
            var wallet = await GetByUserIdAsync(userId);
            if (wallet == null)
            {
                wallet = new UserWallet
                {
                    UserId = userId,
                    Balance = amount,
                    TotalSpent = 0,
                    UpdatedDateTime = System.DateTime.Now
                };
                await _context.UserWallets.AddAsync(wallet);
            }
            else
            {
                wallet.Balance += amount;
                wallet.UpdatedDateTime = System.DateTime.Now;
                _context.UserWallets.Update(wallet);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}