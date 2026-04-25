using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Services;
using BaseCore.Repository.EFCore;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        // GET: api/Wallet
        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var wallet = await _walletService.GetWalletByUserIdAsync(userId);

                if (wallet == null)
                {
                    // Create wallet if not exists
                    wallet = await _walletService.CreateWalletAsync(userId);
                }

                return Ok(new WalletResponse
                {
                    UserId = wallet.UserId,
                    Balance = wallet.Balance,
                    Updated = wallet.UpdatedDateTime
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/Wallet/deposit
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0)
                {
                    return BadRequest(new { message = "Invalid deposit amount" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = await _walletService.DepositAsync(userId, request.Amount, request.PaymentMethod ?? "N/A", request.TransactionCode ?? "N/A");

                if (!success)
                {
                    return BadRequest(new { message = "Deposit failed" });
                }

                return Ok(new { message = "Deposit successful" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Deposit failed", error = ex.Message });
            }
        }

        // POST: api/Wallet/withdraw
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0)
                {
                    return BadRequest(new { message = "Invalid withdraw amount" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = await _walletService.WithdrawAsync(userId, request.Amount, request.PaymentMethod ?? "N/A");

                if (!success)
                {
                    return BadRequest(new { message = "Withdraw failed. Insufficient balance or invalid amount." });
                }

                return Ok(new { message = "Withdraw request submitted successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Withdraw failed", error = ex.Message });
            }
        }

        // POST: api/Wallet/update-balance (Admin only)
        [HttpPost("update-balance")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request" });
                }

                var success = await _walletService.UpdateBalanceAsync(request.UserId, request.Amount);

                if (!success)
                {
                    return BadRequest(new { message = "Failed to update balance. User may not exist." });
                }

                return Ok(new { message = "Balance updated successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Update balance failed", error = ex.Message });
            }
        }

    }

    // DTOs
    public class WalletResponse
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
    }

    public class DepositRequest
    {
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
    }

    public class WithdrawRequest
    {
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class UpdateBalanceRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}