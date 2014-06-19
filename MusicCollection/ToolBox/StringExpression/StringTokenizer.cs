using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.StringExpression
{
    public class StringTokenizer
    {
        private string _Expression;
        private bool _Error = false;

        public StringTokenizer(string iExpression)
        {
            _Expression = iExpression;
        }

        public bool Error
        {
            get { return _Error; }
        }

        private class Expecter
        {
            private char _Expected;
            private bool _ContinueOrError;

            internal Expecter(char expected, bool ContinueOnError)
            {
                _Expected = expected;
                _ContinueOrError = ContinueOnError;
            }

            internal bool MeetExpectation(char entry)
            {
                return _Expected == entry;
            }

            internal bool ContinueOnError
            {
                get { return _ContinueOrError; }
            }
        }

        public IEnumerable<string> GetTokens()
        {
            StringBuilder builder = null;
            bool insidequote = false;
            Expecter expected = null;

            foreach (char c in _Expression)
            {
                if (expected != null)
                {
                    if (!expected.MeetExpectation(c))
                    {
                        if (!expected.ContinueOnError)
                        {
                            _Error = true;
                            break;
                        }

                        expected = null;

                        if (builder != null)
                        {
                            yield return builder.ToString();
                            builder = null;
                        }
                    }
                    else
                    {

                        if (builder == null)
                        {
                            _Error = true;
                            break;
                        }

                        builder.Append(c);
                        yield return builder.ToString();
                        builder = null;

                        expected = null;
                        continue;
                    }
                }

                


                if (c == '"')
                {
                    if (insidequote == false)
                    {
                        if (builder != null)
                        {
                            _Error = true;
                            break;
                        }
                        insidequote = true;
                        builder = new StringBuilder();
                        continue;
                    }
                    else
                    {
                        insidequote = false;
                        yield return builder.ToString();
                        builder = null;
                        continue;
                    }
                }


                if (insidequote)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }
                    builder.Append(c);
                    continue;
                }


                if (c == '(')
                {
                    if (builder != null)
                    {
                        yield return builder.ToString();
                        builder = null;
                    }
                    yield return "(";
                    continue;
                }

                if (c == ')')
                {
                    if (builder != null)
                    {
                        yield return builder.ToString();
                        builder = null;
                    }
                    yield return ")";
                    continue;
                }

                if ( (c == '<') || (c == '>'))
                {
                    if (builder != null)
                    {
                        yield return builder.ToString();
                    }
                    builder = new StringBuilder();
                    builder.Append(c);
                    expected = new Expecter('=',true);
                    continue;
                }

                //if 
                //{
                //    if (builder != null)
                //    {
                //        yield return builder.ToString();
                //        builder = null;
                //    }
                //    yield return ">";
                //    continue;
                //}

                if (c == '=')
                {
                    if (builder != null)
                    {

                        //if (builder.ToString() == "!")
                        //{
                        //    builder = null;
                        //    yield return "!=";
                        //    continue;
                        //}
                        //else
                        //{
                            yield return builder.ToString();
                            builder = null;
                            
                        //}

                      
                    } 
                    
                    yield return "=";
                    continue;
                }

                if (c == ' ')
                {
                    if (builder != null)
                    {
                        yield return builder.ToString();
                        builder = null;
                    }
                    continue;
                }

                if (c == '!')
                {
                    if (builder != null)
                    {
                        yield return builder.ToString();
                    }

                    expected = new Expecter('=', false);

                    builder = new StringBuilder("!");
                    continue;
                }

                if (!char.IsLetterOrDigit(c))
                {
                    _Error = true;
                    break;
                }

                if (builder == null)
                {
                    builder = new StringBuilder();
                }
                builder.Append(c);

            }

            if (builder != null)
                yield return builder.ToString();

            yield break;
        }
    }
}
