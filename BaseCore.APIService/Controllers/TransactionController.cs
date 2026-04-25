using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET: api/Transaction
        [HttpGet]
        public async Task<IActionResult> GetUserTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var transactions = await _transactionService.GetUserTransactionsAsync(userId, page, pageSize);
                var totalCount = await _transactionService.GetTotalCountAsync(userId);

                var result = transactions.Select(t => new TransactionResponse
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description ?? "",
                    Status = t.Status.ToString(),
                    PaymentMethod = t.PaymentMethod ?? "N/A",
                    TransactionCode = "N/A",
                    GameAccountId = t.GameAccountId,
                    GameAccountName = t.GameAccount != null ? (t.GameAccount.AccountName ?? t.GameAccount.GameName ?? "N/A") : "N/A",
                    Created = t.Created,
                    CompletedAt = t.CompletedAt
                }).ToList();

                return Ok(new
                {
                    data = result,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)System.Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
        // GET: api/Transaction/all (Admin only)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync(page, pageSize);
                var totalCount = await _transactionService.GetTotalCountAllAsync();

                var result = transactions.Select(t => new TransactionResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserName = t.User != null ? (t.User.UserName ?? t.User.Name ?? "N/A") : "N/A",
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description ?? "",
                    Status = t.Status.ToString(),
                    PaymentMethod = t.PaymentMethod ?? "N/A",
                    TransactionCode = "N/A",
                    GameAccountId = t.GameAccountId,
                    GameAccountName = t.GameAccount != null ? (t.GameAccount.AccountName ?? t.GameAccount.GameName ?? "N/A") : "N/A",
                    Created = t.Created,
                    CompletedAt = t.CompletedAt
                }).ToList();

                return Ok(new
                {
                    data = result,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)System.Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


        // GET: api/Transaction/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var transaction = await _transactionService.GetByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                if (transaction.UserId != userId)
                {
                    return Forbid();
                }

                var result = new TransactionResponse
                {
                    Id = transaction.Id,
                    Type = transaction.Type.ToString(),
                    Amount = transaction.Amount,
                    Description = transaction.Description ?? "",
                    Status = transaction.Status.ToString(),
                    PaymentMethod = transaction.PaymentMethod ?? "None",
                    TransactionCode = "N/A",
                    GameAccountId = null,
                    GameAccountName = "N/A",
                    Created = transaction.Created,
                    CompletedAt = transaction.CompletedAt
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/Transaction/pending (Admin only)
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetPendingTransactionsAsync();

                var result = transactions.Select(t => new TransactionResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    UserName = t.User?.Name ?? "N/A",
                    Type = t.Type.ToString(),
                    Amount = t.Amount,
                    Description = t.Description ?? "",
                    Status = t.Status.ToString(),
                    PaymentMethod = t.PaymentMethod ?? "None",
                    TransactionCode = "N/A",
                    GameAccountId = null,
                    GameAccountName = "N/A",
                    Created = t.Created
                });

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // PUT: api/Transaction/{id}/status (Admin only)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTransactionStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request" });
                }

                var transaction = await _transactionService.GetByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(new { message = "Transaction not found" });
                }

                if (request.Status == TransactionStatus.Completed)
                {
                    transaction.Status = TransactionStatus.Completed;
                    transaction.CompletedAt = System.DateTime.Now;
                }
                else if (request.Status == TransactionStatus.Cancelled)
                {
                    transaction.Status = TransactionStatus.Cancelled;

                    // Refund for failed withdrawals
                    if (transaction.Type == TransactionType.Withdraw && transaction.Status == TransactionStatus.Pending)
                    {
                        // Note: Wallet service would need a refund method
                    }
                }
                else
                {
                    transaction.Status = request.Status;
                }

                await _transactionService.UpdateAsync(transaction);

                return Ok(new { message = "Transaction status updated successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update transaction status", error = ex.Message });
            }
        }
    }

    // DTOs
    public class TransactionResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
        public int? GameAccountId { get; set; }
        public string? GameAccountName { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime? CompletedAt { get; set; }
    }

    public class UpdateTransactionStatusRequest
    {
        public TransactionStatus Status { get; set; }
    }
}