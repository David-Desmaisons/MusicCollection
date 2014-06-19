using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollectionTest.TestObjects
{
    public static class StringExtensionTest
    {
         static readonly char[] _Blank;

         static StringExtensionTest()
        {
            _Blank = new char[] { ' ' };
        }

        public static IEnumerable<string> GetSubstring(this string @this)
        {
            string[] words = @this.Split(_Blank, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> res = new HashSet<string>();
            res.Add(string.Empty);
            foreach (string w in words)
            {
                GetPrivateSubstring(w).Apply(s => res.Add(s));
            }

            return res;
        }


        private static IEnumerable<string> GetPrivateSubstring(string @this)
        {
            List<StringBuilder> builders = new List<StringBuilder>();

            int Size = @this.Length;
            for (int i = 0; i < Size; i++)
            {
                builders.Add(new StringBuilder());
                builders.Apply(b => b.Append(@this[i]));

                foreach (StringBuilder sb in builders)
                {
                    yield return sb.ToString();
                }
            }
        }


        public static IEnumerable<string> GetSubstrings(this string @this, int Size)
        {
            LinkedList<StringBuilder> builders = new LinkedList<StringBuilder>();

            bool Full = false;
            int c = 0;

            foreach (char a in @this)
            {
                c++;

                if (!Full)
                {
                    if (c == Size)
                        Full = true;
                }
                else
                {
                    builders.RemoveFirst();
                }

                builders.AddLast(new StringBuilder());
                builders.Apply(b => b.Append(a));

                foreach (StringBuilder sb in builders)
                {
                    yield return sb.ToString();
                }
            }
        }
    }
}
