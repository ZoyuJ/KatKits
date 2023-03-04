namespace KatKits
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringUtil
    {
        public static string Take(this string Source, string From = null, string To = null)
        {
            if (From == null && To == null) return Source;
            var I1 = From == null ? -1 : Source.IndexOf(From);
            var I2 = To == null ? -1 : Source.IndexOf(To);
            if (I1 == -1 && I2 == -1) return "";
            if (I1 == -1 && I2 != -1) return Source.Substring(0, I2);
            if (I1 != -1 && I2 == -1) return Source.Substring(I1 + From.Length, Source.Length - I1 - From.Length);
            if (I1 > I2) return null;
            return Source.Substring(I1 + From.Length, I2 - I1 - From.Length);
        }
        public static string TakeWidth(this string Source, string From = null, string To = null)
        {
            if (From == null && To == null) return Source;
            var I1 = From == null ? -1 : Source.IndexOf(From);
            var I2 = To == null ? -1 : Source.IndexOf(To);
            if (I1 == -1 && I2 == -1) return "";
            if (I1 == -1 && I2 != -1) return Source.Substring(0, I2 + To.Length);
            if (I1 != -1 && I2 == -1) return Source.Substring(I1, Source.Length - I1);
            return Source.Substring(I1, I2 - I1 + To.Length);
        }
        /// <summary>
        /// take specified length substring from a specified part
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="From"></param>
        /// <param name="Length"></param>
        /// <returns>if from string is not found, return empty; null or empty, take form begin of source</returns>
        public static string Take(this string Source, string From, int Length)
        {
            if (string.IsNullOrEmpty(From)) return Source.Substring(0, Length);
            var I = Source.IndexOf(From);
            if (I == -1) return "";
            return Source.Substring(I + From.Length, Length);
        }
        public static string ReplaceCanEmpty(this string Source, string OldVal, string NewVal)
        {
            if (string.IsNullOrEmpty(OldVal)) return Source;
            return Source.Replace(OldVal, NewVal);
        }

        public static Regex WildCardsToRegex(string WildCardsPattern, bool IgnoreCase = true)
        {
            return new Regex($"^{Regex.Escape(WildCardsPattern).Replace("\\*", ".*").Replace("\\?", ".")}$", IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
    }
}
