using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal class SimpleFunction<T> : NotifyCompleteAdapterNoCache, IFunction<T>
    {
        private Func<IVisitIObjectAttribute, T> _Func;
        private ListenedElementCollection _ListenedElement;

        internal SimpleFunction(Expression<Func<T>> iexpression)
        {
            ExpressionVisitorFunction<T> evs = new ExpressionVisitorFunction<T>(iexpression);
            _Func = evs.Transformed.Compile();

            var builder = ListenedElementCollection.GetBuilder();
            Value = _Func(builder);
            
            _ListenedElement = builder.GetCollection();
            _ListenedElement.CollectionListened.Apply(le => le.CollectionChanged += CollectionChanged);
            _ListenedElement.ObjectAttributeProperties.Apply(oe => oe.ObjectChanged += OnObjectChanged);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        private void OnObjectChanged(object sender, ObjectModifiedArgs oa)
        {
            if (!_ListenedElement.IsPertinent(oa))
                return;

            Update();
        }

        private void Update()
        {
           var builder = ListenedElementCollection.GetBuilder();

            T nouv = _Func(builder);

            var new_ListenedElement = builder.GetCollection();

            _ListenedElement.ObjectAttributeProperties.Where(o => !new_ListenedElement.Contains(o))
                .Apply(o => o.ObjectChanged -= OnObjectChanged);
            new_ListenedElement.ObjectAttributeProperties.Where(o => !_ListenedElement.Contains(o))
                .Apply(o => o.ObjectChanged += OnObjectChanged);


            _ListenedElement.CollectionListened.Where(o => !new_ListenedElement.Contains(o))
               .Apply(o => o.CollectionChanged -= CollectionChanged);
            new_ListenedElement.CollectionListened.Where(o => !_ListenedElement.Contains(o))
                .Apply(o => o.CollectionChanged += CollectionChanged);

            _ListenedElement = new_ListenedElement;

            Value = nouv;

        }

        private T _Value;
        public T Value
        {
            get { return _Value; }
            private set { this.Set(ref _Value, value); }
        }

        public void Dispose()
        {
            _ListenedElement.CollectionListened.Apply(le => le.CollectionChanged -= CollectionChanged);
            _ListenedElement.ObjectAttributeProperties.Apply(oe => oe.ObjectChanged -= OnObjectChanged);
        }

        public EventHandler<ObjectModifiedArgs> ObjectListener
        {
            get { return OnObjectChanged; }
        }

        public object RawValue
        {
            get { return _Value; }
        }

        public bool IsObserved
        {
            get { return IsObjectObserved; }
        }
    }

    internal class SimplePropertyFunction<T> : SimpleFunction<T>, IPropertyFunction<T>
    {
        internal SimplePropertyFunction(Expression<Func<T>> iexpression, string iname)
            : base(iexpression)
        {
            PropertyName = iname;
        }

        public string PropertyName
        {
	        get;private set;
        }

    }
}
