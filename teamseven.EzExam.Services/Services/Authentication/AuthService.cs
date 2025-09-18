using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Interfaces;

namespace teamseven.EzExam.Services.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jsonToken == null)
            {
                throw new SecurityTokenException("Invalid token");
            }

            var identity = new ClaimsIdentity(jsonToken?.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

        public bool IsUserInRole(string authHeader, string role)
        {
            try
            {
                var tokenString = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);
                var roleClaim = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value?.ToLower();

                return roleClaim == role.ToLower();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsUserInPlan(string token, string plan)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                var planClaim = principal?.FindFirst("AccountType");

                if (planClaim != null && planClaim.Value == plan)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateJwtToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null when generating token.");

            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing in configuration.");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("fullname", user.FullName ?? string.Empty),
                new Claim("email", user.Email),
                new Claim("roleId", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GoogleLoginAsync(string idToken)
        {
            try
            {
                // Authenticate token with Google
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // Check UserSocialProvider
                var socialProvider = await _unitOfWork.UserSocialProviderRepository.GetByProviderAsync("Google", payload.Subject);
                User user;

                if (socialProvider == null)
                {
                    // Check user by email
                    user = await _unitOfWork.UserRepository.GetByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        // Create new user
                        user = new User
                        {
                            Email = payload.Email,
                            FullName = payload.Name,
                            AvatarUrl = payload.Picture,
                            RoleId = 1, // Default RoleId
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.UserRepository.AddUserAsync(user);
                    }

                    // Create UserSocialProvider
                    var userSocialProvider = new UserSocialProvider
                    {
                        UserId = user.Id,
                        ProviderName = "Google",
                        ProviderId = payload.Subject,
                        Email = payload.Email,
                        ProfileUrl = payload.Picture,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.UserSocialProviderRepository.AddAsync(userSocialProvider);

                    // Save changes with transaction
                    await _unitOfWork.SaveChangesWithTransactionAsync();
                }
                else
                {
                    // Existing user
                    user = await _unitOfWork.UserRepository.GetByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        throw new Exception("User not found.");
                    }
                    user.LastLoginAt = DateTime.UtcNow;
                    await _unitOfWork.UserRepository.UpdateAsync(user);
                    await _unitOfWork.SaveChangesWithTransactionAsync();
                }

                // Generate JWT
                return GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Google authentication error.", ex);
            }
        }
    }
}