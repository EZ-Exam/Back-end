using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Services.Interfaces;

namespace teamseven.EzExam.Services.Services.Authentication
{
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordEncryptionService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        /// <summary>
        /// M� h�a m?t kh?u ngu?i d�ng b?ng thu?t to�n ASP.NET Core Identity.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c.</param>
        /// <returns>M?t kh?u d� du?c m� h�a.</returns>
        public string EncryptPassword(string plainPassword)
        {
            return _passwordHasher.HashPassword(null, plainPassword);
        }

        /// <summary>
        /// X�c minh m?t kh?u ngu?i d�ng.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c do ngu?i d�ng nh?p.</param>
        /// <param name="hashedPassword">M?t kh?u d� m� h�a du?c luu trong co s? d? li?u.</param>
        /// <returns>True n?u m?t kh?u kh?p, ngu?c l?i l� false.</returns>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
