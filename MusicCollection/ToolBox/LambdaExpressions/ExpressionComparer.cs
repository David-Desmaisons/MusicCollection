using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class ExpressionComparer : IEqualityComparer<Expression>
    {
        static public IEqualityComparer<Expression> Comparer
        {
            get;
            private set;
        }

        private ExpressionComparer()
        {
        }

        static ExpressionComparer()
        {
            Comparer = new ExpressionComparer();
        }

        public bool Equals(Expression x, Expression y)
        {
            return x.IsTheSame(y);
        }

        public int GetHashCode(Expression obj)
        {
            return obj.GetRelevantHashCode();
        }
    }
}
