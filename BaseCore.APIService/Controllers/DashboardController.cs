using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Services;
using System.Threading.Tasks;
using System.Linq;
using BaseCore.Entities;
using BaseCore.Services.Authen;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IGameAccountService _gameAccountService;
        private readonly IUserService _userService;

        public DashboardController(
            ITransactionService transactionService,
            IGameAccountService gameAccountService,
            IUserService userService)
        {
            _transactionService = transactionService;
            _gameAccountService = gameAccountService;
            _userService = userService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                // In a real app, this should be an optimized repository call
                var allTransactions = await _transactionService.GetAllTransactionsAsync(1, 1000); 
                var revenue = allTransactions
                    .Where(t => t.Type == TransactionType.Deposit && t.Status == TransactionStatus.Completed)
                    .Sum(t => t.Amount);

                var accounts = await _gameAccountService.GetAllAsync();
                var soldCount = accounts.Count(a => a.IsSold);
                var availableCount = accounts.Count(a => !a.IsSold);

                var usersCount = await _userService.GetTotalCountAsync("");

                return Ok(new
                {
                    revenue,
                    soldCount,
                    availableCount,
                    usersCount,
                    totalAccounts = accounts.Count()
                });
            }
            catch (System.Exception ex)
            {
                // Log the full exception details
                var errorMsg = $"Error in GetStats: {ex.Message}";
                if (ex.InnerException != null) errorMsg += $" Inner: {ex.InnerException.Message}";
                
                return StatusCode(500, new 
                { 
                    message = "Failed to load dashboard stats", 
                    error = ex.Message,
                    details = errorMsg,
                    stackTrace = ex.StackTrace 
                });
            }
        }
    }
}
