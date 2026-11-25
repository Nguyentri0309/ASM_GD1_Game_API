using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameApi_ASM_NguyenTriThinh.Data;
using GameApi_ASM_NguyenTriThinh.DTOs;
using GameApi_ASM_NguyenTriThinh.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameApi_ASM_NguyenTriThinh.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Dang ky: kiem tra username trung, bam mat khau roi luu vao DB
        public async Task<(bool Success, string Message, AccountResponse? Account)> RegisterAsync(RegisterRequest request)
        {
            bool exists = await _context.Accounts.AnyAsync(a => a.Username == request.Username);
            if (exists)
            {
                return (false, "Username da ton tai", null);
            }

            var account = new Account
            {
                Username = request.Username,
                CreatedAt = DateTime.UtcNow
            };
            account.PasswordHash = _passwordHasher.HashPassword(account, request.Password);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var response = new AccountResponse { Id = account.Id, Username = account.Username };
            return (true, string.Empty, response);
        }

        // Dang nhap: sai username hoac mat khau deu tra null de controller tra 401
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Username == request.Username);
            if (account == null)
            {
                return null;
            }

            var verify = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return new LoginResponse
            {
                Token = CreateJwtToken(account),
                Username = account.Username
            };
        }

        // Tao JWT: NameIdentifier = Id, Name = Username, han 1 ngay, HMAC SHA-256
        private string CreateJwtToken(Account account)
        {
            string key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Thieu cau hinh Jwt:Key");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
