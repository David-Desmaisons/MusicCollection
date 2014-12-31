using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;


namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    internal interface ICollectionFunctionListener<TSource, TDest> : ICollectionListener<TSource>
    {
        void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<TDest> changes);

        void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TSource,TDest> Changes);

        bool FactorizeEvents { get; }
    }

    internal class RegistrorCollectionIFunctionNoneParametricChanges<TSource, TDest> : RegistorCollectionChanged<TSource> where TSource : class
    {
        protected IFunction<TSource, TDest> _Function;
        private ICollectionFunctionListener<TSource, TDest> _IFL;

        internal RegistrorCollectionIFunctionNoneParametricChanges(IEnumerable<TSource> enumerable, IFunction<TSource, TDest> Function, ICollectionFunctionListener<TSource, TDest> iIFL):
            base(enumerable, iIFL)
        {
            _Function = Function;

            _IFL = iIFL;
            _Function.FactorizeEvent = _IFL.FactorizeEvents;
            _Function.ElementChanged += Individualchanges;
            _Function.ElementsChanged += Factorizedchanges;
        }

        private void Individualchanges(object sender, ObjectAttributeChangedArgs<TDest> e)
        {
            _IFL.OnCollectionItemPropertyChanged((TSource)e.ModifiedObject, e);
        }

        private void Factorizedchanges(object sender, ObjectAttributesChangedArgs<TSource, TDest> e)
        {
            _IFL.OnCollectionItemsPropertyChanged(e);
        }

        protected override void UnListenAll()
        {
            _Function.UnListenAll();
        }

        protected override Nullable<bool> SubscribeToItem(TSource item)
        {
            return null;
        }

        protected override Nullable<bool> NeedToRemove(TSource item)
        {
            return null;
        }

        protected override Nullable<bool> UnsubscribeFromItem(TSource item)
        {
            return null;
        }

        private bool _Disposed = false;
        public override void Dispose()
        {
            if (!_Disposed)
            {
                base.Dispose();
                _Disposed = true;
                _Function.ElementChanged -= Individualchanges;
                _Function.ElementsChanged -= Factorizedchanges;
                _Function.Dispose();
            }
        }

    }

    internal class RegistrorCollectionIFunctionChanges<TSource, TDest> : RegistrorCollectionIFunctionNoneParametricChanges<TSource,TDest> where TSource : class
    {

        private RegistrorCollectionIFunctionChanges(IEnumerable<TSource> enumerable, IFunction<TSource, TDest> Function, ICollectionFunctionListener<TSource, TDest> iIFL):
            base(enumerable, Function, iIFL)
        {
        }


        static internal RegistorCollectionChanged<TSource> GetListener(IList<TSource> enumerable, IFunction<TSource, TDest> Function, ICollectionFunctionListener<TSource, TDest> iIFL)
        {
            RegistorCollectionChanged<TSource> res = null;

            if (Function.IsParameterDynamic)
            {
                res = new RegistrorCollectionIFunctionChanges<TSource, TDest>(enumerable, Function, iIFL);
            }
            else
            {
                res = new RegistrorCollectionIFunctionNoneParametricChanges<TSource, TDest>(enumerable, Function, iIFL);
            }

            res.Register(enumerable);
            return res;
        }

        internal override void Register(IList<TSource> Source)
        {
            Source.Apply(t => _Function.Register(t));
        }
    
        protected override Nullable<bool> SubscribeToItem(TSource item)
        {
            return _Function.Register(item);
        }

        protected override Nullable<bool> NeedToRemove(TSource item)
        {
            return _Function.IsSingleRegistered(item);
        }

        protected override Nullable<bool> UnsubscribeFromItem(TSource item)
        {
            return _Function.UnRegister(item);
        }    
    }
}
