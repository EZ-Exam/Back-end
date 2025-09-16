using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace teamseven.EzExam.Services.Services.JwtHelperService
{
    public class JwtHelperService : IJwtHelperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtHelperService> _logger;

        public JwtHelperService(IConfiguration configuration, ILogger<JwtHelperService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public int? GetCurrentUserIdFromToken(string authHeader)
        {
            try
            {
                var token = ExtractTokenFromHeader(authHeader);
                if (string.IsNullOrEmpty(token))
                    return null;

                var principal = ValidateToken(token);
                if (principal == null)
                    return null;

                var userIdClaim = principal.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("User ID claim not found or invalid in token.");
                    return null;
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from token: {Message}", ex.Message);
                return null;
            }
        }

        public string? GetCurrentUserEmailFromToken(string authHeader)
        {
            try
            {
                var token = ExtractTokenFromHeader(authHeader);
                if (string.IsNullOrEmpty(token))
                    return null;

                var principal = ValidateToken(token);
                if (principal == null)
                    return null;

                return principal.FindFirst("email")?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user email from token: {Message}", ex.Message);
                return null;
            }
        }

        public int? GetCurrentUserRoleIdFromToken(string authHeader)
        {
            try
            {
                var token = ExtractTokenFromHeader(authHeader);
                if (string.IsNullOrEmpty(token))
                    return null;

                var principal = ValidateToken(token);
                if (principal == null)
                    return null;

                var roleIdClaim = principal.FindFirst("roleId")?.Value;
                if (string.IsNullOrEmpty(roleIdClaim) || !int.TryParse(roleIdClaim, out int roleId))
                {
                    _logger.LogWarning("Role ID claim not found or invalid in token.");
                    return null;
                }

                return roleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting role ID from token: {Message}", ex.Message);
                return null;
            }
        }

        public bool IsTokenValid(string token)
        {
            try
            {
                return ValidateToken(token) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token: {Message}", ex.Message);
                return false;
            }
        }

        private string? ExtractTokenFromHeader(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Authorization header is missing or invalid format.");
                return null;
            }

            var tokenString = authHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(tokenString))
            {
                _logger.LogWarning("Token string is empty.");
                return null;
            }

            return tokenString;
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                // Get JWT configuration
                var jwtKey = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(jwtKey))
                {
                    _logger.LogError("JWT key is not configured.");
                    return null;
                }

                // Validate and decode token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT token has expired.");
                return null;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                _logger.LogWarning("JWT token has invalid signature.");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while validating token: {Message}", ex.Message);
                return null;
            }
        }
    }
}
