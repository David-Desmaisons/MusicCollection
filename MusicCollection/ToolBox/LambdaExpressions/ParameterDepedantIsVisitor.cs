using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;

namespace MusicCollection.ToolBox.LambdaExpressions
{  
    internal class ParameterDependantIsVisitor : ExpressionVisitor
    {
        internal ParameterDependantIsVisitor(Expression node)
        {
            Visit(node);
        }

        private HashSet<ParameterExpression> _Parameter = null;

        private void AddParameter(ParameterExpression node)
        {
            if (_Parameter == null)
                _Parameter = new HashSet<ParameterExpression>();

            _Parameter.Add(node);
        }

        internal bool IsParameterDependant
        {
            get { return _Parameter != null; }
        }

        internal IEnumerable<ParameterExpression> Parameters
        {
            get
            {
                return _Parameter ?? Enumerable.Empty<ParameterExpression>();
            }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            AddParameter(node);
            return base.VisitParameter(node);
        }
    }
}
