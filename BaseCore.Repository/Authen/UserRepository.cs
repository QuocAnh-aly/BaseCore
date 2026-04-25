using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Repository.Authen
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlDbContext _context;

        public UserRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(List<User> Users, int TotalCount)> SearchAsync(string keyword, int page, int pageSize)
        {
            var query = _context.Users.Where(u => u.IsActive);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.Contains(keyword)) ||
                    (u.Name != null && u.Name.Contains(keyword)) ||
                    (u.Email != null && u.Email.Contains(keyword)) ||
                    (u.Phone != null && u.Phone.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            try
            {
                // Fetch balances for the current page
                var userIds = users.Select(u => u.Id).ToList();
                var wallets = await _context.UserWallets
                    .Where(w => userIds.Contains(w.UserId))
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.Balance = wallets.FirstOrDefault(w => w.UserId == user.Id)?.Balance ?? 0;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the whole user list
                System.Console.WriteLine($"Error fetching balances: {ex.Message}");
            }

            return (users, totalCount);
        }

        public async Task<int> GetTotalCountAsync(string keyword)
        {
            var query = _context.Users.Where(u => u.IsActive);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.Contains(keyword)) ||
                    (u.Name != null && u.Name.Contains(keyword)) ||
                    (u.Email != null && u.Email.Contains(keyword)) ||
                    (u.Phone != null && u.Phone.Contains(keyword)));
            }

            return await query.CountAsync();
        }
    }
}