using System;
using System.Text;

namespace teamseven.EzExam.Services.Extensions
{
    public class IdObfuscator : IIdObfuscator
    {
        private const string Salt = "EzExam2025";

        public string EncodeId(int id)
        {
            string plainText = $"{Salt}{id}";
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            string base64 = Convert.ToBase64String(bytes);
            char[] charArray = base64.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public int DecodeId(string encodedId)
        {
            if (string.IsNullOrEmpty(encodedId))
                throw new ArgumentException("Encoded ID cannot be null or empty");

            try
            {
                char[] charArray = encodedId.ToCharArray();
                Array.Reverse(charArray);
                string base64 = new string(charArray);
                byte[] bytes = Convert.FromBase64String(base64);
                string decoded = Encoding.UTF8.GetString(bytes);

                if (!decoded.StartsWith(Salt))
                    throw new ArgumentException("Invalid encoded ID format");

                string idString = decoded.Substring(Salt.Length);
                return int.Parse(idString);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to decode ID", ex);
            }
        }
    }

    public interface IIdObfuscator
    {
        string EncodeId(int id);
        int DecodeId(string encodedId);
    }
}
