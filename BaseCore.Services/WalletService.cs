using System.Threading.Tasks;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUserWalletRepositoryEF _walletRepository;
        private readonly ITransactionHistoryRepositoryEF _transactionRepository;

        public WalletService(
            IUserWalletRepositoryEF walletRepository,
            ITransactionHistoryRepositoryEF transactionRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<UserWallet> GetWalletByUserIdAsync(int userId)
        {
            return await _walletRepository.GetByUserIdAsync(userId);
        }

        public async Task<UserWallet> CreateWalletAsync(int userId)
        {
            var wallet = new UserWallet
            {
                UserId = userId,
                Balance = 0
            };

            return await _walletRepository.AddAsync(wallet);
        }

        public async Task<bool> DepositAsync(int userId, decimal amount, string paymentMethod, string transactionCode)
        {
            if (amount <= 0)
                return false;

            // Update wallet balance
            var success = await _walletRepository.UpdateBalanceAsync(userId, amount);
            if (!success)
                return false;

            // Create transaction record
            var transaction = new TransactionHistory
            {
                UserId = userId,
                Type = TransactionType.Deposit,
                Amount = amount,
                Description = $"Deposit via {paymentMethod}",
                Status = TransactionStatus.Completed,
                PaymentMethod = paymentMethod,
                CompletedAt = System.DateTime.Now
            };

            await _transactionRepository.AddAsync(transaction);
            return true;
        }

        public async Task<bool> WithdrawAsync(int userId, decimal amount, string paymentMethod)
        {
            if (amount <= 0)
                return false;

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < amount)
                return false;

            // Update wallet balance
            var success = await _walletRepository.UpdateBalanceAsync(userId, -amount);
            if (!success)
                return false;

            // Create transaction record
            var transaction = new TransactionHistory
            {
                UserId = userId,
                Type = TransactionType.Withdraw,
                Amount = -amount,
                Description = $"Withdraw via {paymentMethod}",
                Status = TransactionStatus.Pending, // Withdraw needs admin approval
                PaymentMethod = paymentMethod
            };

            await _transactionRepository.AddAsync(transaction);
            return true;
        }

        public async Task<bool> UpdateBalanceAsync(int userId, decimal amount)
        {
            var success = await _walletRepository.UpdateBalanceAsync(userId, amount);
            if (success)
            {
                // Log admin adjustment as a transaction
                var transaction = new TransactionHistory
                {
                    UserId = userId,
                    Type = amount >= 0 ? TransactionType.Deposit : TransactionType.Withdraw,
                    Amount = amount,
                    Description = $"Admin balance adjustment: {(amount >= 0 ? "+" : "")}{amount}",
                    Status = TransactionStatus.Completed,
                    PaymentMethod = "Admin System",
                    CompletedAt = System.DateTime.Now
                };
                await _transactionRepository.AddAsync(transaction);
            }
            return success;
        }
    }
}