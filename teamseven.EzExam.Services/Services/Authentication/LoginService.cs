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
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return (false, "Wrong email or password");
            }

            if (!_passwordEncryptionService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return (false, "Invalid password");
            }

            if (!user.IsActive)
            {
                return (false, "This account is disabled");
            }

            await _userRepository.SetLoginDateTime(user.Id);

            var token = await _authService.GenerateJwtTokenWithSubscriptionAsync(user);

            return (true, token);
        }
    }
}