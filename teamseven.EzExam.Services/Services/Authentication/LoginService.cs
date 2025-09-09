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

        public LoginService()
        {
        }

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
            // L?y user t? repository
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return (false, "Wrong email or password");
            }

            // So s�nh m?t kh?u hash
            if (!_passwordEncryptionService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return (false, "Invalid password");
            }

            // Ki?m tra t�i kho?n c� b? v� hi?u h�a kh�ng
            if (!user.IsActive)
            {
                return (false, "This account is disabled");
            }


            //set last login
            await _userRepository.SetLoginDateTime(user.Id);


            // T?o v� tr? v? token
            var token = _authService.GenerateJwtToken(user);

            return (true, token);
        }


        //private void ValidateInput(string email, string password)
        //{
        //    if (string.IsNullOrEmpty(email))
        //        throw new ArgumentException("Email is required.", nameof(email));
        //    if (string.IsNullOrEmpty(password))
        //        throw new ArgumentException("Password is required.", nameof(password));
        //    if (!IsValidEmail(email))
        //        throw new ArgumentException("Invalid email format.", nameof(email));
        //}

        //private async Task<User> ValidateUserExistence(string email)
        //{
        //    User user = await _userRepository.GetByEmailAsync(email);
        //    if (user == null)
        //        throw new KeyNotFoundException("User not found.");
        //    return user;
        //}

        //private void ValidatePassword(string password, string hashedPassword)
        //{
        //    if (!_passwordEncryptionService.VerifyPassword(password, hashedPassword))
        //        throw new UnauthorizedAccessException("Invalid password.");
        //}

        //private bool IsValidEmail(string email)
        //{
        //    // Regex don gi?n d? ki?m tra d?nh d?ng email
        //    string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        //    return Regex.IsMatch(email, pattern);
        //}
    }
}
