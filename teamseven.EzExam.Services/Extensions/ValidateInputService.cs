using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace teamseven.EzExam.Services.Extensions
{
    internal static class ValidateInputService
    {
        /// <summary>
        /// Ki?m tra xem chu?i c� r?ng hay kh�ng.
        /// </summary>
        /// <param name="input">Chu?i c?n ki?m tra.</param>
        /// <returns>True n?u chu?i kh�ng r?ng, ngu?c l?i False.</returns>
        public static bool IsNotEmpty(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Ki?m tra xem s? nguy�n c� l?n hon m?t gi� tr? nh?t d?nh hay kh�ng.
        /// </summary>
        /// <param name="number">S? nguy�n c?n ki?m tra.</param>
        /// <param name="compareValue">Gi� tr? so s�nh.</param>
        /// <returns>True n?u s? nguy�n l?n hon gi� tr? so s�nh, ngu?c l?i False.</returns>
        public static bool IsGreaterThan(int number, int compareValue)
        {
            return number > compareValue;
        }

        /// <summary>
        /// Ki?m tra xem chu?i c� ph?i l� email h?p l? hay kh�ng.
        /// </summary>
        /// <param name="email">Chu?i email c?n ki?m tra.</param>
        /// <returns>True n?u l� email h?p l?, ngu?c l?i False.</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Bi?u th?c ch�nh quy d? ki?m tra d?nh d?ng email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Ki?m tra xem chu?i c� ph?i l� s? di?n tho?i h?p l? hay kh�ng.
        /// </summary>
        /// <param name="phoneNumber">Chu?i s? di?n tho?i c?n ki?m tra.</param>
        /// <returns>True n?u l� s? di?n tho?i h?p l?, ngu?c l?i False.</returns>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Bi?u th?c ch�nh quy d? ki?m tra d?nh d?ng s? di?n tho?i (10-12 ch? s?)
            string phonePattern = @"^\d{10,12}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }


        /// <summary>
        /// Ki?m tra c�c thu?c t�nh c�ng khai c?a m?t object v� tr? v? danh s�ch t�n c�c thu?c t�nh c� gi� tr? null.
        /// </summary>
        /// <param name="obj">Object c?n ki?m tra.</param>
        /// <returns>Danh s�ch t�n c�c thu?c t�nh c� gi� tr? null. N?u kh�ng c� thu?c t�nh n�o null, tr? v? danh s�ch r?ng.</returns>
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
        /// Ki?m tra xem t?t c? thu?c t�nh c�ng khai c?a m?t object c� null hay kh�ng.
        /// </summary>
        /// <param name="obj">Object c?n ki?m tra.</param>
        /// <returns>True n?u t?t c? thu?c t�nh kh�ng null, False n?u c� �t nh?t m?t thu?c t�nh null.</returns>
        public static bool ArePropertiesNotNull(object obj)
        {
            if (obj == null)
                return false;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.All(prop => prop.GetValue(obj) != null);
        }
    }
}
