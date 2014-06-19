using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    abstract class DynamicResultNoFunction<TSource, TDest> : DynamicResultBase<TSource, TDest>, IInvariant
    {
        private RegistorCollectionChanged<TSource> _RCF;

         protected DynamicResultNoFunction(IList<TSource> source)
            : base(source)
        {
            _RCF = RegistorCollectionChanged<TSource>.GetListener(source, new ListenerAdapter(this));
        }

        public override void Dispose()
        {
            _RCF.Dispose();
        }

        #region listeneradapter

        private class ListenerAdapter : ICollectionListener<TSource>
        {
            private DynamicResultNoFunction<TSource, TDest> _Father;
            internal ListenerAdapter(DynamicResultNoFunction<TSource, TDest> iFather)
            {
                _Father = iFather;
            }

            void ICollectionListener<TSource>.AddItem(TSource newItem, int index, Nullable<bool> First)
            {
                _Father.AddItem(newItem, index, First);
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
        }


        #endregion

        protected abstract void AddItem(TSource newItem, int index, Nullable<bool> first);

        protected abstract bool RemoveItem(TSource oldItem, int index, Nullable<bool> last);

        protected abstract IDisposable GetFactorizable();

        protected abstract void OnClear();

        public abstract bool Invariant
        {
            get;
        }
    }
}
