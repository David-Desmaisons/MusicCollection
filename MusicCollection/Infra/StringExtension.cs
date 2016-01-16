using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using MusicCollection.ToolBox;

namespace MusicCollection.Infra
{
    public static class StringExtension
    {
        private static readonly char[] _Blank;
        private static readonly Encoding _Encoding;
        private const string _Ect = "...";

        static StringExtension()
        {
            _Blank = new char[] { ' ' };
            _Encoding = Encoding.GetEncoding("us-ascii", new EncoderReplacementFallback("-"), new DecoderReplacementFallback(string.Empty));
        }

        public static IEnumerable<string> GetLowerWithoutAccentSubstrings(this string @this, int Size)
        {
            return @this.ToLowerWithoutAccent().GetSubStrings(Size);
        }

        public static IEnumerable<string> GetSubStrings(this string @this, int Size)
        {
            int size = @this.Length - Size;
            if (size < 0)
            {
                yield return @this;
                yield break;
            }

            for (int i = 0; i <= size; i++)
            {
                yield return @this.Substring(i, Size);
            }
        }


        public static IEnumerable<string> GetExactSubstrings(this string @this, int Size)
        {
            int size = @this.Length-Size;
            for (int i = 0; i <= size; i++)
            {
                yield return @this.Substring(i, Size);
            }
        }

       

        public static int GetDamerauLevenshteinDistance(this string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    return 0;
                }
                else
                {
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            var score = new int[source.Length + 2, target.Length + 2];

            var INF = source.Length + target.Length;
            score[0, 0] = INF;
            for (var i = 0; i <= source.Length; i++) { score[i + 1, 1] = i; score[i + 1, 0] = INF; }
            for (var j = 0; j <= target.Length; j++) { score[1, j + 1] = j; score[0, j + 1] = INF; }

            var sd = new SortedDictionary<char, int>();
            foreach (var letter in (source + target))
            {
                if (!sd.ContainsKey(letter))
                    sd.Add(letter, 0);
            }

            for (var i = 1; i <= source.Length; i++)
            {
                var DB = 0;
                for (var j = 1; j <= target.Length; j++)
                {
                    var i1 = sd[target[j - 1]];
                    var j1 = DB;

                    if (source[i - 1] == target[j - 1])
                    {
                        score[i + 1, j + 1] = score[i, j];
                        DB = j;
                    }
                    else
                    {
                        score[i + 1, j + 1] = Math.Min(score[i, j], Math.Min(score[i + 1, j], score[i, j + 1])) + 1;
                    }

                    score[i + 1, j + 1] = Math.Min(score[i + 1, j + 1], score[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[source[i - 1]] = i;
            }

            return score[source.Length + 1, target.Length + 1];
        }

        public static string WithoutAccent(this string iName)
        {

            string stFormD = iName.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            string normalized = sb.ToString().Normalize(NormalizationForm.FormKD);

            byte[] encodedBytes = new byte[_Encoding.GetByteCount(normalized)];
            int numberOfEncodedBytes = _Encoding.GetBytes(normalized, 0, normalized.Length, encodedBytes, 0);
            return _Encoding.GetString(encodedBytes);
        }

        public static string ToLowerWithoutAccent(this string iName)
        {
            return iName.ToLower().WithoutAccent();
        }

        public static bool FastContains(this string source, string pattern)
        {
            return source.FastIndexOf(pattern) >= 0;
        }

        public static string Normalized(this string source)
        {
           return (source == null) ? null : source.NormalizeSpace().ToLower().WithoutAccent();
        }

        public static int FastIndexOf(this string source, string pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            if (pattern.Length == 0) return 0;
            if (pattern.Length == 1) return source.IndexOf(pattern[0]);
            bool found;
            int limit = source.Length - pattern.Length + 1;
            if (limit < 1) return -1;
            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];
            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }
                // Check the rest of "pattern" (starting with the 3rd character)
                found = true;
                for (int j = 2; j < pattern.Length; j++)
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                // If the whole word was found, return its index, otherwise try again
                if (found) return first;
                first = source.IndexOf(c0, ++first, limit - first);
            }
            return -1;
        }
    }
}
