using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetLife.Dto;
using PetLife.Dto.ErrorCodes;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;
using Stripe;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PetLife.Serivce
{
    public class UserLoginService : Controller, ICustomerLogin
    {
        private readonly PetLifeDBContext context;
        private readonly IConfiguration config;
        private readonly UserErrors errors;
        private readonly IHttpContextAccessor httpContext;

        public UserLoginService(PetLifeDBContext _context, IConfiguration _config, UserErrors _errors, IHttpContextAccessor _httpContext)
        {
            context = _context;
            config = _config;
            errors = _errors;
            httpContext = _httpContext;
        }

        public async Task<IActionResult> LoginCustomer(LoginDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == dto.UserName);
            if (user == null)
            {
                return new UnauthorizedObjectResult(new { Message = errors.InvalidUserCredentials });
            }
            if (user.PasswordHash != HashPassword(dto.Password))
            {
                return new UnauthorizedObjectResult(new { Message = errors.InvalidUserCredentials });
            }
            //Generate JWT token
            var token = GenerateJWTToken(user);
            var refreshingToken = await CreateAndSaveRefreshToken(user.UserId);
            setRefreshTokenCookie(refreshingToken);
            return Ok(new { 
                Message = "Loggin Successfull...!", 
                Token = token,
                Role = user.Role,
                UserIdentifier = user.UserId,
                UserName = user.UserName,
                RefreshingToken = refreshingToken
            });
        }
        
        public async Task<IActionResult> LogoutCustomer()
        {
            // Use IHttpContextAccessor to access the current user principal since this is a service
            var userPrincipal = httpContext.HttpContext?.User;
            if (userPrincipal == null)
            {
                return BadRequest("User not found in token");
            }

            // Try to read standard NameIdentifier claim first, fall back to custom "userId"
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier) ?? userPrincipal.FindFirst("userId");
            if (userIdClaim == null)
            {
                return BadRequest("User not found in token");
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userIdGuid))
            {
                return BadRequest("Invalid user id in token");
            }

            // Invalidate all refresh tokens for this user
            var tokens = await context.RefreshTokens.Where(rt => rt.UserId == userIdGuid && rt.Token != null && rt.Revoked==null).ToListAsync();
            if (tokens.Any())
            {
                foreach (var token in tokens)
                {
                    token.Token = token.Token;
                    token.Expires = DateTime.UtcNow;
                    token.Revoked = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
            }

            // Also remove cookie on response if available
            var httpResponse = httpContext.HttpContext?.Response;
            if (httpResponse != null)
            {
                httpResponse.Cookies.Delete("refreshToken");
            }

            return Ok(new { message = "Logged out successfully" });
        }

        private string HashPassword(string password)
        {
            //Implementing the security of password using SHA256 hashing algorithm
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder(hashedBytes.Length * 2);
            foreach (var b in hashedBytes)
                builder.AppendFormat("{0:x2}", b);
            return builder.ToString();
        }

        private string GenerateJWTToken(User user)
        {
            var claims = new List<Claim>
           {
                // Add standard claims for interoperability
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),

                // preserve existing custom claims (optional)
                new Claim("userId", user.UserId.ToString()),
                new Claim("userName", user.UserName),
                new Claim("role", user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public void setRefreshTokenCookie(string token)
        {
            var httpContextAccessor = httpContext.HttpContext;
            if (httpContextAccessor == null) return;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                // For cross-origin requests from the frontend (different origin), the cookie must use SameSite=None
                // and the client must include credentials when making requests.
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(1),
                Path = "/"
            };
            httpContextAccessor.Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        public async Task<TokenResponse?> RotateTokens(string? oldToken)
        {
            if (string.IsNullOrEmpty(oldToken)) return null;

            //find token in database
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == oldToken);

            //Validate token: must exist, not be expired, and not be revoked
            if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow || refreshToken.Revoked != null)
                return null;

            //Revoke the old token
            refreshToken.Revoked = DateTime.UtcNow;

            //Generate new Tokens
            var user = await context.Users.FindAsync(refreshToken.UserId);
            if (user == null) return null;

            var newAccessToken = GenerateJWTToken(user);
            var newRefreshTokenStr = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            //save new refresh token to database
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenStr,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                UserId = user.UserId
            };

            context.RefreshTokens.Add(newRefreshToken);
            await context.SaveChangesAsync();

            return new TokenResponse(newAccessToken, newRefreshTokenStr);
        }
        public async Task<string> CreateAndSaveRefreshToken(Guid userId)
        {
            // Create a secure random token
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var tokenString = Convert.ToBase64String(randomNumber);

            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(7), // Token valid for 1 week
                Created = DateTime.UtcNow
            };

            // Save to your Entity Framework Database Context
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();

            return tokenString;
        }
    }
}
