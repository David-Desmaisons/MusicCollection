using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox
{
    internal static class StringExtender
    {


        internal static string TitleLike(this string iName)
        {
          if (iName == null)
                throw new ArgumentNullException("this can not be null");


            StringBuilder SB = new StringBuilder();
            bool NeedToUp = true;
            bool skeepspace = true;

            foreach (char c in iName)
            {
                if (skeepspace && c == ' ')
                    continue;

                if (NeedToUp)
                {
                    SB.Append(char.ToUpper(c));
                }
                else
                {
                    SB.Append(char.ToLower(c));   
                }

                switch (c)
                {
                    case ' ':
                        skeepspace = true;
                        NeedToUp = true;
                        break;

                    case ',':
                    case '-':
                    case '.':
                    case ';':
                    case '/':
                    case '\'':
                    case '\\':
                        NeedToUp = true;
                        skeepspace = false;
                        break;

                    default:
                        NeedToUp = false;
                        skeepspace = false;
                        break;
                }
            }

            if ((skeepspace) && (SB.Length > 0))
                SB.Remove(SB.Length - 1, 1);

            return SB.ToString();
}

        internal static string NormalizeSpace(this string Name)
        {
            return Regex.Replace(Name.Trim(), @"\s+", " ");
        }

        internal static string ToMaxLength(this string iName, int MaxL)
        {
            if (iName == null)
                return string.Empty;

            if (iName.Length <= MaxL)
                return string.Copy(iName);

            return iName.Remove(MaxL).TrimEnd();
       }

        static internal string RemoveInvalidCharacters(this string fname)
        {
            return new string((from c in fname
                               where ((c == '.') || (!char.IsPunctuation(c) && !char.IsSymbol(c)))
                               select (char.IsSeparator(c) || char.IsWhiteSpace(c)) ? ' ' : c).ToArray());
        }

        static internal string FormatForDirectoryName(this string fname, int Limit = -1)
        {
            return fname.FormatFileName(Limit).Replace(",", "").Replace(@".", " ").Trim();
        }

        static internal string FormatExistingRelativeDirectoryName(this string fname)
        {
            if (fname == null)
                return null;

            return Path.Combine((from fn in fname.Split('\\') select fn.FormatFileName()).ToArray());
        }

        static internal string FormatFileName(this string fname, int LimitLength = -1)
        {
            if ((LimitLength > -1) && (LimitLength < 7))
                LimitLength = 7;

            if (LimitLength < 0)
                LimitLength = -1;

            if (string.IsNullOrEmpty(fname))
                return "Unknown";

            string res = fname.WithoutAccent().RemoveInvalidCharacters().Trim();

            if (string.IsNullOrEmpty(res))
                return "Unknown";

            if ((LimitLength == -1) || (res.Length <= LimitLength))
                return res;

            return res.Substring(0, LimitLength).TrimEnd();
        }

        static public int TryParse(this string @this)
        {
            int tentativeint = 0;
            int.TryParse(@this, out tentativeint);
            return tentativeint;
        }

    }
}
