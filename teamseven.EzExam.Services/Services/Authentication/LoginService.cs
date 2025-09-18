using Microsoft.AspNetCore.Identity.Data;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.Authentication
{
    public class LoginService : ILoginService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordEncryptionService _passwordEncryptionService;
        private readonly IAuthService _authService;

        public LoginService(
            UserRepository userRepository,
            IPasswordEncryptionService passwordEncryptionService,
            IAuthService authService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<(bool IsSuccess, string ResultOrError)> ValidateUserAsync(teamseven.EzExam.Services.Object.Requests.LoginRequest loginRequest)
        {
            // Get user from repository
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return (false, "Wrong email or password");
            }

            // Compare password hash
            if (!_passwordEncryptionService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return (false, "Invalid password");
            }

            // Check if account is disabled
            if (!user.IsActive)
            {
                return (false, "This account is disabled");
            }

            // Set last login
            await _userRepository.SetLoginDateTime(user.Id);

            // Generate and return token
            var token = _authService.GenerateJwtToken(user);

            return (true, token);
        }
    }
}