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
        /// Encrypts plain password.
        /// </summary>
        /// <param name="plainPassword">Plain password.</param>
        /// <returns>Hashed password.</returns>
        public string EncryptPassword(string plainPassword)
        {
            return _passwordHasher.HashPassword(null, plainPassword);
        }

        /// <summary>
        /// Verifies plain password against hashed.
        /// </summary>
        /// <param name="plainPassword">Plain password.</param>
        /// <param name="hashedPassword">Hashed password.</param>
        /// <returns>True if match.</returns>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
