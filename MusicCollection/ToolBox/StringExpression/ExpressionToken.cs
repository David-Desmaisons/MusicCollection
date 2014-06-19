using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;

namespace MusicCollection.ToolBox.StringExpression
{
    public enum TokenType { OpenBracket, CloseBracket, And, Or, Equal, Superior, Inferior, SuperiorOrEqual, InferiorOrEqual, Different, Attribute, Value }

    public abstract class ExpressionToken
    {
        public abstract TokenType Type
        {
            get;
        }

        public abstract bool IsOutputStack
        {
            get;
        }

        static public ExpressionToken GetToken(string TokenName, SimpleToken stinstack, bool ConstExpected, ParameterExpression p)
        {
            ExpressionToken res = ParanthesisToken.GetToken(TokenName);
            if (res != null)
                return res;

            if (!ConstExpected)
            {
                res = Operator.GetFromString(TokenName);
                if (res != null)
                    return res;
            }

            return SimpleToken.GetFromstring(TokenName, stinstack, p);
        }

    }

    public abstract class ParanthesisToken : ExpressionToken
    {
        public override bool IsOutputStack
        {
            get { return false; }
        }

        private static OpenParanthesisToken _Open;
        private static CloseParanthesisToken _Close;

        static ParanthesisToken()
        {
            _Open = new OpenParanthesisToken();
            _Close = new CloseParanthesisToken();

        }

        static public ParanthesisToken GetToken(string istring)
        {
            if (istring == "(")
            {
                return _Open;
            }
            else if (istring == ")")
            {
                return _Close;
            }

            return null;
        }


        private class OpenParanthesisToken : ParanthesisToken
        {
            public override TokenType Type
            {
                get { return TokenType.OpenBracket; }
            }

            public override string ToString()
            {
                return "(";
            }
        }

        private class CloseParanthesisToken : ParanthesisToken
        {
            public override TokenType Type
            {
                get { return TokenType.CloseBracket; }
            }

            public override string ToString()
            {
                return ")";
            }
        }

    }

    public abstract class SimpleToken : ExpressionToken
    {

        protected string _Name;

        public override bool IsOutputStack
        {
            get { return true; }
        }

        protected SimpleToken(string iName)
        {
            _Name = iName;
        }

        public abstract Expression Builder
        {
            get;
        }


        protected abstract Func<string, ParameterExpression, SimpleToken> NextSimpleTokenFactory
        {
            get;
        }

        public static SimpleToken GetFromstring(string name, SimpleToken old, ParameterExpression pe)
        {
            if (old == null)
            {
                return new AttributeToken(name, pe);
            }

            return old.NextSimpleTokenFactory(name, pe);
        }

        private class AttributeToken : SimpleToken
        {
            public override TokenType Type
            {
                get { return TokenType.Attribute; }
            }

            private MemberExpression _ME;

            public AttributeToken(string name, ParameterExpression p)
                : base(name)
            {
                _ME = Expression.Property(p, _Name);
            }

            public override string ToString()
            {
                return string.Format("[AttributeToken:{0}]", _Name);
            }




            public override Expression Builder
            {
                get { return _ME; }
            }

            protected override Func<string, ParameterExpression, SimpleToken> NextSimpleTokenFactory
            {
                get { return (n, p) => new ValueToken(n, _ME.Type); }
            }
        }

        private class ValueToken : SimpleToken
        {
            public override TokenType Type
            {
                get { return TokenType.Value; }
            }

            public Type TargetType
            {
                get;
                private set;
            }

            public object Value
            {
                get;
                private set;
            }

            private ConstantExpression _CE;

            public ValueToken(string name, Type targettype)
                : base(name)
            {
                TargetType = targettype;
                TypeConverter TypeConvertor = TypeDescriptor.GetConverter(TargetType);
                Value = TypeConvertor.ConvertFromString(name);

                _CE = Expression.Constant(Value);
            }

            public override string ToString()
            {
                return string.Format("[ValueToken:Type:{0},Value:{1}]", TargetType, Value);
            }

            public override Expression Builder
            {
                get { return _CE; }
            }

            protected override Func<string, ParameterExpression, SimpleToken> NextSimpleTokenFactory
            {
                get { return (n, p) => new AttributeToken(n, p); }
            }
        }

    }



    public abstract class Operator : ExpressionToken
    {
        public override bool IsOutputStack
        {
            get { return false; }
        }

        public abstract int Precedence
        {
            get;
        }

        public abstract Func<Expression, Expression, Expression> Builder
        {
            get;
        }

        private Operator()
        {
            _Operators.Add(this.ToString(), this);
        }

        private static Dictionary<string, Operator> _Operators = new Dictionary<string, Operator>();

        static public Operator GetFromString(string isting)
        {
            Operator res = null;
            _Operators.TryGetValue(isting, out res);
            return res;
        }

        static Operator()
        {
            new AndOperator();
            new OrOperator();
            new EqualOperator();
            new DifferentOperator();
            new InferiorOperator();
            new InferiorOrEqualOperator();
            new SuperiorOperator();
            new SuperiorOrEqualOperator();
        }



        private class AndOperator : Operator
        {
            public AndOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.And; }
            }

            public override int Precedence
            {
                get { return 3; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.And(Left, Rigth); }
            }

            public override string ToString()
            {
                return "and";
            }
        }


        private class OrOperator : Operator
        {
            public OrOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.Or; }
            }

            public override int Precedence
            {
                get { return 1; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.Or(Left, Rigth); }
            }

            public override string ToString()
            {
                return "or";
            }
        }

        private class EqualOperator : Operator
        {
            public EqualOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.Equal; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.Equal(Left, Rigth); }
            }

            public override string ToString()
            {
                return "=";
            }
        }

        private class DifferentOperator : Operator
        {
            public DifferentOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.Different; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.NotEqual(Left, Rigth); }
            }

            public override string ToString()
            {
                return "!=";
            }
        }

        private class SuperiorOperator : Operator
        {
            public SuperiorOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.Superior; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.GreaterThan(Left, Rigth); }
            }

            public override string ToString()
            {
                return ">";
            }
        }


        private class InferiorOperator : Operator
        {
            public InferiorOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.Inferior; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.LessThan(Left, Rigth); }
            }

            public override string ToString()
            {
                return "<";
            }
        }


        private class SuperiorOrEqualOperator : Operator
        {
            public SuperiorOrEqualOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.SuperiorOrEqual; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.GreaterThanOrEqual(Left, Rigth); }
            }

            public override string ToString()
            {
                return ">=";
            }
        }


        private class InferiorOrEqualOperator : Operator
        {
            public InferiorOrEqualOperator()
            {
            }

            public override TokenType Type
            {
                get { return TokenType.InferiorOrEqual; }
            }

            public override int Precedence
            {
                get { return 5; }
            }

            public override Func<Expression, Expression, Expression> Builder
            {
                get { return (Left, Rigth) => Expression.LessThanOrEqual(Left, Rigth); }
            }

            public override string ToString()
            {
                return "<=";
            }
        }


    }
}
