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
    internal class LiveWhere<T> : LiveCollectionFunction<T, T, bool> where T : class
    {
        internal LiveWhere(IList<T> Orginal, Expression<Func<T, bool>> Transformer)
            : base(Orginal, Transformer)
        {
            this.RealAddRange(Expected,true);
        }

        internal LiveWhere(IList<T> Orginal, IFunction<T, bool> Transformer)
            : base(Orginal, Transformer)
        {
            this.RealAddRange(Expected,true);
        }

        protected override IEnumerable<T> Expected
        {
            get { return this._Source.Where(_Function.Evaluate); }
        }

        protected override bool FactorizeEvents
        {
            get
            {
                return true;
            }
        }

        //private int GetIndex(IList<T> NotFiltered, int IndexNotFilter)
        //{
        //    if (IndexNotFilter == 0)
        //        return 0;

        //    int Max = this.Count;

        //    int FilteredCount = 0;
        //    for(int i=0; i<IndexNotFilter; i++)
        //    {
        //        T raw = NotFiltered[i];

        //        if (FilteredCount<Max && object.Equals(raw, this[FilteredCount]))
        //            FilteredCount++;
        //    }

        //    return FilteredCount;
        //}

        private int GetIndex(int IndexNotFilter)
        {
            return _Source.GetIndex(this, IndexNotFilter);
        }

        private IEnumerable<int> IndexTobeInserted(T objectToInsert)
        {
            int Index = 0;
            int IndexToReturn = 0;
            int FilterCount = this.Count;

            foreach (T source in _Source)
            {

                if ((Index < FilterCount) && (object.ReferenceEquals(source, this[Index])))
                {
                    Index++;
                    IndexToReturn++;
                    continue;
                }

                if (object.ReferenceEquals(source, objectToInsert))
                {
                    yield return IndexToReturn;
                    IndexToReturn++;
                }
            }
        }

        private IEnumerable<int> IndexTobeRemoved(T objectToRemove)
        {
            int IndexToReturn = 0;
            int FilterCount = this.Count;

            for (int Index = 0; Index < FilterCount; Index++)
            {
                if (object.ReferenceEquals(this[Index], objectToRemove))
                {
                    yield return IndexToReturn;

                }
                else
                {
                    IndexToReturn++;
                }
            }
        }



        protected override void OnCollectionItemsPropertyChanged(ObjectAttributesChangedArgs<T, bool> Changes)
        {
            int FutureIndexFiltered = 0;
            int CurrentIndexFiltered = 0;
            int FilteredCount = this.Count;

            GroupedChangedRegistror replayer = GetReplayer();

            //List<Action> ToExecute = new List<Action>();
            foreach (T source in _Source)
            {
                bool IsElementInFilteredList = false;
                if ((CurrentIndexFiltered < FilteredCount) && (object.ReferenceEquals(source, this[CurrentIndexFiltered])))
                {
                    IsElementInFilteredList = true;
                }

                IObjectAttributeChanged<bool> objectchanges = Changes.GetChanges(source);
                if (objectchanges != null)
                {
                    if (objectchanges.New)
                    {
                        if (IsElementInFilteredList)
                            throw new Exception("Algo error in");

                        //int index = FutureIndexFiltered;
                        //T toadd = source;
                        //ToExecute.Add(() => this.RealInsert(index, toadd));
                        replayer.RegisterInsert(FutureIndexFiltered, source);
                        FutureIndexFiltered++;
                    }
                    else
                    {
                        if (!IsElementInFilteredList)
                            throw new Exception("Algo error out");

                        //int index = FutureIndexFiltered;
                        //ToExecute.Add(() => this.RealRemoveAt(index));
                        replayer.RegisterRemoveAt(FutureIndexFiltered);
                        FutureIndexFiltered--;

                    }
                }

                if (IsElementInFilteredList)
                {
                    FutureIndexFiltered++;
                    CurrentIndexFiltered++;
                }

            }

            replayer.Replay();
            //ToExecute.Apply(a => a());
        }


        protected override void OnCollectionItemPropertyChanged(T item, ObjectAttributeChangedArgs<bool> changes)
        {

            if (changes.New)
            {
                var computenow = IndexTobeInserted(item).ToList();
                computenow.Apply(i => this.RealInsert(i, item));
                ////_Source.Indexes(item).Apply(i => this.RealInsert(GetIndex(i), item));
            }
            else
            {
                var computenow = IndexTobeRemoved(item).ToList();
                computenow.Apply(i => this.RealRemoveAt(i));

                //this.Indexes(item).Apply(i => this.RealRemoveAt(i));
                ////while (this.Contains(item))
                ////{
                ////    this.RealRemove(item);
                ////}
            }
        }

        protected override void AddItem(T newItem, int index, Nullable<bool> first)
        {
            if (!_Function.Evaluate(newItem))
                return;

            this.RealInsert(GetIndex(index), newItem);
        }

        protected override bool RemoveItem(T oldItem, int index, Nullable<bool> last)
        {
            if (!_Function.Evaluate(oldItem))
                return true;

            this.RealRemoveAt(GetIndex(index));
            return true;
        }
    }
}
