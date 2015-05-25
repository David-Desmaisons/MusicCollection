using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

using MusicCollection.Infra;
using System.Diagnostics;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class LiveSelectCollection<TSource, TDes> : LiveCollectionFunction<TSource, TDes, TDes> where TSource : class
    {
        internal LiveSelectCollection(IList<TSource> Orginal, Expression<Func<TSource, TDes>> Transformer)
            : base(Orginal, Transformer)
        {
            this.RealAddRange(Expected,true);
        }

        private LiveSelectCollection(IList<TSource> source, IFunction<TSource, TDes> Transformer)
            : base(source, Transformer)
        {
            this.RealAddRange(Expected,true);
        }

        static public LiveSelectCollection<TSource, TDes> FromFunction(IList<TSource> source, Func<TSource, TDes> Transformer)
        {
            return new LiveSelectCollection<TSource, TDes>(source, Transformer.CompileToConst());
        }

        protected override IEnumerable<TDes> Expected
        {
            get { return this._Source.Select(_Function.Evaluate); }
        }


        protected override void OnCollectionItemPropertyChanged(TSource item, ObjectAttributeChangedArgs<TDes> changes)
        {
            _Source.Indexes(item).Apply(i => this.RealOverWrite(i, changes.New));
        }

        protected override void AddItem(TSource newItem, int index, Nullable<bool> first)
        {
            TDes mynew = _Function.Evaluate(newItem);
            if (index == this.Count)
            {
                this.RealAdd(_Function.Evaluate(newItem));
            }
            else
            {
                this.RealInsert(index, mynew);
            }
        }

        protected override void AddItems(IEnumerable<Changed<TSource>> sources)
        {
            var changed = sources.FirstOrDefault();
            if (changed==null)
                return;

            this.RealInsertRange(changed.Index,sources.Select(t=>_Function.Evaluate(t.Source)).ToList());
        }

        protected override bool RemoveItem(TSource oldItem, int index, Nullable<bool> last)
        {
            this.RealRemoveAt(index);
            return true;
        }


    }
}
