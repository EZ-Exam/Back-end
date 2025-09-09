using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace teamseven.EzExam.Services.Extensions
{
    internal static class ValidateInputService
    {
        /// <summary>
        /// Ki?m tra xem chu?i có r?ng hay không.
        /// </summary>
        /// <param name="input">Chu?i c?n ki?m tra.</param>
        /// <returns>True n?u chu?i không r?ng, ngu?c l?i False.</returns>
        public static bool IsNotEmpty(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Ki?m tra xem s? nguyên có l?n hon m?t giá tr? nh?t d?nh hay không.
        /// </summary>
        /// <param name="number">S? nguyên c?n ki?m tra.</param>
        /// <param name="compareValue">Giá tr? so sánh.</param>
        /// <returns>True n?u s? nguyên l?n hon giá tr? so sánh, ngu?c l?i False.</returns>
        public static bool IsGreaterThan(int number, int compareValue)
        {
            return number > compareValue;
        }

        /// <summary>
        /// Ki?m tra xem chu?i có ph?i là email h?p l? hay không.
        /// </summary>
        /// <param name="email">Chu?i email c?n ki?m tra.</param>
        /// <returns>True n?u là email h?p l?, ngu?c l?i False.</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Bi?u th?c chính quy d? ki?m tra d?nh d?ng email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Ki?m tra xem chu?i có ph?i là s? di?n tho?i h?p l? hay không.
        /// </summary>
        /// <param name="phoneNumber">Chu?i s? di?n tho?i c?n ki?m tra.</param>
        /// <returns>True n?u là s? di?n tho?i h?p l?, ngu?c l?i False.</returns>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Bi?u th?c chính quy d? ki?m tra d?nh d?ng s? di?n tho?i (10-12 ch? s?)
            string phonePattern = @"^\d{10,12}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }


        /// <summary>
        /// Ki?m tra các thu?c tính công khai c?a m?t object và tr? v? danh sách tên các thu?c tính có giá tr? null.
        /// </summary>
        /// <param name="obj">Object c?n ki?m tra.</param>
        /// <returns>Danh sách tên các thu?c tính có giá tr? null. N?u không có thu?c tính nào null, tr? v? danh sách r?ng.</returns>
        public static List<string> GetNullProperties(object obj)
        {
            if (obj == null)
                return new List<string> { "Object is null" };

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties
                .Where(prop => prop.GetValue(obj) == null)
                .Select(prop => prop.Name)
                .ToList();
        }

        /// <summary>
        /// Ki?m tra xem t?t c? thu?c tính công khai c?a m?t object có null hay không.
        /// </summary>
        /// <param name="obj">Object c?n ki?m tra.</param>
        /// <returns>True n?u t?t c? thu?c tính không null, False n?u có ít nh?t m?t thu?c tính null.</returns>
        public static bool ArePropertiesNotNull(object obj)
        {
            if (obj == null)
                return false;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.All(prop => prop.GetValue(obj) != null);
        }
    }
}
