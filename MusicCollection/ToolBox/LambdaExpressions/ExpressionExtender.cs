using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Linq.Expressions;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    static class ExpressionExtender
    {
        internal static bool IsParameterDependant(this Expression expr)
        {
            ParameterDependantIsVisitor pdiv = new ParameterDependantIsVisitor(expr);
            return pdiv.IsParameterDependant;
        }

        //internal static IEnumerable<ParameterExpression> ParameterBreakdown(this Expression expr)
        //{
        //    ParameterDependantIsVisitor pdiv = new ParameterDependantIsVisitor(expr);
        //    return pdiv.Parameters;
        //}

        internal static Expression ConvertToType(this Expression my, Type itype)
        {
            return itype.IsClass ? Expression.TypeAs(my, itype) : Expression.Convert(my, itype);
        }

        internal static bool IsTheSame(this Expression mexpr, Expression other)
        {
            //todo perfectionner et etendre comparaison
            if (object.ReferenceEquals(mexpr, other))
                return true;

            if (mexpr == null)
                throw new ArgumentNullException();

            if (other == null)
                return false;

            if (mexpr.NodeType != other.NodeType)
                return false;

            if (mexpr.Type != other.Type)
                return false;

            switch (mexpr.NodeType)
            {
                case ExpressionType.Lambda:
                    LambdaExpression lexpr = mexpr as LambdaExpression;
                    LambdaExpression lother = other as LambdaExpression;
                    return ((lexpr.ReturnType == lother.ReturnType) && (lexpr.Parameters.SequenceEqual(lother.Parameters, ExpressionComparer.Comparer)) && (lexpr.Body.IsTheSame(lother.Body)));

                case ExpressionType.Parameter:
                    ParameterExpression pexpr = mexpr as ParameterExpression;
                    ParameterExpression pother = other as ParameterExpression;
                     return ((object.ReferenceEquals(pexpr.Name, pother.Name)) && (pexpr.IsByRef == pother.IsByRef) && (pexpr.Type == pother.Type));

                case ExpressionType.Invoke:
                     InvocationExpression ie = mexpr as InvocationExpression;
                     InvocationExpression io = other as InvocationExpression;
                     return ((ie.Expression.IsTheSame(io.Expression)) && (ie.Arguments.SequenceEqual(io.Arguments, ExpressionComparer.Comparer)));

                case ExpressionType.New:
                     NewExpression ne = mexpr as NewExpression;
                     NewExpression no = other as NewExpression;
                    return ((ne.Constructor == no.Constructor) && ((ne.Members == no.Members) || (ne.Members.SequenceEqual(no.Members)))
                    && (ne.Arguments.SequenceEqual(no.Arguments, ExpressionComparer.Comparer)));

                case ExpressionType.MemberAccess:
                          MemberExpression mmexpr = mexpr as MemberExpression;
                          MemberExpression mother = other as MemberExpression;
                          return ( (mmexpr.Member == mother.Member) && ( mmexpr.Expression.IsTheSame(mother.Expression)));

                case ExpressionType.Constant:
                          ConstantExpression cmexpr = mexpr as ConstantExpression;
                          ConstantExpression cother = other as ConstantExpression;
                          return object.Equals(cmexpr.Value, cother.Value);

                case ExpressionType.Call:
                          MethodCallExpression mcexpr = mexpr as MethodCallExpression;
                          MethodCallExpression mcother = other as MethodCallExpression;
                      return ((mcexpr.Method == mcother.Method) && (mcexpr.Arguments.SequenceEqual(mcother.Arguments, ExpressionComparer.Comparer)) && (mcexpr.Object.IsTheSame(mcother.Object)));

                case ExpressionType.Or:
                case ExpressionType.And:
                case ExpressionType.OrElse:
                case ExpressionType.Subtract:
                case ExpressionType.Add:
                case ExpressionType.Multiply:
                case ExpressionType.Modulo:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Divide:
                case ExpressionType.AndAlso:
                case ExpressionType.Assign:
                case ExpressionType.Power:

                      BinaryExpression bexpr = mexpr as BinaryExpression;
                      BinaryExpression bother = other as BinaryExpression;
                     return ((bexpr.IsLifted == bother.IsLifted) && (bexpr.IsLiftedToNull == bother.IsLiftedToNull) && (bexpr.Conversion.IsTheSame(bother.Conversion)) &&
                    (bexpr.Left.IsTheSame(bother.Left)) && (bexpr.Right.IsTheSame(bother.Right)) && (bexpr.Method == bother.Method));

               
                case ExpressionType.Convert:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.ConvertChecked:
                case ExpressionType.IsFalse:
                case ExpressionType.IsTrue:

                     UnaryExpression uexpr = mexpr as UnaryExpression;
                     UnaryExpression uother = other as UnaryExpression;
               
                return ((uexpr.IsLifted == uother.IsLifted) && (uexpr.IsLiftedToNull == uother.IsLiftedToNull) && (uexpr.Method == uother.Method) &&
                     (uexpr.Operand.IsTheSame(uother.Operand)));
  
            }

            Trace.WriteLine(string.Format("Test Expression not supported {0}", mexpr.NodeType));
            return mexpr == other;
        }

        internal static int GetRelevantHashCode(this Expression mexpr)
        {
            if (mexpr == null)
                throw new ArgumentNullException();

            return (mexpr.NodeType.GetHashCode()) ^ (mexpr.Type.GetHashCode());
        }

    }
}
