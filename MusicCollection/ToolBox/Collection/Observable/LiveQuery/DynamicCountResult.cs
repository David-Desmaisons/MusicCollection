using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{

    internal sealed class DynamicCountResult<TSource> : DynamicResultNoFunction<TSource,int >
    {
        static WeakDictionary<IList<TSource>, DynamicCountResult<TSource>> _Cache;

        static private WeakDictionary<IList<TSource>, DynamicCountResult<TSource>> Cache
        {
            get
            {
                if (_Cache == null)
                    _Cache = new WeakDictionary<IList<TSource>, DynamicCountResult<TSource>>();

                return _Cache;
            }
        }


        static internal DynamicCountResult<TSource> GetDynamicCountResult(IList<TSource> source)
        {
            return Cache.FindOrCreateEntity(source, (s) => new DynamicCountResult<TSource>(s));
        }

        private DynamicCountResult(IList<TSource> source)
            : base(source)
        {
            Value = source.Count;
        }


        private NoUpadte _Trav;
        protected override IDisposable GetFactorizable()
        {
            if (_Trav == null)
            {
                _Trav = new NoUpadte(this);
            }

            return _Trav;
        }

        private class NoUpadte : IDisposable
        {
            private DynamicCountResult<TSource> _Father;
            internal NoUpadte(DynamicCountResult<TSource> Father)
            {
                _Father = Father;
            }

            public void Dispose()
            {
                _Father._Trav = null;
                _Father.Update();
            }
        }

        private void Update()
        {
            if (_Trav != null)
                return;

            Value = _Source.Count;
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            Update();
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            Update();
            return true;
        }

        protected override void OnClear()
        {
            Update();
        }

        public override bool Invariant
        {
            get
            {
                return (Value == _Source.Count);
            }
        }
    }
}
