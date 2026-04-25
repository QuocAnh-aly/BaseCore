using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public class GameAccountService : IGameAccountService
    {
        private readonly IGameAccountRepositoryEF _gameAccountRepository;
        private readonly IUserWalletRepositoryEF _walletRepository;
        private readonly ITransactionHistoryRepositoryEF _transactionRepository;

        public GameAccountService(
            IGameAccountRepositoryEF gameAccountRepository,
            IUserWalletRepositoryEF walletRepository,
            ITransactionHistoryRepositoryEF transactionRepository)
        {
            _gameAccountRepository = gameAccountRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<GameAccount>> GetAvailableAccountsAsync()
        {
            return await _gameAccountRepository.GetAvailableAccountsAsync();
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsBySellerAsync(int sellerId)
        {
            return await _gameAccountRepository.GetAccountsBySellerAsync(sellerId);
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsByBuyerAsync(int buyerId)
        {
            return await _gameAccountRepository.GetAccountsByBuyerAsync(buyerId);
        }

        public async Task<IEnumerable<GameAccount>> GetAccountsByGameAsync(string gameName)
        {
            return await _gameAccountRepository.GetAccountsByGameAsync(gameName);
        }

        public async Task<IEnumerable<GameAccount>> GetAllAsync()
        {
            return await _gameAccountRepository.GetAllAsync();
        }

        public async Task<GameAccount?> GetByIdAsync(int id)
        {
            return await _gameAccountRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<GameAccount?> CreateAsync(GameAccount gameAccount)
        {
            return await _gameAccountRepository.AddAsync(gameAccount);
        }

        public async Task UpdateAsync(GameAccount gameAccount)
        {
            await _gameAccountRepository.UpdateAsync(gameAccount);
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _gameAccountRepository.GetByIdAsync(id);
            if (account != null)
            {
                await _gameAccountRepository.DeleteAsync(account);
            }
        }

        public async Task<bool> PurchaseAccountAsync(int accountId, int buyerId)
        {
            var account = await _gameAccountRepository.GetByIdWithDetailsAsync(accountId);
            if (account == null || account.IsSold || !account.SellerId.HasValue)
                return false;

            var buyerWallet = await _walletRepository.GetByUserIdAsync(buyerId);
            if (buyerWallet == null || buyerWallet.Balance < account.Price)
                return false;

            // Update account status
            account.IsSold = true;
            account.BuyerId = buyerId;
            account.SoldAt = System.DateTime.Now;
            await _gameAccountRepository.UpdateAsync(account);

            // Update buyer wallet
            await _walletRepository.UpdateBalanceAsync(buyerId, -account.Price);

            // Update seller wallet
            await _walletRepository.UpdateBalanceAsync(account.SellerId.Value, account.Price);

            // Create transaction records
            var buyerTransaction = new TransactionHistory
            {
                UserId = buyerId,
                Type = TransactionType.Purchase,
                Amount = -account.Price,
                Description = $"Purchased {account.GameName} account",
                Status = TransactionStatus.Completed,
                GameAccountId = accountId,
                CompletedAt = System.DateTime.Now
            };

            var sellerTransaction = new TransactionHistory
            {
                UserId = account.SellerId.Value,
                Type = TransactionType.Sale,
                Amount = account.Price,
                Description = $"Sold {account.GameName} account",
                Status = TransactionStatus.Completed,
                GameAccountId = accountId,
                CompletedAt = System.DateTime.Now
            };

            await _transactionRepository.AddAsync(buyerTransaction);
            await _transactionRepository.AddAsync(sellerTransaction);

            return true;
        }
    }
}