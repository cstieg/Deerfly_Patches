using System;
using System.Text.RegularExpressions;

namespace Deerfly_Patches.Modules
{
    public static class StringHelper
    {
        /// <summary>
        /// Checks whether the string matches the wildcard pattern
        /// </summary>
        /// <param name="s">The present string object</param>
        /// <param name="pattern">A wildcard pattern possible containing wildcards '*' and/or '?'</param>
        /// <returns>True if a match, false if not</returns>
        public static bool Matches(this String s, string pattern)
        {
            string regExPattern = "^" + Regex.Escape(s).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// Checks whether the first character of the string is a digit (0-9)
        /// </summary>
        /// <param name="s">The present string object</param>
        /// <returns>True if the first character is a digit, false if not</returns>
        public static bool IsDigit(this String s)
        {
            if (s == "")
            {
                return false;
            }
            return IsDigit(s.ToCharArray()[0]);
        }

        /// <summary>
        /// Checks whether a char is a digit (0-9)
        /// </summary>
        /// <param name="c">The char to check</param>
        /// <returns>True if the char is a digit, false if not</returns>
        public static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Returns a string of all the digits (0-9) in a given string
        /// </summary>
        /// <param name="s">The present string object</param>
        /// <returns>A string of all the digits</returns>
        public static string Digits(this String s)
        {
            char[] chars = s.ToCharArray();
            string digits = "";
            for (int i = 0; i < chars.Length; i++)
            {
                if (IsDigit(chars[i]))
                {
                    digits += chars[i];
                }
            }
            return digits;
        }
    }
}