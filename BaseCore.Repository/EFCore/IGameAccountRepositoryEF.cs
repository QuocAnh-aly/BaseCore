using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{
    public interface IGameAccountRepositoryEF : IRepository<GameAccount>
    {
        Task<IEnumerable<GameAccount>> GetAvailableAccountsAsync();
        Task<IEnumerable<GameAccount>> GetAccountsBySellerAsync(int sellerId);
        Task<IEnumerable<GameAccount>> GetAccountsByBuyerAsync(int buyerId);
        Task<IEnumerable<GameAccount>> GetAccountsByGameAsync(string gameName);
        Task<GameAccount?> GetByIdWithDetailsAsync(int id);
    }
}