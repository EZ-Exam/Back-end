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
        /// M� h�a m?t kh?u ngu?i d�ng.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c.</param>
        /// <returns>M?t kh?u d� m� h�a.</returns>
        string EncryptPassword(string plainPassword);

        /// <summary>
        /// X�c minh m?t kh?u ngu?i d�ng so v?i m?t kh?u d� m� h�a.
        /// </summary>
        /// <param name="plainPassword">M?t kh?u g?c do ngu?i d�ng nh?p.</param>
        /// <param name="hashedPassword">M?t kh?u d� m� h�a trong co s? d? li?u.</param>
        /// <returns>True n?u m?t kh?u kh?p, ngu?c l?i False.</returns>
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
