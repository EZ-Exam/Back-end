using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace teamseven.EzExam.Services.Helpers
{
    public static class IdHelper
    {
        public static string EncodeId(int id)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(id));
        }

        public static int DecodeId(string encodedId)
        {
            try
            {
                var bytes = Convert.FromBase64String(encodedId);
                return BitConverter.ToInt32(bytes, 0);
            }
            catch
            {
                throw new ArgumentException("Invalid encoded ID format.");
            }
        }
    }
}
