using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;

using MusicCollection.Infra;
using MusicCollection.ToolBox.Event;


//some inspiration from CLINQ http://clinq.codeplex.com

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{

   

    internal abstract class LiveCollectionBase<TSource, TDest> : ROCollection<TDest>, IExtendedObservableCollection<TDest>, IInvariant
    {
        protected IList<TSource> _Source;

        internal LiveCollectionBase(IList<TSource> enumerable)
            : base()
        {
            _Source = enumerable;
        }

        protected abstract IEnumerable<TDest> Expected
        {
            get;
        }

        protected virtual void OnClear()
        {
            this.RealClear(true);
            this.RealAddRange(Expected,true);
            SendResetEvent();
        }

        public override IDisposable GetFactorizableEvent()
        {
            return this.GetEventFactorizable();
        }

        protected abstract void AddItem(TSource newItem, int index, Nullable<bool> First);

        protected virtual void AddItems(IEnumerable<Changed<TSource>> sources)
        {
            sources.Apply(s => AddItem(s.Source, s.Index, s.First));
        }

        protected abstract bool RemoveItem(TSource oldItem, int index, Nullable<bool> Last);

        public virtual bool Invariant
        {
            get
            {
                return Expected.SequenceEqual(this);
            }
        }

        public override void Dispose()
        {
        }
    }

    internal abstract class LiveCollectionNoFunction<TSource, TDest> : LiveCollectionBase<TSource, TDest>, IDisposable
    {
        private RegistorCollectionChanged<TSource> _RCF;

        protected LiveCollectionNoFunction(IList<TSource> source)
            : base(source)
        {
            _RCF = RegistorCollectionChanged<TSource>.GetListener(source, new ListenerAdapter(this));
        }

        #region listeneradapter

        private class ListenerAdapter : ICollectionListener<TSource>
        {
            private LiveCollectionNoFunction<TSource, TDest> _Father;
            internal ListenerAdapter(LiveCollectionNoFunction<TSource, TDest> iFather)
            {
                _Father = iFather;
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
                return _Father.GetFactorizableEvent();
            }

            void ICollectionListener<TSource>.OnClear()
            {
                _Father.OnClear();
            }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            _RCF.Dispose();
        }
    }



    internal abstract class LiveCollectionFunction<TSource, TDest, Tint> : LiveCollectionBase<TSource, TDest>, IInvariant, IDisposable
        where TSource : class
    {
        private RegistorCollectionChanged<TSource> _RCF;
        protected IFunction<TSource, Tint> _Function;

        protected LiveCollectionFunction(IList<TSource> source, Expression<Func<TSource, Tint>> Transformer)
            : base(source)
        {
            using (TimeTracer.TimeTrack(string.Format("Perf Register Collection")))
            {
                _Function = Transformer.CompileToObservable();
                _RCF = RegistrorCollectionIFunctionChanges<TSource, Tint>.GetListener(source, _Function, new ListenerAdapter(this));
            }
        }

        protected LiveCollectionFunction(IList<TSource> source, IFunction<TSource, Tint> Transformer)
            : base(source)
        {
            _Function = Transformer;
            _RCF = RegistrorCollectionIFunctionChanges<TSource, Tint>.GetListener(source, Transformer, new ListenerAdapter(this));
        }

        #region listeneradapter

        private class ListenerAdapter : ICollectionFunctionListener<TSource, Tint>
        {
            private LiveCollectionFunction<TSource, TDest, Tint> _Father;
            internal ListenerAdapter(LiveCollectionFunction<TSource, TDest, Tint> iFather)
            {
                _Father = iFather;
            }

            void ICollectionFunctionListener<TSource, Tint>.OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<Tint> changes)
            {
                _Father.OnCollectionItemPropertyChanged(item, changes);
            }

            void ICollectionListener<TSource>.AddItem(TSource newItem, int index, Nullable<bool> First)
            {
                _Father.AddItem(newItem, index, First);
            }

            public void AddItems(IEnumerable<Changed<TSource>> sources)
            {
                _Father.AddItems(sources);
            }

            bool ICollectionListener<TSource>.RemoveItem(TSource oldItem, int index, Nullable<bool> Last)
            {
                return _Father.RemoveItem(oldItem, index, Last);
            }

            IDisposable ICollectionListener<TSource>.GetFactorizable()
            {
                return _Father.GetFactorizableEvent();
            }

            void ICollectionListener<TSource>.OnClear()
            {
                _Father.OnClear();
            }

            public void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TSource, Tint> Changes)
            {
                _Father.OnCollectionItemsPropertyChanged(Changes);
            }

            public bool FactorizeEvents
            {
                get { return _Father.FactorizeEvents; }
            }


           
        }

        #endregion

        protected abstract void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<Tint> changes);

        protected virtual void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<TSource, Tint> Changes)
        {
            throw new NotImplementedException();
        }

        protected virtual bool FactorizeEvents
        {
            get { return false; }
        }

        public override void Dispose()
        {
            base.Dispose();
            _RCF.Dispose();
        }
    }
}

