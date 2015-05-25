using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;

using MusicCollection.ToolBox.LambdaExpressions;
using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal abstract class DynamicResultFunction<TSource, TDest, TInt> : DynamicResultBase<TSource, TDest>, IInvariant where TSource : class
    {
        private RegistorCollectionChanged<TSource> _RCF;
        protected IFunction<TSource, TInt> _Function;


        protected DynamicResultFunction(IList<TSource> source, Expression<Func<TSource, TInt>> Transformer)
            : base(source)
        {
            _Function = Transformer.CompileToObservable();
            _RCF = RegistrorCollectionIFunctionChanges<TSource, TInt>.GetListener(source, _Function, new ListenerAdapter(this));
        }

        public override void Dispose()
        {
            _RCF.Dispose();
        }

        #region listeneradapter

        private class ListenerAdapter : ICollectionFunctionListener<TSource, TInt>
        {
            private DynamicResultFunction<TSource, TDest, TInt> _Father;
            internal ListenerAdapter(DynamicResultFunction<TSource, TDest, TInt> iFather)
            {
                _Father = iFather;
            }

            void ICollectionFunctionListener<TSource, TInt>.OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<TInt> changes)
            {
                _Father.OnCollectionItemPropertyChanged(item, changes);
            }

            void ICollectionListener<TSource>.AddItem(TSource newItem, int index, Nullable<bool> First)
            {
                _Father.AddItem(newItem, index, First);
            }

            void ICollectionListener<TSource>.AddItems(IEnumerable<Changed<TSource>> sources)
            {
                _Father.AddItems(sources);
            }

            bool ICollectionListener<TSource>.RemoveItem(TSource oldItem, int index, Nullable<bool> Last)
            {
                return _Father.RemoveItem(oldItem, index, Last);
            }

            IDisposable ICollectionListener<TSource>.GetFactorizable()
            {
                return _Father.GetFactorizable();
            }

            void ICollectionListener<TSource>.OnClear()
            {
                _Father.OnClear();
            }


            public void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TSource, TInt> Changes)
            {
                _Father.OnCollectionItemsPropertyChanged(Changes);
            }

            public bool FactorizeEvents
            {
                get { return _Father.FactorizeEvents; }
            }
        }

        #endregion

        protected abstract void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<TInt> changes);

        protected abstract void AddItem(TSource newItem, int index, Nullable<bool> First);

        protected virtual void AddItems(IEnumerable<Changed<TSource>> sources)
        {
            sources.Apply(s => AddItem(s.Source, s.Index, s.First));
        }

        protected abstract bool RemoveItem(TSource oldItem, int index, Nullable<bool> Last);

        protected abstract IDisposable GetFactorizable();

        protected abstract void OnClear();

        protected virtual void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TSource, TInt> Changes)
        {
            throw new NotImplementedException();
        }

        protected virtual bool FactorizeEvents
        {
            get { return false; }
        }

        public abstract bool Invariant
        {
            get;
        }
    }

}
