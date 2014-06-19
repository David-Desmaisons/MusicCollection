using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Diagnostics;

using MusicCollection.ToolBox.Collection;

//Shunting-yard algorithm

namespace MusicCollection.ToolBox.StringExpression
{
    public class ExpressionFilterBuilder<T>
    {

        static private readonly PolyMorphDictionaryGeneric<string, ExpressionFilterBuilder<T>> _Cache = null;

        static ExpressionFilterBuilder()
        {
            _Cache = new PolyMorphDictionary<string, ExpressionFilterBuilder<T>>();
        }

        public Expression<Func<T, bool>> ExpressionResult
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return String.Format("Name: {0} - Expression: {1} - Exception: {2}", Name, ExpressionResult, BuildingException);
        }

        private static bool ApplyBinary(Stack<Expression> expressions, Operator op)
        {
            Expression left = expressions.TryPop();
            Expression right = expressions.TryPop();

            if ((left == null) || (right == null))
                return false;

            expressions.Push(op.Builder(right, left));
            return true;
        }

        public Exception BuildingException
        {
            get;
            private set;
        }

        public ExpressionFilterBuilder(string iName)
        { 
            Name = iName;

            ExpressionFilterBuilder<T> sim = null;
            if (_Cache.TryGetValue(iName, out sim))
            {
                ExpressionResult = sim.ExpressionResult;
                BuildingException = sim.BuildingException;
                return;
            }

            try
            {
                ExpressionResult = GetFilterFromString(iName);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                BuildingException = e;
            }

            _Cache.Add(Name, this);
           
        }


        static private Expression<Func<T, bool>> GetFilterFromString(string Entry)
        {
            StringTokenizer stn = new StringTokenizer(Entry);

            var Elements = stn.GetTokens().ToList();

            if (stn.Error)
                return null;

            Stack<Expression> expressions = new Stack<Expression>();
            Stack<ExpressionToken> Operators = new Stack<ExpressionToken>();

            ParameterExpression p = Expression.Parameter(typeof(T), "x");

            SimpleToken lastst = null;
            bool ConstExpected = false;


            foreach (string t in Elements)
            {
                ExpressionToken res = ExpressionToken.GetToken(t, lastst,ConstExpected, p);
                if (res == null)
                    return null;

                ConstExpected = false;

                if (res.IsOutputStack)
                {
                    lastst = res as SimpleToken;
                    expressions.Push(lastst.Builder);
                }
                else
                {
                    if (res is Operator)
                    {
                        Operator op = (Operator)res;
                        int prec = op.Precedence;
                        if (prec == 5)
                        {
                            ConstExpected = true;
                        }


                        if (Operators.Count == 0)
                        {
                            Operators.Push(op);
                        }
                        else
                        {
                            Operator other = Operators.TryPeek() as Operator;                          
                            
                            while ((other != null) && (other.Precedence >= prec))
                            {
                                Operators.Pop();
                                if (!ApplyBinary(expressions, other))
                                    return null;

                                other = Operators.TryPeek() as Operator;
                            }
                            Operators.Push(op);

                        }
                    }
                    else
                    {
                        if (res.Type == TokenType.OpenBracket)
                        {
                            Operators.Push(res);
                        }
                        else if (res.Type == TokenType.CloseBracket)
                        {
                            ExpressionToken ot = null;
                            bool ok = false;
                            while ( ( (ot=Operators.TryPop())!=null ) && ( (ok= (ot.Type==TokenType.OpenBracket) ) == false) )
                            {
                                if (!ApplyBinary(expressions, (ot as Operator)))
                                    return null;
                            }

                            if (ok==false)
                                return null;
                        }

                    }
                }
            }

            while (Operators.Count > 0)
            {
                Operator op = Operators.Pop() as Operator;
                if (op == null)
                    return null;

                if (!ApplyBinary(expressions, op))
                    return null;
            }
                
            if (expressions.Count!=1)
                return null;


            return Expression.Lambda<Func<T, bool>>(expressions.Pop(), p);
        }


        public static implicit operator Expression<Func<T, bool>>(ExpressionFilterBuilder<T> EFT)
        {
            return EFT.ExpressionResult;
        }



    }

}
