using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    interface IVisitIObjectAttribute
    {
        void Visit(IObjectAttribute io, PropertyInfo myProperty, bool IsParameter);

        void Visit(INotifyCollectionChanged icc);
    }

}
