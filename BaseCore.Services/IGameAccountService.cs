using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public interface IGameAccountService
    {
        Task<IEnumerable<GameAccount>> GetAvailableAccountsAsync();
        Task<IEnumerable<GameAccount>> GetAccountsBySellerAsync(int sellerId);
        Task<IEnumerable<GameAccount>> GetAccountsByBuyerAsync(int buyerId);
        Task<IEnumerable<GameAccount>> GetAccountsByGameAsync(string gameName);
        Task<IEnumerable<GameAccount>> GetAllAsync();
        Task<GameAccount?> GetByIdAsync(int id);
        Task<GameAccount?> CreateAsync(GameAccount gameAccount);
        Task UpdateAsync(GameAccount gameAccount);
        Task DeleteAsync(int id);
        Task<bool> PurchaseAccountAsync(int accountId, int buyerId);
    }
}