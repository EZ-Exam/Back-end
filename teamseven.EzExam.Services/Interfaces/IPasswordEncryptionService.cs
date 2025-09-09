using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Interfaces
{
    public interface IPasswordEncryptionService
    {
        /// <summary>
        /// Mã hóa m?t kh?u ngu?i dùng.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c.</param>
        /// <returns>M?t kh?u dã mã hóa.</returns>
        string EncryptPassword(string plainPassword);

        /// <summary>
        /// Xác minh m?t kh?u ngu?i dùng so v?i m?t kh?u dã mã hóa.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c do ngu?i dùng nh?p.</param>
        /// <param name="hashedPassword">M?t kh?u dã mã hóa trong co s? d? li?u.</param>
        /// <returns>True n?u m?t kh?u kh?p, ngu?c l?i False.</returns>
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
