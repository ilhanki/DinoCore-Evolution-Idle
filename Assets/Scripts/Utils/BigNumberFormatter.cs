using System;

namespace DinoCore.Utils
{
    /// <summary>
    /// Handles formatting and arithmetic for very large numbers.
    /// Uses double internally (supports up to ~1e308).
    /// For display: K, M, B, T, then aa, ab, ac … az, ba, bb …
    /// </summary>
    public static class BigNumberFormatter
    {
        private static readonly string[] ShortSuffixes = { "", "K", "M", "B", "T" };

        /// <summary>
        /// Formats a double into a human-readable short string.
        /// Examples: 999 → "999", 1500 → "1.50K", 2.3e15 → "2.30aa"
        /// </summary>
        public static string Format(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "0";

            bool negative = value < 0;
            if (negative) value = -value;

            if (value < 1000)
            {
                string result = value < 10 ? value.ToString("F1") : value.ToString("F0");
                return negative ? "-" + result : result;
            }

            // Determine tier (each tier = ×1000)
            int tier = (int)Math.Floor(Math.Log10(value) / 3);

            string suffix;
            if (tier < ShortSuffixes.Length)
            {
                suffix = ShortSuffixes[tier];
            }
            else
            {
                // After T (tier 4), use aa=5, ab=6 … az=30, ba=31 …
                int extraTier = tier - ShortSuffixes.Length; // 0-based
                int first = extraTier / 26;  // 0→a, 1→b …
                int second = extraTier % 26; // 0→a, 1→b …
                suffix = ((char)('a' + first)).ToString() + ((char)('a' + second));
            }

            double scaled = value / Math.Pow(1000, tier);
            string formatted = scaled.ToString("F2") + suffix;
            return negative ? "-" + formatted : formatted;
        }

        /// <summary>
        /// Attempts to parse a formatted string back to double. 
        /// Useful for debugging / editor tools.
        /// </summary>
        public static bool TryParse(string text, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;

            text = text.Trim();

            // Try plain parse first
            if (double.TryParse(text, out value)) return true;

            // Check known suffixes
            for (int i = ShortSuffixes.Length - 1; i >= 1; i--)
            {
                if (text.EndsWith(ShortSuffixes[i], StringComparison.OrdinalIgnoreCase))
                {
                    string numPart = text.Substring(0, text.Length - ShortSuffixes[i].Length);
                    if (double.TryParse(numPart, out double num))
                    {
                        value = num * Math.Pow(1000, i);
                        return true;
                    }
                }
            }

            // Check aa-style suffix (2-char lowercase at end)
            if (text.Length >= 3)
            {
                string possibleSuffix = text.Substring(text.Length - 2);
                if (char.IsLower(possibleSuffix[0]) && char.IsLower(possibleSuffix[1]))
                {
                    int first = possibleSuffix[0] - 'a';
                    int second = possibleSuffix[1] - 'a';
                    int tier = ShortSuffixes.Length + first * 26 + second;
                    string numPart = text.Substring(0, text.Length - 2);
                    if (double.TryParse(numPart, out double num))
                    {
                        value = num * Math.Pow(1000, tier);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns compact time string: "2h 30m", "45m 12s", "30s"
        /// </summary>
        public static string FormatTime(double totalSeconds)
        {
            if (totalSeconds <= 0) return "0s";

            int hours = (int)(totalSeconds / 3600);
            int minutes = (int)((totalSeconds % 3600) / 60);
            int seconds = (int)(totalSeconds % 60);

            if (hours > 0)
                return $"{hours}h {minutes}m";
            if (minutes > 0)
                return $"{minutes}m {seconds}s";
            return $"{seconds}s";
        }
    }
}
