using Microsoft.Extensions.Configuration;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Object.Requests;

namespace teamseven.EzExam.Services.Services.Authentication
{
    public class RegisterService : IRegisterService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordEncryptionService _passwordEncryptionService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RegisterService(
            UserRepository userRepository,
            IPasswordEncryptionService passwordEncryptionService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<(bool IsSuccess, string ResultOrError)> RegisterUserAsync(RegisterRequest request)
        {
            // Check for duplicate email
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return (false, "Email already in use");
            }

            // Create new user
            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordEncryptionService.EncryptPassword(request.Password),
                FullName = request.Name,
                CreatedAt = DateTime.UtcNow,
                RoleId = 1,
                IsActive = true,
                AvatarUrl = null,
                PhoneNumber = null,
                EmailVerifiedAt = null,
                LastLoginAt = null,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = null
            };

            // Save to database
            await _userRepository.AddUserAsync(user);

            return (true, "User registered successfully");
        }

        public async Task<(bool IsSuccess, string ResultOrError)> ChangeUserRoleAsync(int userId, string roleName, string providedSecretKey)
        {
            // Check secret key
            var expectedSecretKey = _configuration["Security:SuperSecretKey"]
                                    ?? Environment.GetEnvironmentVariable("SUPER_SECRET_KEY");
            if (string.IsNullOrEmpty(expectedSecretKey) || providedSecretKey != expectedSecretKey)
            {
                return (false, "Invalid secret key");
            }

            return await _userRepository.ChangeUserRoleAsync(userId, roleName);
        }

        private void SendWelcomeMail(User user)
        {
            string subject = "Welcome to EzExam";
            string body = $"Hello {user.Email},\n\nWelcome to EzExam platform!";
            _emailService.SendEmail(user.Email, subject, body);
        }
    }
}