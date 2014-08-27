using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Collections.Specialized;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.LambdaExpressions
{
    internal abstract class ListenedItem<Tor, TDes> where Tor : class
    {
        private ListenedElementCollection _ListenedElement;
        protected CompleteDynamicFunction<Tor, TDes> _Father;

        internal ListenedItem(CompleteDynamicFunction<Tor, TDes> iFather)
        {
            _Father = iFather;
        }

        internal bool Islistening(object listened)
        {
            return this._ListenedElement.Contains(listened);
        }

        protected void Init()
        {
            var builder = ListenedElementCollection.GetBuilder();
            CachedValue = Visit(builder);
            _ListenedElement = builder.GetCollection();

            _ListenedElement.CollectionListened.Apply(le => le.CollectionChanged += CollectionChanged);
            _ListenedElement.ObjectAttributeProperties.Apply(oe => oe.ObjectChanged += ObjectChanged);
        }

        internal void Unregister()
        {
            _ListenedElement.CollectionListened.Apply(le => le.CollectionChanged -= CollectionChanged);
            _ListenedElement.ObjectAttributeProperties.Apply(oe => oe.ObjectChanged -= ObjectChanged);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update(true);
        }

        private void ObjectChanged(object sender, ObjectModifiedArgs oa)
        {
            if (!_ListenedElement.IsPertinent(oa))
                return;

            Update(false);
        }

        protected EventHandler<ObjectModifiedArgs> ObjectListenerProtected
        {
            get { return ObjectChanged; }
        }

        abstract protected void OnChanges(TDes oldv, TDes nvalue);
        abstract protected TDes Visit(IVisitIObjectAttribute iov);
        abstract protected TDes CachedValue { set; get; }


        private void Update(bool IsCollection)
        {
            var builder = ListenedElementCollection.GetBuilder();

            TDes nouv = Visit(builder);

            var new_ListenedElement = builder.GetCollection();

            _ListenedElement.ObjectAttributeProperties.Where(o => !new_ListenedElement.Contains(o))
                .Apply(o => o.ObjectChanged -= ObjectChanged);
            new_ListenedElement.ObjectAttributeProperties.Where(o => !_ListenedElement.Contains(o))
                .Apply(o => o.ObjectChanged += ObjectChanged);


            _ListenedElement.CollectionListened.Where(o => !new_ListenedElement.Contains(o))
               .Apply(o => o.CollectionChanged -= CollectionChanged);
            new_ListenedElement.CollectionListened.Where(o => !_ListenedElement.Contains(o))
                .Apply(o => o.CollectionChanged += CollectionChanged);

            _ListenedElement = new_ListenedElement;


            var old = CachedValue;
            CachedValue = nouv;

            OnChanges(old, CachedValue);
        }
    }

    internal class ListenedRoot<Tor, TDes> : ListenedItem<Tor, TDes> where Tor : class
    {

        public override string ToString()
        {
            return string.Format("Root:<{0}> Value<{1}> FatherExpression<{2}>", _Origin, _CachedValue,_Father.Expression);
        }

        public int Count { get; set; }

        private Tor _Origin;
        internal ListenedRoot(CompleteDynamicFunction<Tor, TDes> iFather, Tor iOr):base(iFather)
        {
            Count = 1;
             _Origin = iOr;
             Init();
        }

        protected override void OnChanges(TDes oldv, TDes nvalue)
        {
            if (object.Equals(oldv, nvalue))
                return;

            _Father.OnChanges(_Origin, oldv, nvalue);
        }

        internal EventHandler<ObjectModifiedArgs> ObjectListener
        {
            get { return ObjectListenerProtected; }
        }

        protected override TDes Visit(IVisitIObjectAttribute iov)
        {
            return _Father.Computor(_Origin, iov);
        }

        internal void UpdateOnConstantChanged()
        {
            var old = _CachedValue;

            _CachedValue =  _Father.Evaluate(_Origin);

            OnChanges(old, _CachedValue);
        }

        internal void UpdateOnConstantChanged(ObjectAttributesChangedArgs<Tor, TDes> cached, Func<Tor, TDes> Computor)
        {
            var old = _CachedValue;
            _CachedValue = Computor(_Origin);

            if (object.Equals(old, _CachedValue))
                return;

            cached.AddChanges(this._Origin, new ObjectAttributeChangedArgs<TDes>(this._Origin, null, old, _CachedValue));
        }

        private TDes _CachedValue;
        protected override TDes CachedValue
        {
            set {  _CachedValue = value; }
            get { return _CachedValue; }
        }

        internal TDes CurrentValue
        {
            get { return _CachedValue;  }
        }
    }

    internal class ListenedConstant<Tor, TDes> : ListenedItem<Tor, TDes> where Tor : class
    {
         internal ListenedConstant(CompleteDynamicFunction<Tor, TDes> iFather)
             : base(iFather)
        {
            Init();
        }

         public override string ToString()
         {
             return string.Format("FatherExpression<{0}>", _Father.Expression);
         }

         protected override void OnChanges(TDes oldv, TDes nvalue)
         {
             _Father.ConstantChanges();
         }

         protected override TDes Visit(IVisitIObjectAttribute iov)
         {
             _Father.ConstantVisitor(iov);
             return default(TDes);
         }

         protected override TDes CachedValue
         {
             set{}
             get { return default(TDes); }
         }
    }

}
