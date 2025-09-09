using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace teamseven.EzExam.Repository
{
  

    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var regex = new Regex("[^a-zA-Z0-9 ]");
            return regex.Replace(normalized, "");
        }
    }
}
