using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using MusicCollection.Infra;

namespace MusicCollection.ToolBox.Collection.Observable.LiveQuery
{
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class LiveDistinct<T> : LiveCollectionNoFunction<T, T>, IInvariant
    {
        internal LiveDistinct(IList<T> list)
            : base(list)
        {
            this.RealAddRange(Expected,true);
        }

        public override bool Invariant
        {
            get
            {
                return Expected.SequenceEqual(this);
            }
        }

   
        protected override IEnumerable<T> Expected
        {
            get { return this._Source.Distinct(); }
        }


        private bool GetIndexForUnicItem(T item, int inewindex,out int onewindex)
        {        
            if (inewindex == _Source.Count-1)
            {
                onewindex = -1;
                return true;
            }

            if (inewindex == 0)
            {
                onewindex = 0;
                return false;
            }

            onewindex = _Source.Take(inewindex).Distinct().Count();
            return false;
        }

        private bool UpdateIndexIfNeeded(T item)
        {
            int ni = Expected.Index(item);
            int oi = this.IndexOf(item);
            if (ni == oi)
                return false;

            this.RealMove(oi, ni);
            return true;
        }

        protected override void AddItem(T newItem, int index, Nullable<bool> first)
        {
            bool RealFirst = false;

            if (first != null)
            {
                RealFirst = first.Value;
            }
            else
            {
                RealFirst = !this.Contains(newItem);
            }


            if (!RealFirst)
            {
                UpdateIndexIfNeeded(newItem);
                return;
            }

            int newindex=0;

            if (GetIndexForUnicItem(newItem, index, out newindex))
                this.RealAdd(newItem);
            else
                this.RealInsert(newindex, newItem);       
        }

        protected override bool RemoveItem(T oldItem, int index, Nullable<bool> last)
        {
            bool RealLast = false;

            if (last != null)
            {
                RealLast = last.Value;
            }
            else
            {
                RealLast = !_Source.Contains(oldItem);
            }

            if (!RealLast)
            {
                return UpdateIndexIfNeeded(oldItem);
            }

            this.RealRemove(oldItem);
            return true;
        }
    }
}
