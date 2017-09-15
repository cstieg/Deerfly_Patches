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
    }
}