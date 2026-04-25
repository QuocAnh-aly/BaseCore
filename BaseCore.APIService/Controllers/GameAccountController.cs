using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameAccountController : ControllerBase
    {
        private readonly IGameAccountService _gameAccountService;

        public GameAccountController(IGameAccountService gameAccountService)
        {
            _gameAccountService = gameAccountService;
        }

        // GET: api/GameAccount
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableAccounts([FromQuery] string? gameName = null)
        {
            try
            {
                IEnumerable<GameAccount> accounts;
                if (!string.IsNullOrEmpty(gameName))
                {
                    accounts = await _gameAccountService.GetAccountsByGameAsync(gameName);
                }
                else
                {
                    accounts = await _gameAccountService.GetAvailableAccountsAsync();
                }

                var result = accounts.Select(a => new GameAccountResponse
                {
                    Id = a.Id,
                    GameName = a.GameName,
                    AccountName = a.AccountName,
                    Description = a.Description,
                    Price = a.Price,
                    ImageUrl = a.ImageUrl,
                    IsSold = a.IsSold,
                    SellerId = a.SellerId,
                    SellerName = a.Seller?.Name,
                    Created = a.CreatedDateTime
                });

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/GameAccount/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAccounts()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var accounts = await _gameAccountService.GetAccountsBySellerAsync(userId);

                var result = accounts.Select(a => new GameAccountResponse
                {
                    Id = a.Id,
                    GameName = a.GameName,
                    AccountName = a.AccountName,
                    Description = a.Description,
                    Price = a.Price,
                    ImageUrl = a.ImageUrl,
                    IsSold = a.IsSold,
                    SellerId = a.SellerId,
                    SellerName = a.Seller?.Name,
                    BuyerId = a.BuyerId,
                    BuyerName = a.Buyer?.Name,
                    SoldAt = a.SoldAt,
                    Created = a.CreatedDateTime
                });

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/GameAccount/my/purchases
        [HttpGet("my/purchases")]
        public async Task<IActionResult> GetMyPurchases()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var accounts = await _gameAccountService.GetAccountsByBuyerAsync(userId);
                var result = accounts.Select(a => new GameAccountResponse
                {
                    Id = a.Id,
                    GameName = a.GameName,
                    AccountName = a.AccountName,
                    Password = a.AccountPassword, // Buyer needs to see password
                    Description = a.Description,
                    Price = a.Price,
                    ImageUrl = a.ImageUrl,
                    IsSold = a.IsSold,
                    SellerId = a.SellerId,
                    SellerName = a.Seller?.Name,
                    BuyerId = a.BuyerId,
                    BuyerName = a.Buyer?.Name,
                    SoldAt = a.SoldAt,
                    Created = a.CreatedDateTime
                });

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/GameAccount/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var account = await _gameAccountService.GetByIdAsync(id);
                if (account == null)
                {
                    return NotFound(new { message = "Game account not found" });
                }

                var result = new GameAccountResponse
                {
                    Id = account.Id,
                    GameName = account.GameName,
                    AccountName = account.AccountName,
                    Password = account.AccountPassword, // Only show password to seller/buyer
                    Description = account.Description,
                    Price = account.Price,
                    ImageUrl = account.ImageUrl,
                    IsSold = account.IsSold,
                    SellerId = account.SellerId,
                    SellerName = account.Seller?.Name,
                    BuyerId = account.BuyerId,
                    BuyerName = account.Buyer?.Name,
                    SoldAt = account.SoldAt,
                    Created = account.CreatedDateTime
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/GameAccount
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameAccountRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "admin";

                var gameAccount = new GameAccount
                {
                    GameName = request.GameName ?? "",
                    AccountName = request.AccountName ?? "",
                    AccountPassword = request.Password ?? "",
                    Description = request.Description ?? "",
                    Price = request.Price,
                    ImageUrl = request.ImageUrl ?? "",
                    SellerId = userId,
                    CreatedUser = userName
                };

                var createdAccount = await _gameAccountService.CreateAsync(gameAccount);

                return CreatedAtAction(nameof(GetById), new { id = createdAccount.Id },
                    new GameAccountResponse
                    {
                        Id = createdAccount.Id,
                        GameName = createdAccount.GameName,
                        AccountName = createdAccount.AccountName,
                        Description = createdAccount.Description,
                        Price = createdAccount.Price,
                        ImageUrl = createdAccount.ImageUrl,
                        SellerId = createdAccount.SellerId,
                        Created = createdAccount.CreatedDateTime
                    });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create game account", error = ex.Message });
            }
        }

        // PUT: api/GameAccount/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGameAccountRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Invalid request" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var existingAccount = await _gameAccountService.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return NotFound(new { message = "Game account not found" });
                }

                // Admin có thể sửa bất kỳ acc nào, User chỉ sửa acc của mình
                bool isAdmin = User.IsInRole("Admin");
                if (!isAdmin && existingAccount.SellerId != userId)
                {
                    return Forbid();
                }

                existingAccount.GameName = request.GameName ?? existingAccount.GameName;
                existingAccount.AccountName = request.AccountName ?? existingAccount.AccountName;
                existingAccount.AccountPassword = request.Password ?? existingAccount.AccountPassword;
                existingAccount.Description = request.Description ?? existingAccount.Description;
                existingAccount.Price = request.Price ?? existingAccount.Price;
                existingAccount.ImageUrl = request.ImageUrl ?? existingAccount.ImageUrl;

                await _gameAccountService.UpdateAsync(existingAccount);

                return Ok(new { message = "Game account updated successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update game account", error = ex.Message });
            }
        }

        // DELETE: api/GameAccount/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var existingAccount = await _gameAccountService.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return NotFound(new { message = "Game account not found" });
                }

                // Admin có thể xóa bất kỳ acc nào, User chỉ xóa acc của mình
                bool isAdminDel = User.IsInRole("Admin");
                if (!isAdminDel && existingAccount.SellerId != userId)
                {
                    return Forbid();
                }

                await _gameAccountService.DeleteAsync(id);

                return Ok(new { message = "Game account deleted successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete game account", error = ex.Message });
            }
        }

        // POST: api/GameAccount/{id}/purchase
        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> Purchase(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = await _gameAccountService.PurchaseAccountAsync(id, userId);

                if (!success)
                {
                    return BadRequest(new { message = "Purchase failed. Account may be sold or insufficient balance." });
                }

                return Ok(new { message = "Account purchased successfully" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Purchase failed", error = ex.Message });
            }
        }
    }

    // DTOs
    public class GameAccountResponse
    {
        public int Id { get; set; }
        public string? GameName { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsSold { get; set; }
        public int? SellerId { get; set; }
        public string? SellerName { get; set; }
        public int? BuyerId { get; set; }
        public string? BuyerName { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime? SoldAt { get; set; }
    }

    public class CreateGameAccountRequest
    {
        public string? GameName { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateGameAccountRequest
    {
        public string? GameName { get; set; }
        public string? AccountName { get; set; }
        public string? Password { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}