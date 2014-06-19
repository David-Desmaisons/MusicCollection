using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MusicCollection.ToolBox.StringExpression
{
    public static class StackExtension
    {
        public static T TryPop<T>(this Stack<T> st) where T : class
        {
            if (st.Count == 0)
                return null;

            return st.Pop();
        }

        public static T TryPeek<T>(this Stack<T> st) where T : class
        {
            if (st.Count == 0)
                return null;

            return st.Peek();
        }
    }

    static class EpressionExtender
    {
        internal static Expression ConvertToType(this Expression my, Type itype)
        {
            return itype.IsClass ? Expression.TypeAs(my, itype) : Expression.Convert(my, itype);
        }
    }
}
